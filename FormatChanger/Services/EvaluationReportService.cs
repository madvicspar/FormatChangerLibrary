using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Services.Interfaces;
using System.Text.Json;

namespace FormatChanger.Services
{
    public class EvaluationReportService : IEvaluationReportService
    {
        private static readonly (string Id, string Label, (string Id, string Label, int W)[] Rules)[] Types =
        [
            ("normal", "Основной текст",
            [
                ("font",    "Шрифт",                20),
                ("size",    "Размер шрифта",         20),
                ("align",   "Выравнивание",          15),
                ("spacing", "Межстрочный интервал",  20),
                ("indent",  "Отступ первой строки",  15),
                ("margins", "Боковые отступы",       10),
            ]),
            ("heading", "Заголовки",
            [
                ("font",    "Шрифт",                    20),
                ("size",    "Размер шрифта",             25),
                ("bold",    "Полужирное начертание",     20),
                ("align",   "Выравнивание",              15),
                ("newpage", "Начало с новой страницы",   20),
            ]),
            ("list", "Списки",
            [
                ("font",   "Шрифт",               20),
                ("marker", "Маркер",               30),
                ("end",    "Знак конца элемента",  30),
                ("indent", "Отступ",               20),
            ]),
            ("image", "Изображения",
            [
                ("align",   "Выравнивание",     30),
                ("caption", "Наличие подписи",  40),
                ("spacing", "Интервалы",        30),
            ]),
            ("table", "Таблицы",
            [
                ("caption", "Наличие подписи",    35),
                ("header",  "Строка заголовка",   25),
                ("spacing", "Интервалы",           20),
                ("align",   "Выравнивание ячеек", 20),
            ]),
        ];

        private static readonly Dictionary<string, Dictionary<string, int>> DemoScores = new()
        {
            ["normal"]  = new() { ["font"] = 90, ["size"] = 85, ["align"] = 95, ["spacing"] = 80, ["indent"] = 75, ["margins"] = 90 },
            ["heading"] = new() { ["font"] = 85, ["size"] = 90, ["bold"] = 100, ["align"] = 95, ["newpage"] = 70 },
            ["list"]    = new() { ["font"] = 85, ["marker"] = 90, ["end"] = 80, ["indent"] = 85 },
            ["image"]   = new() { ["align"] = 90, ["caption"] = 70, ["spacing"] = 85 },
            ["table"]   = new() { ["caption"] = 75, ["header"] = 95, ["spacing"] = 80, ["align"] = 85 },
        };

        public byte[] GenerateReport(EvaluationSystemModel scoring, FormattingTemplateModel template)
        {
            var typeWeights = new Dictionary<string, int>
            {
                ["normal"]  = scoring.TextWeight,
                ["heading"] = scoring.HeaderWeight,
                ["list"]    = scoring.ListWeight,
                ["image"]   = scoring.ImageWeight,
                ["table"]   = scoring.TableWeight,
            };

            var ruleWeights = ParseRuleWeights(scoring.RuleWeightsJson);

            using var mem = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(mem, WordprocessingDocumentType.Document))
            {
                var main = doc.AddMainDocumentPart();
                main.Document = new Document();
                var body = main.Document.AppendChild(new Body());

                body.AppendChild(TitlePara("Отчёт по оцениванию документа"));
                body.AppendChild(InfoPara("Шаблон форматирования:", template?.Title ?? "—"));
                body.AppendChild(InfoPara("Система оценивания:", scoring.Title ?? "—"));
                body.AppendChild(InfoPara("Дата формирования:", DateTime.Now.ToString("dd.MM.yyyy HH:mm")));
                body.AppendChild(EmptyPara());

                double totalScore = CalcTotal(typeWeights, ruleWeights);
                body.AppendChild(SummaryPara($"Итоговый балл: {totalScore:F1} / 100"));
                body.AppendChild(EmptyPara());

                body.AppendChild(BuildTable(typeWeights, ruleWeights));
                body.AppendChild(EmptyPara());

                //body.AppendChild(NotePara("* Отчёт сформирован в демонстрационном режиме. Фактическое оценивание находится в разработке."));

                var sectPr = new SectionProperties(
                    new PageSize { Width = 11906, Height = 16838 },
                    new PageMargin { Top = 1134, Right = 850, Bottom = 1134, Left = 1134 }
                );
                body.AppendChild(sectPr);

                main.Document.Save();
            }

            return mem.ToArray();
        }

