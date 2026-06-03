using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Strategies
{
    public class ImageCorrectionStrategy : ElementCorrectionStrategyBase<ImageSettingsModel>
    {
        public override ImageSettingsModel GetSettings(FormattingTemplateModel template) =>
            template.ImageSettings;

        public override void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().ToList();
            if (paragraphs == null) return;

            var paraProps = GetParagraphProperties(settings);

            foreach (var paragraph in paragraphs)
            {
                // TODO: скорее всего нужно так же исправлять настройки текста для параграфа с рисунками
                if (paragraph.Descendants<Drawing>().Any())
                    paragraph.ParagraphProperties = paraProps.CloneNode(true) as ParagraphProperties;
            }
        }

        public ParagraphProperties GetParagraphProperties(ImageSettingsModel settings)
        {
            return new ParagraphProperties(
                new Justification { Val = JustificationConverter.Parse(settings.Justification) },
                new SpacingBetweenLines
                {
                    Line = settings.LineSpacing.ToString(),
                    LineRule = LineSpacingRuleValues.Auto,
                    Before = settings.BeforeSpacing.ToString(),
                    After = settings.AfterSpacing.ToString()
                },
                new Indentation
                {
                    Left = settings.Left.ToString(),
                    Right = settings.Right.ToString(),
                    FirstLine = settings.FirstLine.ToString()
                },
                new KeepNext { Val = settings.KeepWithNext });
        }

        public override List<string> CheckFormatting(ParagraphStyleProperties actual, FormattingTemplateModel template)
        {
            var expected = GetSettings(template);
            return FormattingChecker.CheckImage(actual, expected);
        }

        /// <summary>
        /// Проверяет форматирование параграфа с рисунком, читая свойства напрямую —
        /// точно так же, как GetParagraphProperties их записывает (сырые значения, без иерархии стилей).
        /// </summary>
        public List<string> CheckParagraph(Paragraph p, ImageSettingsModel settings)
        {
            var issues = new List<string>();
            var pp = p.ParagraphProperties;
            var spacing = pp?.SpacingBetweenLines;
            var indent = pp?.Indentation;
            var just = pp?.Justification;

            CompareRaw("Междустрочный интервал", spacing?.Line?.Value, settings.LineSpacing, 240.0, issues);
            CompareRaw("Интервал перед", spacing?.Before?.Value, settings.BeforeSpacing, 20.0, issues);
            CompareRaw("Интервал после", spacing?.After?.Value, settings.AfterSpacing, 20.0, issues);
            CompareRaw("Отступ слева", indent?.Left?.Value, settings.Left, 567.0, issues);
            CompareRaw("Отступ справа", indent?.Right?.Value, settings.Right, 567.0, issues);
            CompareRaw("Отступ первой строки", indent?.FirstLine?.Value, settings.FirstLine, 567.0, issues);

            var expectedJust = JustificationConverter.Parse(settings.Justification).ToString().ToLowerInvariant();
            var actualJust = just?.Val?.Value.ToString()?.ToLowerInvariant();
            if ((actualJust ?? "") != (expectedJust ?? ""))
                issues.Add($"Выравнивание: {actualJust ?? "не задано"}, должно быть {expectedJust ?? "не задано"}");

            return issues;
        }

        private static void CompareRaw(string name, string actualRaw, float expectedRaw, double divisor, List<string> issues)
        {
            var expectedNorm = Math.Round(expectedRaw / divisor, 2);
            if (expectedNorm == 0 && actualRaw == null) return;

            double? actualNorm = null;
            if (actualRaw != null && double.TryParse(actualRaw, out var v))
                actualNorm = Math.Round(v / divisor, 2);

            if (actualNorm == null || actualNorm != expectedNorm)
                issues.Add($"{name}: {(actualNorm.HasValue ? actualNorm.ToString() : "не задан")}, должен быть {expectedNorm}");
        }
    }
}
