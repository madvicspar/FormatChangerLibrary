using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public class HeadingFirstCorrectionStrategy : IElementCorrectionStrategy<HeadingSettingsModel>
    {
        public HeadingSettingsModel GetSettings(FormattingTemplateModel template)
        {
            return template.HeadingSettings;
        }
        public RunProperties GetRunProperties(HeadingSettingsModel settings)
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

        public ParagraphProperties GetParagraphProperties(HeadingSettingsModel settings)
        {
            // TODO: Add numbering
            var paragraphProperties = new ParagraphProperties(
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
                FirstLine = settings.TextSettings.FirstLine.ToString()
            },
            new Justification { Val = JustificationConverter.Parse(settings.TextSettings.Justification) },
            new KeepNext { Val = settings.TextSettings.KeepWithNext }
            );

            if (settings.StartOnNewPage)
                paragraphProperties.AddChild(new PageBreakBefore());
            return paragraphProperties;
        }

        public void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var stylePart = doc.MainDocumentPart?.StyleDefinitionsPart;
            if (stylePart?.Styles == null) return;

            var style = stylePart.Styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "1");
            if (style == null)
            {
                Console.WriteLine("Style 'Heading' not found.");
                return;
            }

            style.RemoveAllChildren<StyleRunProperties>();
            style.RemoveAllChildren<StyleParagraphProperties>();

            style.AppendChild(new StyleRunProperties(GetRunProperties(settings)));
            style.AppendChild(new StyleParagraphProperties(GetParagraphProperties(settings)));
        }

        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
        {
            var issues = new List<string>();
            var settings = GetSettings(template);

            var expectedRunProps = GetRunProperties(settings);
            var expectedParaProps = GetParagraphProperties(settings);

            var actualRunProps = paragraph.Descendants<RunProperties>().FirstOrDefault();
            var actualParaProps = paragraph.ParagraphProperties;

            string styleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val;
            Style style = null;
            StyleRunProperties styleRunProps = null;
            StyleParagraphProperties styleParagraphProps = null;

            IEnumerable<Style> styles = new List<Style>();
            var document = paragraph.Ancestors<Document>().FirstOrDefault();
            if (document != null)
            {
                var stylePart = document.MainDocumentPart?.StyleDefinitionsPart;
                if (stylePart != null)
                {
                    styles = stylePart.Styles.Elements<Style>();
                    style = stylePart.Styles.Elements<Style>().FirstOrDefault(s => s.StyleId == styleId);
                    styleRunProps = style?.StyleRunProperties;
                    styleParagraphProps = style?.StyleParagraphProperties;
                }
            }

            

            CompareRunProperties(paragraph, actualRunProps, expectedRunProps, styles, issues);
            CompareParagraphProperties(actualParaProps, expectedParaProps, styleParagraphProps, issues);

            return issues;
        }

        RunFonts GetFont(Paragraph p, IEnumerable<Style> styles)
        {
            // если styleId = null, то проверяем стиль обычный
            Style style;
            var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;

            do
            {
                style = styles.First(s => s.StyleId == styleId);
                styleId = style?.BasedOn?.Val;
            } while (style.StyleRunProperties?.RunFonts?.Ascii == null);

            return style?.StyleRunProperties?.RunFonts;
        }


        private void CompareRunProperties(Paragraph p, RunProperties actual, RunProperties expected, IEnumerable<Style> styles, List<string> issues)
        {
            var actualFont = actual?.RunFonts?.Ascii?.Value ?? GetFont(p, styles)?.Ascii?.Value;
            var expectedFont = expected.RunFonts?.Ascii?.Value;

            if (actualFont != expectedFont)
            {
                issues.Add($"Шрифт: {actualFont ?? "не задан"}, должен быть {expectedFont}");
            }

            //string actualColor = actual?.Color?.Val ?? style?.Color?.Val;
            //if (actualColor != expected.Color?.Val)
            //    issues.Add($"Цвет текста: {actualColor ?? "не задан"}, должен быть {expected.Color?.Val}");

            //string actualFontSize = actual?.FontSize?.Val ?? style?.FontSize?.Val;
            //if (actualFontSize != expected.FontSize?.Val)
            //    issues.Add($"Размер шрифта: {actualFontSize ?? "не задан"}, должен быть {expected.FontSize?.Val}");

            //bool isBold = actual?.Bold != null || (actual?.Bold == null && style?.Bold != null);
            //bool shouldBeBold = expected.Bold?.Val == true;
            //if (isBold != shouldBeBold)
            //    issues.Add(shouldBeBold ? "Должен быть полужирным" : "Не должен быть полужирным");

            //bool isItalic = actual?.Italic != null || (actual?.Italic == null && style?.Italic != null);
            //bool shouldBeItalic = expected.Italic?.Val == true;
            //if (isItalic != shouldBeItalic)
            //    issues.Add(shouldBeItalic ? "Должен быть курсивным" : "Не должен быть курсивным");

            //bool isUnderlined = (actual?.Underline != null && actual?.Underline.Val != UnderlineValues.None) ||
            //                    (actual?.Underline == null && style?.Underline != null && style.Underline.Val != UnderlineValues.None);
            //bool shouldBeUnderlined = expected.Underline?.Val == UnderlineValues.Single;
            //if (isUnderlined != shouldBeUnderlined)
            //    issues.Add(shouldBeUnderlined ? "Должен быть подчеркнут" : "Не должен быть подчеркнут");
        }

        private void CompareParagraphProperties(ParagraphProperties actual, ParagraphProperties expected, StyleParagraphProperties style, List<string> issues)
        {
            //var actualSpacing = actual?.SpacingBetweenLines ?? style?.SpacingBetweenLines;
            //var expectedSpacing = expected.SpacingBetweenLines;

            //if (actualSpacing?.Line != expectedSpacing?.Line)
            //    issues.Add($"Неверный межстрочный интервал: {actualSpacing?.Line ?? "не задан"}, должен быть {expectedSpacing?.Line}");

            //if (actualSpacing?.Before != expectedSpacing?.Before)
            //    issues.Add($"Неверный отступ перед: {actualSpacing?.Before ?? "не задан"}, должен быть {expectedSpacing?.Before}");

            //if (actualSpacing?.After != expectedSpacing?.After)
            //    issues.Add($"Неверный отступ после: {actualSpacing?.After ?? "не задан"}, должен быть {expectedSpacing?.After}");

            //var actualIndentation = actual?.Indentation ?? style?.Indentation;
            //var expectedIndentation = expected.Indentation;

            //if (actualIndentation?.Left != expectedIndentation?.Left)
            //    issues.Add($"Неверный отступ слева: {actualIndentation?.Left ?? "не задан"}, должен быть {expectedIndentation?.Left}");

            //if (actualIndentation?.Right != expectedIndentation?.Right)
            //    issues.Add($"Неверный отступ справа: {actualIndentation?.Right ?? "не задан"}, должен быть {expectedIndentation?.Right}");

            //if (actualIndentation?.FirstLine != expectedIndentation?.FirstLine)
            //    issues.Add($"Неверный отступ первой строки: {actualIndentation?.FirstLine ?? "не задан"}, должен быть {expectedIndentation?.FirstLine}");

            var actualSpacing = actual?.SpacingBetweenLines ?? style?.SpacingBetweenLines;
            var expectedSpacing = expected.SpacingBetweenLines;

            if (actualSpacing?.Line != expectedSpacing?.Line)
                issues.Add($"Неверный межстрочный интервал: {actualSpacing?.Line ?? "не задан"}, должен быть {expectedSpacing?.Line}");

            if (actualSpacing?.Before != expectedSpacing?.Before)
                issues.Add($"Неверный отступ перед: {actualSpacing?.Before ?? "не задан"}, должен быть {expectedSpacing?.Before}");

            if (actualSpacing?.After != expectedSpacing?.After)
                issues.Add($"Неверный отступ после: {actualSpacing?.After ?? "не задан"}, должен быть {expectedSpacing?.After}");

            var actualIndentation = actual?.Indentation ?? style?.Indentation;
            var expectedIndentation = expected.Indentation;

            if (actualIndentation?.Left != expectedIndentation?.Left)
                issues.Add($"Неверный отступ слева: {actualIndentation?.Left ?? "не задан"}, должен быть {expectedIndentation?.Left}");

            if (actualIndentation?.Right != expectedIndentation?.Right)
                issues.Add($"Неверный отступ справа: {actualIndentation?.Right ?? "не задан"}, должен быть {expectedIndentation?.Right}");

            if (actualIndentation?.FirstLine != expectedIndentation?.FirstLine)
                issues.Add($"Неверный отступ первой строки: {actualIndentation?.FirstLine ?? "не задан"}, должен быть {expectedIndentation?.FirstLine}");

            if (actual?.KeepNext?.Val != expected.KeepNext?.Val)
                issues.Add("Некорректный параметр KeepWithNext");

            if (actual?.PageBreakBefore != null != (expected.PageBreakBefore != null))
                issues.Add("Неверный разрыв страницы перед параграфом");
        }
    }
}
