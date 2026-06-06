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
		private readonly IElementCorrectionStrategy<ImageSettingsModel> _imageStrategy;
		private readonly IElementCorrectionStrategy<ImageCaptionSettingsModel> _imageCaptionStrategy;
		private readonly IElementCorrectionStrategy<TableCaptionSettingsModel> _tableCaptionStrategy;
		private readonly IElementCorrectionStrategy<TextSettingsModel> _textStrategy;

		public DocumentChecker(
			IElementCorrectionStrategy<HeadingSettingsModel> headingStrategy,
			IElementCorrectionStrategy<ImageSettingsModel> imageStrategy,
			IElementCorrectionStrategy<ImageCaptionSettingsModel> imageCaptionStrategy,
			IElementCorrectionStrategy<TableCaptionSettingsModel> tableCaptionStrategy,
			IElementCorrectionStrategy<TextSettingsModel> textStrategy,
			FormattingResolver resolver)
		{
			_resolver = resolver;
			_headingStrategy = headingStrategy;
			_imageStrategy = imageStrategy;
			_imageCaptionStrategy = imageCaptionStrategy;
			_tableCaptionStrategy = tableCaptionStrategy;
			_textStrategy = textStrategy;
		}

		public async Task CheckAndCommentAsync(WordprocessingDocument doc, FormattingTemplateModel template, List<ParagraphModel> paragraphList, string[] types)
		{
			for (int i = 0; i < paragraphList.Count; i++)
			{
				var paragraphModel = paragraphList[i];
				if (paragraphModel.Paragraph == null)
					continue;

				var paragraph = paragraphModel.Paragraph;
				var resolved = _resolver.ResolveParagraph(paragraph);
				List<string> issues;

				if (paragraphModel.Type == ParagraphTypes.FirstH.ToString())
					issues = _headingStrategy.CheckFormatting(resolved, template);
				else if (paragraphModel.Type == ParagraphTypes.ImageCaption.ToString())
				{
					issues = _imageCaptionStrategy.CheckFormatting(resolved, template);
					CheckCaptionPosition(paragraphList, i, template.ImageSettings?.CaptionSettings, CaptionPosition.Below, issues);
				}
				else if (paragraphModel.Type == ParagraphTypes.TableCaption.ToString())
				{
					issues = _tableCaptionStrategy.CheckFormatting(resolved, template);
					CheckCaptionPosition(paragraphList, i, template.TableSettings?.CaptionSettings, CaptionPosition.Above, issues);
				}
				else
					issues = _textStrategy.CheckFormatting(resolved, template);

				if (issues.Count != 0)
				{
					issues.Insert(0, $"Тип: {GetTypeName(paragraphModel.Type)}");
					AddComment(paragraphModel.Paragraph, issues);
				}
			}

			// Separate pass for paragraphs that contain drawings (excluded from paragraphList).
			var imageParagraphs = doc.MainDocumentPart?.Document?.Body?
				.Descendants<Paragraph>()
				.Where(p => p.Descendants<Drawing>().Any() && !p.Ancestors<TableCell>().Any())
				.ToList() ?? [];

			foreach (var imgPara in imageParagraphs)
			{
				var resolved = _resolver.ResolveParagraph(imgPara);
				var issues = _imageStrategy.CheckFormatting(resolved, template);
				if (issues.Count != 0)
				{
					issues.Insert(0, "Тип: Рисунок");
					AddComment(imgPara, issues);
				}
			}

			doc.Save();
		}

		// actualPosition — caption position derived from its location in the document (Above/Below)
		private static void CheckCaptionPosition(List<ParagraphModel> list, int index, ICaptionSettingsModel settings, CaptionPosition actualPosition, List<string> issues)
		{
			if (settings == null) return;
			if (settings.Position != actualPosition)
			{
				var expectedLabel = settings.Position == CaptionPosition.Above ? "над" : "под";
				var actualLabel = actualPosition == CaptionPosition.Above ? "над" : "под";
				issues.Add($"Положение подписи: фактически {actualLabel} элементом, должно быть {expectedLabel}");
			}
		}
		private static string GetTypeName(string type) => type switch
		{
			"FirstH" => "Заголовок первого уровня",
			"SecondH" => "Заголовок первого уровня",
			"ThirdH" => "Заголовок первого уровня",
			"ImageCaption" => "Подпись к рисунку",
			"TableCaption" => "Подпись к таблице",
			"Period" => "Элемент списка",
			"Bracket" => "Элемент списка",
			"Dash" => "Элемент списка",
			_ => "Обычный текст",
		};

		private static void AddComment(Paragraph paragraph, List<string> commentText)
		{
			// TODO: matches the normal text style, which is logical but may be incorrect in some cases (e.g. the red text in the test document)
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