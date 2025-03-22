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

            CompareRunProperties(actualRunProps, expectedRunProps, issues);
            CompareParagraphProperties(actualParaProps, expectedParaProps, issues);

            return issues;
        }

        private void CompareRunProperties(RunProperties actual, RunProperties expected, List<string> issues)
        {
            if (actual == null)
            {
                issues.Add("Не получилось получить настройки");
                return;
            }

            if (actual.RunFonts?.Ascii?.Value != expected.RunFonts?.Ascii?.Value)
                issues.Add($"Неверный шрифт: {actual.RunFonts?.Ascii?.Value}, должен быть {expected.RunFonts?.Ascii?.Value}");

            if (actual.Color?.Val != expected.Color?.Val)
                issues.Add($"Неверный цвет: {actual.Color?.Val}, должен быть {expected.Color?.Val}");

            if (actual.FontSize?.Val != expected.FontSize?.Val)
                issues.Add($"Неверный размер шрифта: {actual.FontSize?.Val}, должен быть {expected.FontSize?.Val}");

            bool isBold = actual.Bold != null;
            bool shouldBeBold = expected.Bold?.Val == true;
            if (isBold != shouldBeBold)
                issues.Add(shouldBeBold ? "Должен быть полужирным" : "Не должен быть полужирным");

            bool isItalic = actual.Italic != null;
            bool shouldBeItalic = expected.Italic?.Val == true;
            if (isItalic != shouldBeItalic)
                issues.Add(shouldBeItalic ? "Должен быть курсивным" : "Не должен быть курсивным");

            bool isUnderlined = actual.Underline != null && actual.Underline?.Val != UnderlineValues.None;
            bool shouldBeUnderlined = expected.Underline?.Val == UnderlineValues.Single;
            if (isUnderlined != shouldBeUnderlined)
                issues.Add(shouldBeUnderlined ? "Должен быть подчеркнут" : "Не должен быть подчеркнут");
        }

        private void CompareParagraphProperties(ParagraphProperties actual, ParagraphProperties expected, List<string> issues)
        {
            //var styleParaProps = actual.Elements<Style>().FirstOrDefault().Elements<StyleParagraphProperties>().FirstOrDefault();
            var expectedSpacing = expected.SpacingBetweenLines;
            if (actual == null)
            {
                //if (styleParaProps.Justification?.Val != expected.Justification?.Val)
                //    issues.Add($"Неверное выравнивание: {styleParaProps.Justification?.Val}, должно быть {expected.Justification?.Val}");

                //var actualSpacingStyle = styleParaProps.SpacingBetweenLines;
                //if (actualSpacingStyle?.Line != expectedSpacing?.Line)
                //    issues.Add($"Неверный межстрочный интервал: {actualSpacingStyle?.Line}, должен быть {expectedSpacing?.Line}");

                //if (actualSpacingStyle?.Before != expectedSpacing?.Before)
                //    issues.Add($"Неверный отступ перед: {actualSpacingStyle?.Before}, должен быть {expectedSpacing?.Before}");

                //if (actualSpacingStyle?.After != expectedSpacing?.After)
                //    issues.Add($"Неверный отступ после: {actualSpacingStyle?.After}, должен быть {expectedSpacing?.After}");

                //if (styleParaProps?.Indentation?.Left != expected?.Indentation?.Left)
                //    issues.Add($"Неверный отступ слева: {styleParaProps?.Indentation?.Left}, должен быть {expected?.Indentation?.Left}");

                //if (styleParaProps?.Indentation?.Right != expected?.Indentation?.Right)
                //    issues.Add($"Неверный отступ справа: {styleParaProps?.Indentation?.Right}, должен быть {expected?.Indentation?.Right}");

                //if (styleParaProps?.Indentation?.FirstLine != expected?.Indentation?.FirstLine)
                //    issues.Add($"Неверный отступ первой строки: {styleParaProps?.Indentation?.FirstLine}, должен быть {expected?.Indentation?.FirstLine}");

                //return;
            }

            //if (actual.Indentation != null)
            //{
            //    if (actual.Justification.Val != expected.Justification?.Val)
            //        issues.Add($"Неверное выравнивание: {actual.Justification.Val}, должно быть {expected.Justification?.Val}");
            //}
            //else
            //{
            //    if (styleParaProps?.Justification?.Val != expected.Justification?.Val)
            //        issues.Add($"Неверное выравнивание: {styleParaProps?.Justification?.Val}, должно быть {expected.Justification?.Val}");
            //}

            //if (actual.SpacingBetweenLines != null)
            //{
            //    var actualSpacing = actual.SpacingBetweenLines;
            //    if (actualSpacing.Line != null)
            //    {
            //        if (actualSpacing?.Line != expectedSpacing?.Line)
            //            issues.Add($"Неверный межстрочный интервал: {actualSpacing?.Line}, должен быть {expectedSpacing?.Line}");
            //    }
            //    else
            //    {
            //        if (styleParaProps.SpacingBetweenLines?.Line != expectedSpacing?.Line)
            //            issues.Add($"Неверный межстрочный интервал: {styleParaProps.SpacingBetweenLines?.Line}, должен быть {expectedSpacing?.Line}");
            //    }
            //}
            //else
            //{
            //    var actualSpacingStyle = styleParaProps.SpacingBetweenLines;
            //    if (actualSpacingStyle.Line != null)
            //    {
            //        if (actualSpacingStyle?.Line != expectedSpacing?.Line)
            //            issues.Add($"Неверный межстрочный интервал: {actualSpacingStyle?.Line}, должен быть {expectedSpacing?.Line}");
            //    }
            //}

            var actualSpacing = actual.SpacingBetweenLines;
            if (actualSpacing?.Line != expectedSpacing?.Line)
                issues.Add($"Неверный межстрочный интервал: {actualSpacing?.Line}, должен быть {expectedSpacing?.Line}");

            if (actualSpacing?.Before != expectedSpacing?.Before)
                issues.Add($"Неверный отступ перед: {actualSpacing?.Before}, должен быть {expectedSpacing?.Before}");

            if (actualSpacing?.After != expectedSpacing?.After)
                issues.Add($"Неверный отступ после: {actualSpacing?.After}, должен быть {expectedSpacing?.After}");

            if (actual?.Indentation?.Left != expected?.Indentation?.Left)
                issues.Add($"Неверный отступ слева: {actual?.Indentation?.Left}, должен быть {expected?.Indentation?.Left}");

            if (actual?.Indentation?.Right != expected?.Indentation?.Right)
                issues.Add($"Неверный отступ справа: {actual?.Indentation?.Right}, должен быть {expected?.Indentation?.Right}");

            if (actual?.Indentation?.FirstLine != expected?.Indentation?.FirstLine)
                issues.Add($"Неверный отступ первой строки: {actual?.Indentation?.FirstLine}, должен быть {expected?.Indentation?.FirstLine}");
        }
    }
}