        private static Dictionary<string, Dictionary<string, int>> ParseRuleWeights(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                try { return JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json); }
                catch { }
            }
            return Types.ToDictionary(t => t.Id, t => t.Rules.ToDictionary(r => r.Id, r => r.W));
        }

        private double CalcTotal(Dictionary<string, int> typeWeights, Dictionary<string, Dictionary<string, int>> ruleWeights)
        {
            double total = 0;
            foreach (var type in Types)
            {
                int tw = typeWeights.GetValueOrDefault(type.Id);
                var rw = ruleWeights.GetValueOrDefault(type.Id, new());
                int rsum = rw.Values.Sum(); if (rsum == 0) rsum = 100;
                foreach (var rule in type.Rules)
                {
                    double contrib = (double)tw * rw.GetValueOrDefault(rule.Id, rule.W) / rsum;
                    total += contrib * DemoScores[type.Id].GetValueOrDefault(rule.Id, 85) / 100.0;
                }
            }
            return total;
        }

        // ── Document element helpers ─────────────────────────────────────────

        private static Paragraph TitlePara(string text) => new(
            PP(new Justification { Val = JustificationValues.Center }, new SpacingBetweenLines { After = "200" }),
            Run(text, bold: true, size: 32, color: "1F3864")
        );

        private static Paragraph InfoPara(string label, string value) => new(
            PP(new SpacingBetweenLines { After = "60" }),
            Run(label + " ", bold: true, size: 22),
            Run(value, size: 22)
        );

        private static Paragraph SummaryPara(string text) => new(
            new ParagraphProperties(
                new Justification { Val = JustificationValues.Center },
                new SpacingBetweenLines { After = "100" },
                new ParagraphBorders(
                    new TopBorder    { Val = BorderValues.Single, Size = 6, Color = "1F3864", Space = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 6, Color = "1F3864", Space = 4 },
                    new LeftBorder   { Val = BorderValues.Single, Size = 6, Color = "1F3864", Space = 8 },
                    new RightBorder  { Val = BorderValues.Single, Size = 6, Color = "1F3864", Space = 8 }
                ),
                new Shading { Fill = "D6E4F7", Val = ShadingPatternValues.Clear }
            ),
            Run(text, bold: true, size: 26, color: "1F3864")
        );

        private static Paragraph NotePara(string text) => new(
            PP(new SpacingBetweenLines { After = "60" }),
            Run(text, size: 18, color: "777777", italic: true)
        );

        private static Paragraph EmptyPara() => new(PP(new SpacingBetweenLines { After = "80" }));

        private static ParagraphProperties PP(params OpenXmlElement[] children)
        {
            var pp = new ParagraphProperties();
            foreach (var c in children) pp.AppendChild(c);
            return pp;
        }

        private static Run Run(string text, bool bold = false, int size = 22, string color = null, bool italic = false)
        {
            var rp = new RunProperties(
                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                new FontSize { Val = size.ToString() }
            );
            if (bold) rp.AppendChild(new Bold());
            if (italic) rp.AppendChild(new Italic());
            if (color != null) rp.AppendChild(new Color { Val = color });
            return new Run(rp, new Text(text) { Space = SpaceProcessingModeValues.Preserve });
        }

        // ── Table ────────────────────────────────────────────────────────────

        // Column widths in DXA: Type | Rule | TypeW | RuleW | Contrib | Score | Weighted
        private static readonly int[] ColW = [2000, 2800, 950, 950, 800, 800, 854];
        private static readonly int TotalW = 9154;

        private Table BuildTable(Dictionary<string, int> typeWeights, Dictionary<string, Dictionary<string, int>> ruleWeights)
        {
            var table = new Table(
                new TableProperties(
                    new TableWidth { Width = TotalW.ToString(), Type = TableWidthUnitValues.Dxa },
                    new TableBorders(
                        MakeBorder<TopBorder>(), MakeBorder<BottomBorder>(),
                        MakeBorder<LeftBorder>(), MakeBorder<RightBorder>(),
                        MakeBorder<InsideHorizontalBorder>(), MakeBorder<InsideVerticalBorder>()
                    )
                )
            );

            // Header
            string[] headers = ["Тип элемента", "Правило", "Вес типа (%)", "Вес правила (%)", "Вклад (%)", "Оценка (%)", "Балл"];
            var headerRow = new TableRow();
            for (int i = 0; i < headers.Length; i++)
                headerRow.AppendChild(MakeCell(ColW[i], headers[i], "FFFFFF", "1F3864", bold: true, center: true));
            table.AppendChild(headerRow);

            // Data rows
            bool alt = false;
            foreach (var type in Types)
            {
                int tw = typeWeights.GetValueOrDefault(type.Id);
                var rw = ruleWeights.GetValueOrDefault(type.Id, new());
                int rsum = rw.Values.Sum(); if (rsum == 0) rsum = 100;

                bool first = true;
                foreach (var rule in type.Rules)
                {
                    int rulew = rw.GetValueOrDefault(rule.Id, rule.W);
                    double contrib = (double)tw * rulew / rsum;
                    int demo = DemoScores[type.Id].GetValueOrDefault(rule.Id, 85);
                    double weighted = contrib * demo / 100.0;

                    string bg = alt ? "F0F4FA" : "FFFFFF";
                    string typeBg = first ? "E1EAF8" : bg;
                    string scoreColor = demo >= 90 ? "1E6B2E" : demo >= 75 ? "7B4F00" : "8B1A1A";

                    var row = new TableRow();
                    row.AppendChild(MakeCell(ColW[0], first ? type.Label : "", "333333", typeBg, bold: first));
                    row.AppendChild(MakeCell(ColW[1], rule.Label, "333333", bg));
                    row.AppendChild(MakeCell(ColW[2], tw.ToString(), "444444", bg, center: true));
                    row.AppendChild(MakeCell(ColW[3], rulew.ToString(), "444444", bg, center: true));
                    row.AppendChild(MakeCell(ColW[4], contrib.ToString("F1"), "444444", bg, center: true));
                    row.AppendChild(MakeCell(ColW[5], demo + "%", scoreColor, bg, bold: true, center: true));
                    row.AppendChild(MakeCell(ColW[6], weighted.ToString("F2"), "333333", bg, center: true));
                    table.AppendChild(row);
                    first = false;
                }
                alt = !alt;
            }

            // Total row
            double total = CalcTotal(typeWeights, ruleWeights);
            int labelWidth = ColW[0] + ColW[1] + ColW[2] + ColW[3] + ColW[4] + ColW[5];
            var totalRow = new TableRow();
            totalRow.AppendChild(MakeMergedCell(labelWidth, 6, "Итоговый балл (демо):", "FFFFFF", "1F3864", bold: true, right: true));
            totalRow.AppendChild(MakeCell(ColW[6], total.ToString("F1"), "FFFFFF", "1F3864", bold: true, center: true));
            table.AppendChild(totalRow);

            return table;
        }

        private static T MakeBorder<T>() where T : BorderType, new() =>
            new() { Val = BorderValues.Single, Size = 4, Color = "BBBBBB" };

        private static TableCell MakeCell(int width, string text, string textColor, string bgColor,
            bool bold = false, bool center = false, bool italic = false)
        {
            var rp = new RunProperties(
                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                new FontSize { Val = "18" },
                new Color { Val = textColor }
            );
            if (bold) rp.AppendChild(new Bold());
            if (italic) rp.AppendChild(new Italic());

            var pp = new ParagraphProperties(new SpacingBetweenLines { After = "0" });
            if (center) pp.AppendChild(new Justification { Val = JustificationValues.Center });

            var para = new Paragraph(pp, new Run(rp, new Text(text) { Space = SpaceProcessingModeValues.Preserve }));

            return new TableCell(
                new TableCellProperties(
                    new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
                    new Shading { Fill = bgColor, Val = ShadingPatternValues.Clear }
                ),
                para
            );
        }

        private static TableCell MakeMergedCell(int width, int span, string text, string textColor, string bgColor,
            bool bold = false, bool right = false)
        {
            var rp = new RunProperties(
                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                new FontSize { Val = "18" },
                new Color { Val = textColor }
            );
            if (bold) rp.AppendChild(new Bold());

            var pp = new ParagraphProperties(new SpacingBetweenLines { After = "0" });
            if (right) pp.AppendChild(new Justification { Val = JustificationValues.Right });

            var para = new Paragraph(pp, new Run(rp, new Text(text) { Space = SpaceProcessingModeValues.Preserve }));

            return new TableCell(
                new TableCellProperties(
                    new TableCellWidth { Width = width.ToString(), Type = TableWidthUnitValues.Dxa },
                    new GridSpan { Val = span },
                    new Shading { Fill = bgColor, Val = ShadingPatternValues.Clear }
                ),
                para
            );
        }
    }
}
