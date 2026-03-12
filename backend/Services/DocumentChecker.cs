using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using FormatChanger.WebAPI.Models.FormattingModels;
using FormatChanger.WebAPI.Models.Helpers;
using FormatChanger.WebAPI.Services.Interfaces;

namespace FormatChanger.WebAPI.Services
{
	public class DocumentChecker : IDocumentChecker
	{
		private readonly IElementCorrectionStrategy<HeadingSettingsModel> _headingStrategy;

		public DocumentChecker(IElementCorrectionStrategy<HeadingSettingsModel> headingStrategy)
			=> _headingStrategy = headingStrategy;
		public async Task CheckAndCommentAsync(WordprocessingDocument doc, FormattingTemplateModel template, List<ParagraphModel> paragraphList, string[] types)
		{
			var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().Where(p => !string.IsNullOrWhiteSpace(p.InnerText)).ToList();
			for (int i = 0; i < 1; i++)
			{
				var paragraph = paragraphs[i];
				if (paragraphList[i].Type == ParagraphTypes.FirstH.ToString())
				{
					var issues = _headingStrategy.CheckFormatting(paragraph, template);

					if (issues.Any())
						AddComment(paragraph, issues);
				}
			}
			doc.Save();
		}
		private static void AddComment(Paragraph paragraph, List<string> commentText)
		{
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