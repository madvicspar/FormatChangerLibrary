using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Models.Helpers;
using FormatChanger.Services.Interfaces;

namespace FormatChanger.Services
{
    public class DocumentChecker : IDocumentChecker
    {
		private readonly FormattingResolver _resolver;
		private readonly IElementCorrectionStrategy<HeadingSettingsModel> _headingStrategy;
		private readonly IElementCorrectionStrategy<ImageCaptionSettingsModel> _imageCaptionStrategy;
		private readonly IElementCorrectionStrategy<TableCaptionSettingsModel> _tableCaptionStrategy;
		private readonly IElementCorrectionStrategy<TextSettingsModel> _textStrategy;

        public DocumentChecker(IElementCorrectionStrategy<HeadingSettingsModel> headingStrategy, IElementCorrectionStrategy<ImageCaptionSettingsModel> imageCaptionStrategy, IElementCorrectionStrategy<TableCaptionSettingsModel> tableCaptionStrategy, IElementCorrectionStrategy<TextSettingsModel> textStrategy, FormattingResolver resolver)
        {
			_resolver = resolver;
			_headingStrategy = headingStrategy;
            _imageCaptionStrategy = imageCaptionStrategy;
            _tableCaptionStrategy = tableCaptionStrategy;
            _textStrategy = textStrategy;
        }

        public async Task CheckAndCommentAsync(WordprocessingDocument doc, FormattingTemplateModel template, List<ParagraphModel> paragraphList, string[] types)
        {
			foreach (var paragraphModel in paragraphList)
			{
                if (paragraphModel.Paragraph == null)
                    continue;

				var paragraph = paragraphModel.Paragraph;
				var issues = new List<string>();
				var resolved = _resolver.ResolveParagraph(paragraph);

				//if (paragraphModel.Paragraph == null)
    //                issues.Add("Как может не быть абзаца?"); // TODO: такая ситуация вообще мб?
                if (paragraphModel.Type == ParagraphTypes.FirstH.ToString())
                    issues = _headingStrategy.CheckFormatting(resolved, template);
                else if (paragraphModel.Type == ParagraphTypes.ImageCaption.ToString())
                    issues = _imageCaptionStrategy.CheckFormatting(resolved, template);
				else if (paragraphModel.Type == ParagraphTypes.TableCaption.ToString())
					issues = _tableCaptionStrategy.CheckFormatting(resolved, template);
				else
                    issues = _textStrategy.CheckFormatting(resolved, template);

				if (issues.Count != 0)
					AddComment(paragraphModel.Paragraph, issues);
			}
			doc.Save();
        }
        private static void AddComment(Paragraph paragraph, List<string> commentText)
        {
            // TODO: мэтчится со стилем обычного текста, что логично, но может быть неправильным (например, как в тестовом примере - красный текст)
            var mainPart = paragraph.Ancestors<Document>().First().MainDocumentPart;
            var commentsPart = mainPart.GetPartsOfType<WordprocessingCommentsPart>().FirstOrDefault();

            if (commentsPart == null)
            {
                commentsPart = mainPart.AddNewPart<WordprocessingCommentsPart>();
                commentsPart.Comments = new Comments();
            }

            var comments = commentsPart.Comments;

            int id = comments.Elements<Comment>().Count() + 1;
            string commentId = id.ToString();

            var comment = new Comment()
            {
                Id = commentId,
                Author = "Автоматическая проверка",
                Date = DateTime.Now
            };

            foreach (var line in commentText)
            {
                comment.Append(new Paragraph(new Run(new Text(line))));
            }

            comments.Append(comment);
            comments.Save();

            var commentRangeStart = new CommentRangeStart() { Id = commentId };
            var commentRangeEnd = new CommentRangeEnd() { Id = commentId };
            var commentReference = new CommentReference() { Id = commentId };

            var firstRun = paragraph.GetFirstChild<Run>();
            if (firstRun != null)
            {
                paragraph.InsertBefore(commentRangeStart, firstRun);
            }
            paragraph.Append(commentRangeEnd);
            paragraph.Append(new Run(commentReference));
        }
    }
}