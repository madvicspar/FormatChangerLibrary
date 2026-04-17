using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Strategies
{
    public class HeadingFirstCorrectionStrategy : ElementCorrectionStrategyBase<HeadingSettingsModel>
    {
        public override HeadingSettingsModel GetSettings(FormattingTemplateModel template) =>
            template.HeadingSettings;

        public override void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var stylePart = doc.MainDocumentPart?.StyleDefinitionsPart;
            if (stylePart?.Styles == null)
                return;

            var numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart
                ?? doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>();
            EnsureNumbering(numberingPart);
            ApplyRecursiveStyleCorrection(stylePart.Styles, settings, level: 1);
        }

        public override List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
        {
            var issues = new List<string>();
            var settings = GetSettings(template);

            var runProps = CreateRunProperties(settings.TextSettings);
            var paraProps = CreateHeadingParagraphProperties(settings);

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

            CompareRunProperties(paragraph, actualRunProps, runProps, styles, issues);
            CompareParagraphProperties(paragraph, actualParaProps, paraProps, styles, issues);

			var text = paragraph.InnerText?.Trim();

			if (!HasNumbering(text))
			{
				issues.Add("Отсутствует нумерация заголовка");
			}

			return issues;
        }

        private void ApplyRecursiveStyleCorrection(Styles styles, HeadingSettingsModel settings, int level)
        {
            var styleName = $"heading {level}";
            var style = styles.Elements<Style>().FirstOrDefault(s => s.StyleName?.Val == styleName);
            if (style == null)
                return;

            var runProps = CreateRunProperties(settings.TextSettings);
            var paraProps = CreateHeadingParagraphProperties(settings);

            ApplyToStyle(styles, styleName, runProps, paraProps);

            if (settings.NextHeadingLevel != null)
            {
                ApplyRecursiveStyleCorrection(styles, settings.NextHeadingLevel, level + 1);
            }
        }

        private ParagraphProperties CreateHeadingParagraphProperties(HeadingSettingsModel settings)
        {
            var paraProps = CreateParagraphProperties(settings.TextSettings);

            if (settings.StartOnNewPage)
                paraProps.AddChild(new PageBreakBefore());

            var numbering = new NumberingProperties(
                new NumberingLevelReference { Val = settings.HeadingLevel - 1 },
                new NumberingId { Val = 1008 }
            );
            paraProps.Append(numbering);
            return paraProps;
        }

        private void EnsureNumbering(NumberingDefinitionsPart numberingPart)
        {
            var numbering = numberingPart.Numbering ?? new Numbering();

            //TODO: брать нормальный номер

            var abstractNum = new AbstractNum { AbstractNumberId = 1007 };
            abstractNum.Append(new MultiLevelType() { Val = MultiLevelValues.HybridMultilevel });

            abstractNum.Append(CreateHeadingLevel(0, "%1"));
            abstractNum.Append(CreateHeadingLevel(1, "%1.%2"));
            abstractNum.Append(CreateHeadingLevel(2, "%1.%2.%3"));

            numbering.Append(abstractNum);
            numbering.Append(new NumberingInstance(new AbstractNumId { Val = 1007 }) { NumberID = 1008 });
            numberingPart.Numbering.Save();
        }

        private Level CreateHeadingLevel(int index, string levelText)
        {
            var level = new Level(
                new TemplateCode { Val = levelText },
                new NumberingFormat() { Val = NumberFormatValues.Decimal },
                new LevelText() { Val = levelText },
                new LevelJustification() { Val = LevelJustificationValues.Left },
                new ParagraphProperties(
                    new Indentation()
                    {
                        Left = (720 * (index + 1)).ToString(), // 720 = 0.5 inch
                        Hanging = "360"
                    })
            )
            {
                LevelIndex = index,
                StartNumberingValue = new StartNumberingValue() { Val = 1 }
            };

            return level;
        }

        private void CompareRunProperties(Paragraph p, RunProperties actual, RunProperties expected, IEnumerable<Style> styles, List<string> issues)
        {
            var actualFont = actual?.RunFonts?.Ascii?.Value ?? GetPropertyValue(p, styles, rp => rp?.RunFonts?.Ascii?.Value, srp => srp?.RunFonts?.Ascii?.Value);
            var actualFontSize = actual?.FontSize?.Val ?? GetPropertyValue(p, styles, rp => rp?.FontSize?.Val, srp => srp?.FontSize?.Val);
            var actualColor = actual?.Color?.Val ?? GetPropertyValue(p, styles, rp => rp?.Color?.Val, srp => srp?.Color?.Val);
            var actualBold = actual?.Bold ?? GetBold(p, styles);
            var actualItalic = actual?.Italic ?? GetItalic(p, styles);

            var expectedFontSize = (int.Parse(expected.FontSize?.Val) / 2).ToString();
            actualFontSize = (int.Parse(actualFontSize) / 2).ToString();


            CompareProperty("Шрифт", actualFont, expected.RunFonts?.Ascii?.Value, issues);
            CompareProperty("Размер шрифта", actualFontSize, expectedFontSize, issues);
            CompareProperty("Цвет текста", actualColor, expected.Color?.Val, issues);
            CompareBoldAndItalic(actualBold, expected.Bold, actualItalic, expected.Italic, issues);
        }

        private void CompareProperty(string propertyName, string actualValue, string expectedValue, List<string> issues)
        {
            if (actualValue != expectedValue)
            {
                if (expectedValue == "0" && actualValue == null)
                    return;
                issues.Add($"{propertyName}: {actualValue ?? "не задан"}, должен быть {expectedValue}");
            }
        }

        private void CompareBoldAndItalic(Bold? actualBold, Bold? expectedBold, Italic? actualItalic, Italic? expectedItalic, List<string> issues)
        {
            if (actualBold.Val != expectedBold.Val)
            {
                issues.Add(expectedBold.Val ? "Должен быть полужирным" : "Не должен быть полужирным");
            }

            if (actualItalic != expectedItalic)
            {
                issues.Add(expectedItalic.Val ? "Должен быть курсивом" : "Не должен быть курсивом");
            }
        }

        private T GetPropertyValue<T>(
            Paragraph p,
            IEnumerable<Style> styles,
            Func<RunProperties, T> runPropSelector,
            Func<StyleRunProperties, T> stylePropSelector)
            where T : class
        {
            var actual = runPropSelector(p.Descendants<RunProperties>().FirstOrDefault());
            if (actual != null)
                return actual;

            var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;
            if (string.IsNullOrEmpty(styleId)) return null;

            // Ищем стиль по ID
            var style = styles.FirstOrDefault(s => s.StyleId == styleId);
            if (style != null)
            {
                var fromStyle = stylePropSelector(style.StyleRunProperties);
                if (fromStyle != null)
                    return fromStyle;
            }

            // Если стиль не найден, проверяем на основе родительского стиля
            styleId = styles.FirstOrDefault(s => s.StyleId == styleId)?.BasedOn?.Val;
            while (!string.IsNullOrEmpty(styleId))
            {
                var parentStyle = styles.FirstOrDefault(s => s.StyleId == styleId);
                if (parentStyle == null) break;

                var parentStyleProp = stylePropSelector(parentStyle.StyleRunProperties);
                if (parentStyleProp != null)
                    return parentStyleProp;

                styleId = parentStyle.BasedOn?.Val;
            }

            return null;
        }

        private T GetPropertyValue<T>(
            Paragraph p,
            IEnumerable<Style> styles,
            Func<ParagraphProperties, T> runPropSelector,
            Func<StyleParagraphProperties, T> stylePropSelector)
            where T : class
        {
            var actual = runPropSelector(p.Descendants<ParagraphProperties>().FirstOrDefault());
            if (actual != null)
                return actual;

            var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;
            if (string.IsNullOrEmpty(styleId)) return null;

            // Ищем стиль по ID
            var style = styles.FirstOrDefault(s => s.StyleId == styleId);
            if (style != null)
            {
                var fromStyle = stylePropSelector(style.StyleParagraphProperties);
                if (fromStyle != null)
                    return fromStyle;
            }

            // Если стиль не найден, проверяем на основе родительского стиля
            styleId = styles.FirstOrDefault(s => s.StyleId == styleId)?.BasedOn?.Val;
            while (!string.IsNullOrEmpty(styleId))
            {
                var parentStyle = styles.FirstOrDefault(s => s.StyleId == styleId);
                if (parentStyle == null) break;

                var parentStyleProp = stylePropSelector(parentStyle.StyleParagraphProperties);
                if (parentStyleProp != null)
                    return parentStyleProp;

                styleId = parentStyle.BasedOn?.Val;
            }

            return null;
        }

        private void CompareParagraphProperties(Paragraph p, ParagraphProperties actual, ParagraphProperties expected, IEnumerable<Style> styles, List<string> issues)
        {
            // Функция для извлечения и преобразования значений
            string GetSpacingValue(string value, double denominator)
            {
                if (double.TryParse(value, out double parsedValue))
                {
                    return Math.Round(parsedValue / denominator, 2).ToString();
                }
                return null;
            }

            // Получение значений и преобразование для межстрочного интервала и отступов
            var actualSpacingLine = GetSpacingValue(actual?.SpacingBetweenLines?.Line ?? GetPropertyValue(p, styles, rp => rp?.SpacingBetweenLines?.Line, srp => srp?.SpacingBetweenLines?.Line), 240);
            var expectedSpacingLine = GetSpacingValue(expected.SpacingBetweenLines.Line, 240);
            var actualSpacingBefore = GetSpacingValue(actual?.SpacingBetweenLines?.Before?.Value ?? GetPropertyValue(p, styles, rp => rp?.SpacingBetweenLines?.Before?.Value, srp => srp?.SpacingBetweenLines?.Before?.Value), 20);
            var expectedSpacingBefore = GetSpacingValue(expected.SpacingBetweenLines.Before?.Value, 20);
            var actualSpacingAfter = GetSpacingValue(actual?.SpacingBetweenLines?.After?.Value ?? GetPropertyValue(p, styles, rp => rp?.SpacingBetweenLines?.After?.Value, srp => srp?.SpacingBetweenLines?.After?.Value), 20);
            var expectedSpacingAfter = GetSpacingValue(expected.SpacingBetweenLines.After?.Value, 20);
            var actualIndentationFirstLine = GetSpacingValue(actual?.Indentation?.FirstLine?.Value ?? GetPropertyValue(p, styles, rp => rp?.Indentation?.FirstLine?.Value, srp => srp?.Indentation?.FirstLine?.Value), 567);
            var expectedIndentationFirstLine = GetSpacingValue(expected.Indentation?.FirstLine?.Value, 567);
            var actualIndentationLeft = GetSpacingValue(actual?.Indentation?.Left?.Value ?? GetPropertyValue(p, styles, rp => rp?.Indentation?.Left?.Value, srp => srp?.Indentation?.Left?.Value), 567);
            var expectedIndentationLeft = GetSpacingValue(expected.Indentation?.Left?.Value, 567);
            var actualIndentationRight = GetSpacingValue(actual?.Indentation?.Right?.Value ?? GetPropertyValue(p, styles, rp => rp?.Indentation?.Right?.Value, srp => srp?.Indentation?.Right?.Value), 567);
            var expectedIndentationRight = GetSpacingValue(expected.Indentation?.Right?.Value, 567);

            CompareProperty("Междустрочный интервал", actualSpacingLine, expectedSpacingLine, issues);
            CompareProperty("Интервал перед", actualSpacingBefore, expectedSpacingBefore, issues);
            CompareProperty("Интервал после", actualSpacingAfter, expectedSpacingAfter, issues);
            CompareProperty("Отступ первой строки", actualIndentationFirstLine, expectedIndentationFirstLine, issues);
            CompareProperty("Отступ слева", actualIndentationLeft, expectedIndentationLeft, issues);
            CompareProperty("Отступ справа", actualIndentationRight, expectedIndentationRight, issues);

            //if (actual?.KeepNext?.Val != expected.KeepNext?.Val)
            //    issues.Add("Некорректный параметр KeepWithNext");

            //if (actual?.PageBreakBefore != null != (expected.PageBreakBefore != null))
            //    issues.Add("Неверный разрыв страницы перед параграфом");
        }

        Bold GetBold(Paragraph p, IEnumerable<Style> styles)
        {
            // если styleId = null, то проверяем стиль обычный
            var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;
            if (styleId == null)
                return new Bold() { Val = false };

            Style style = null;

            do
            {
                if (style == null) // Выход, если стиль не найден
                    return new Bold() { Val = false };
                style = styles.First(s => s.StyleId == styleId);
                styleId = style?.BasedOn?.Val;
            } while (style.StyleRunProperties?.Bold?.Val == null);

            return style?.StyleRunProperties?.Bold;
        }

        Italic GetItalic(Paragraph p, IEnumerable<Style> styles)
        {
            // если styleId = null, то проверяем стиль обычный
            var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;
            if (styleId == null)
                return new Italic() { Val = false };

            Style style = null;

            do
            {
                if (style == null) // Выход, если стиль не найден
                    return new Italic() { Val = false };
                style = styles.First(s => s.StyleId == styleId);
                styleId = style?.BasedOn?.Val;
            } while (style.StyleRunProperties?.Italic?.Val == null);

            return style?.StyleRunProperties?.Italic;
        }

		private bool HasNumbering(string text)
		{
            // TODO: сделать нормальную проверку нумерации со всеми ее сложностями
			if (string.IsNullOrWhiteSpace(text))
				return false;

			text = text.Trim();

			if (char.IsDigit(text[0]))
				return true;

			if (text.StartsWith("Глава ", StringComparison.OrdinalIgnoreCase))
				return true;

			return false;
		}
	}
}