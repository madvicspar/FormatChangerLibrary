using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;
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

		public override List<string> CheckFormatting(ParagraphStyleProperties actual, FormattingTemplateModel template)
		{
			var issues = new List<string>();
			var expected = GetSettings(template);

			if (actual.RunStyle.Bold != expected.TextSettings.IsBold)
				issues.Add(expected.TextSettings.IsBold
					? "Должен быть полужирным"
					: "Не должен быть полужирным");

			if (actual.RunStyle.Italic != expected.TextSettings.IsItalic)
				issues.Add(expected.TextSettings.IsItalic
					? "Должен быть курсивом"
					: "Не должен быть курсивом");

			if (!CompareNullable(actual.RunStyle.Color, expected.TextSettings.Color))
				issues.Add($"Цвет текста должен быть {expected.TextSettings.Color}");

			if (!CompareNullable(actual.RunStyle.FontSize, expected.TextSettings.FontSize.ToString()))
				issues.Add($"Размер шрифта должен быть {expected.TextSettings.FontSize}");

			CompareProperty("Междустрочный интервал", actual.SpacingLine, expected.TextSettings.LineSpacing.ToString(), issues);
			CompareProperty("Интервал перед", actual.SpacingBefore, expected.TextSettings.BeforeSpacing.ToString(), issues);
			CompareProperty("Интервал после", actual.SpacingAfter, expected.TextSettings.AfterSpacing.ToString(), issues);

			CompareProperty("Отступ первой строки", actual.IndentFirstLine, expected.TextSettings.FirstLine.ToString(), issues);
			CompareProperty("Отступ слева", actual.IndentLeft, expected.TextSettings.Left.ToString(), issues);
			CompareProperty("Отступ справа", actual.IndentRight, expected.TextSettings.Right.ToString(), issues);

			return issues;
		}

		private static void CompareProperty(string name, string actual, string expected, List<string> issues)
		{
			if (!CompareNullable(actual, expected))
			{
				if (expected == "0" && actual == null) return;

				issues.Add($"{name}: {actual ?? "не задан"}, должен быть {expected}");
			}
		}

		private static bool CompareNullable(string actual, string expected)
		{
			return (actual ?? "") == (expected ?? "");
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