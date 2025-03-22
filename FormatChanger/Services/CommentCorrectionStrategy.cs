using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public class CommentCorrectionStrategy : IElementCorrectionStrategy<CommentSettingsModel>
    {
        // TODO: какие-то проблемы с форматированием, скорее всего удалить потом можно
        public CommentSettingsModel GetSettings(FormattingTemplateModel template)
        {
            var textSettings = template.TextSettings;
            return new CommentSettingsModel()
            {
                Font = textSettings.Font,
                Color = textSettings.Color,
                IsBold = textSettings.IsBold,
                IsItalic = textSettings.IsItalic,
                IsUnderscore = textSettings.IsUnderscore,
                FontSize = textSettings.FontSize,
                LineSpacing = textSettings.LineSpacing,
                BeforeSpacing = textSettings.BeforeSpacing,
                AfterSpacing = textSettings.AfterSpacing,
                Justification = textSettings.Justification,
                Left = textSettings.Left,
                Right = textSettings.Right,
                FirstLine = textSettings.FirstLine,
                KeepWithNext = textSettings.KeepWithNext
            };
        }
        public RunProperties GetRunProperties(CommentSettingsModel settings)
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

        public ParagraphProperties GetParagraphProperties(CommentSettingsModel settings)
        {
            return new ParagraphProperties(
                new Justification { Val = JustificationValues.Left}
            );
        }

        public void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var commentsPart = doc.MainDocumentPart.GetPartsOfType<WordprocessingCommentsPart>().FirstOrDefault();
            if (commentsPart == null)
            {
                return;
            }

            var comments = commentsPart.Comments;

            foreach (var comment in comments)
            {
                foreach (var paragraph in comment.Elements<Paragraph>())
                {
                    var firstRun = paragraph.Elements<Run>().FirstOrDefault();
                    if (firstRun != null)
                    {
                        firstRun.RunProperties?.RemoveAllChildren();
                        firstRun.AppendChild(GetRunProperties(settings));
                    }

                    // Применяем стили ParagraphProperties
                    paragraph.ParagraphProperties?.RemoveAllChildren();
                    paragraph.AppendChild(GetParagraphProperties(settings));
                }
            }
        }

        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }
    }
}
