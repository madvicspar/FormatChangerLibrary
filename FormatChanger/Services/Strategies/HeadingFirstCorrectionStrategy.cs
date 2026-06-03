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

        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
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