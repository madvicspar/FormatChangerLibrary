using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public class TableCellCorrectionStrategy : IElementCorrectionStrategy<CellSettingsModel>
    {
        public CellSettingsModel GetSettings(FormattingTemplateModel template)
        {
            return template.TableSettings.CellSettings;
        }
        public RunProperties GetRunProperties(CellSettingsModel settings)
        {
            return new RunProperties(
                new RunFonts { Ascii = settings.TextSettings.Font, HighAnsi = settings.TextSettings.Font },
                new Color { Val = settings.TextSettings.Color },
                new Bold { Val = settings.TextSettings.IsBold },
                new Italic { Val = settings.TextSettings.IsItalic },
                new Underline { Val = settings.TextSettings.IsUnderscore ? UnderlineValues.Single : UnderlineValues.None },
                new FontSize { Val = (settings.TextSettings.FontSize * 2).ToString() }
            );
        }

        public ParagraphProperties GetParagraphProperties(CellSettingsModel settings)
        {
            return new ParagraphProperties(
                new SpacingBetweenLines
                {
                    Line = settings.TextSettings.LineSpacing.ToString(),
                    LineRule = LineSpacingRuleValues.Auto,
                    Before = settings.TextSettings.BeforeSpacing.ToString(),
                    After = settings.TextSettings.AfterSpacing.ToString()
                },
                new Indentation
                {
                    Left = settings.TextSettings.Left.ToString(),
                    Right = settings.TextSettings.Right.ToString(),
                    FirstLine = ((int)(settings.TextSettings.FirstLine * 567)).ToString()
                },
                new Justification { Val = JustificationConverter.Parse(settings.TextSettings.Justification) },
                new KeepNext { Val = settings.TextSettings.KeepWithNext }
            );
        }

        public void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var tables = doc.MainDocumentPart?.Document?.Body?.Descendants<Table>().ToList();
            if (tables == null) return;

            foreach (var table in tables)
            {
                var cells = table.Descendants<TableCell>().ToList();
                foreach (var cell in cells)
                {
                    var cellProperties = cell.Elements<TableCellProperties>().FirstOrDefault();
                    if (cellProperties == null)
                    {
                        cellProperties = new TableCellProperties();
                        cell.PrependChild(cellProperties);
                    }

                    // в пт
                    cellProperties.RemoveAllChildren<TableCellMargin>();

                    var margins = new TableCellMargin(
                        new LeftMargin { Width = (settings.LeftPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                        new RightMargin { Width = (settings.RightPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                        new TopMargin { Width = (settings.TopPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                        new BottomMargin { Width = (settings.BottomPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa }
                    );

                    cellProperties.AppendChild(margins);

                    // TODO: парсер
                    // Устанавливаем выравнивание текста в ячейке
                    var verticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Top };

                    cellProperties.AppendChild(verticalAlignment);

                    var paragraphProperties = GetParagraphProperties(settings);
                    var runProperties = GetRunProperties(settings);

                    foreach (Paragraph paragraph in cell.Elements<Paragraph>())
                    {
                        paragraph.RemoveAllChildren<ParagraphProperties>();
                        paragraph.PrependChild(paragraphProperties.CloneNode(true));

                        foreach (Run run in paragraph.Elements<Run>())
                        {
                            run.RemoveAllChildren<RunProperties>();
                            run.PrependChild(runProperties.CloneNode(true));
                        }
                    }
                }
            }
        }
    }
}
