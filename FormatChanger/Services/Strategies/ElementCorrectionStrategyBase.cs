using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Services.Interfaces;

namespace FormatChanger.Services.Strategies
{
    public abstract class ElementCorrectionStrategyBase<TSettings> : IElementCorrectionStrategy<TSettings>
    {
        public abstract TSettings GetSettings(FormattingTemplateModel template);

        public abstract void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template);

        public virtual List<string> CheckFormatting(ParagraphStyleProperties actual, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }

        protected RunProperties CreateRunProperties(TextSettingsModel settings)
        {
            return new RunProperties(
                new RunFonts { Ascii = settings.Font, HighAnsi = settings.Font },
                new Color { Val = settings.Color },
                new Bold { Val = settings.IsBold },
                new Italic { Val = settings.IsItalic },
                new Underline { Val = settings.IsUnderscore ? UnderlineValues.Single : UnderlineValues.None },
                new FontSize { Val = (settings.FontSize * 2).ToString() }
            );
        }

        protected ParagraphProperties CreateParagraphProperties(TextSettingsModel settings)
        {
            return new ParagraphProperties(
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
                    FirstLine = ((int)(settings.FirstLine * 567)).ToString()
                },
                new Justification { Val = JustificationConverter.Parse(settings.Justification) },
                new KeepNext { Val = settings.KeepWithNext }
            );
        }

        protected void ApplyToStyle(Styles styles, string styleName, RunProperties runProps, ParagraphProperties paraProps)
        {
            var style = styles.Elements<Style>().FirstOrDefault(s => s.StyleName?.Val == styleName);
            if (style == null) return;

            style.RemoveAllChildren<StyleRunProperties>();
            style.RemoveAllChildren<StyleParagraphProperties>();

            style.AppendChild(new StyleRunProperties(runProps));
            style.AppendChild(new StyleParagraphProperties(paraProps));
        }
    }
}
