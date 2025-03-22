using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;
using FormatChanger.Utilities.Data;

namespace FormatChanger.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IElementCorrectionStrategy<TextSettingsModel> _textCorrectionStrategy;
        private readonly IElementCorrectionStrategy<HeadingSettingsModel> _headingFirstCorrectionStrategies;
        private readonly IElementCorrectionStrategy<ImageSettingsModel> _imageCorrectionStrategy;
        private readonly IElementCorrectionStrategy<ImageCaptionSettingsModel> _imageCaptionCorrectionStrategy;
        private readonly IElementCorrectionStrategy<TableSettingsModel> _tableCorrectionStrategy;
        private readonly IElementCorrectionStrategy<CellSettingsModel> _cellCorrectionStrategy;
        private readonly IElementCorrectionStrategy<HeaderSettingsModel> _headerTableCorrectionStrategies;
        private readonly IElementCorrectionStrategy<TableCaptionSettingsModel> _tableCaptionCorrectionStrategy;

        public DocumentService(ApplicationDbContext context,
            IElementCorrectionStrategy<TextSettingsModel> textStrategy,
            IElementCorrectionStrategy<HeadingSettingsModel> h1Strategy,
            IElementCorrectionStrategy<ImageSettingsModel> imageStrategy,
            IElementCorrectionStrategy<TableSettingsModel> tableCorrectionStrategy,
            IElementCorrectionStrategy<CellSettingsModel> cellCorrectionStrategy,
            IElementCorrectionStrategy<HeaderSettingsModel> headerTableCorrectionStrategies,
            IElementCorrectionStrategy<ImageCaptionSettingsModel> imageCaptionCorrectionStrategy,
            IElementCorrectionStrategy<TableCaptionSettingsModel> tableCaptionCorrectionStrategy)
        {
            _context = context;
            //_context.ClearAndSeed(_context);
            _textCorrectionStrategy = textStrategy;
            _headingFirstCorrectionStrategies = h1Strategy;
            _imageCorrectionStrategy = imageStrategy;
            _tableCorrectionStrategy = tableCorrectionStrategy;
            _cellCorrectionStrategy = cellCorrectionStrategy;
            _headerTableCorrectionStrategies = headerTableCorrectionStrategies;
            _imageCaptionCorrectionStrategy = imageCaptionCorrectionStrategy;
            _tableCaptionCorrectionStrategy = tableCaptionCorrectionStrategy;

            //_context.SeedData(_context);
        }

        // Загрузка документа и сохранение в БД
        public async Task<DocumentModel> UploadDocumentAsync(IFormFile file)
        {
            var users = _context.Users.First();
            var document = new DocumentModel
            {
                FilePath = Path.Combine("uploads", file.FileName),
                IsOriginal = true,
                UserId = _context.Users.First().Id.ToString()
            };

            // Сохраняем файл на сервер
            using (var stream = new FileStream(document.FilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return document;
        }

        // Получаем документ по его ID
        public async Task<DocumentModel> GetDocumentByIdAsync(long id)
        {
            return await _context.Documents.FindAsync(id);
        }

        // Извлечение абзацев из документа
        public List<ParagraphModel>? GetDocumentParagraphs(DocumentModel document)
        {
            // TODO: убрать рисунки и таблицы (хотя мб не убирать, а писать что вот тут фотка или изображение
            using (WordprocessingDocument doc = WordprocessingDocument.Open(document.FilePath, true))
            {
                return doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().Where(p => !string.IsNullOrWhiteSpace(p.InnerText))
                    .Select(p => new ParagraphModel
                    {
                        Paragraph = p,
                        Type = ParagraphTypes.Normal.ToString()
                    })
                    .ToList();
            }
        }

        public void AddPageNumbers(WordprocessingDocument doc)
        {
            // TODO: привязать к обычному тексту?
            FooterPart footerPart = doc.MainDocumentPart.AddNewPart<FooterPart>();
            string footerPartId = doc.MainDocumentPart.GetIdOfPart(footerPart);

            Footer footer = new Footer(new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Normal" },
                    new Justification() { Val = JustificationValues.Center },
                    new SpacingBetweenLines() { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                    new Indentation { Left = "0", Right = "0", FirstLine = "0" }
                ),
                new Run(
                    new SimpleField() { Instruction = "PAGE" })));

            footerPart.Footer = footer;

            IEnumerable<SectionProperties> sectionProperties = doc.MainDocumentPart.Document.Body.Elements<SectionProperties>();

            foreach (var sectionProperty in sectionProperties)
            {
                sectionProperty.RemoveAllChildren<FooterReference>();
                sectionProperty.PrependChild(new FooterReference()
                {
                    Id = footerPartId
                });
            }
        }

        public void EnsureStylesExists(Styles styles, ParagraphTypes type)
        {
            // Проверяем, существует ли стиль
            if (styles.Elements<Style>().All(s => s.StyleName.Val != type.ToString()))
            {
                Style style = new Style()
                {
                    Type = StyleValues.Paragraph,
                    StyleId = type.ToString(),
                    CustomStyle = true,
                    StyleName = new StyleName() { Val = type.ToString() }
                };

                style.Append(new BasedOn() { Val = "Normal" });
                style.Append(new NextParagraphStyle() { Val = "Normal" });

                styles.Append(style);
            }
        }

        public async Task<DocumentModel> CorrectDocumentAsync(DocumentModel document, FormattingTemplateModel template, string[] types)
        {
            var paragraphList = GetDocumentParagraphs(document);
            for (int i = 0; i < types.Length; i++)
            {
                paragraphList[i].Type = ParagraphTypesEnumExtensions.ToEnum(types[i]).ToString();
            }

            using (WordprocessingDocument doc = WordprocessingDocument.Open(document.FilePath, true))
            {
                AddPageNumbers(doc);

                var styles = doc.MainDocumentPart.StyleDefinitionsPart.Styles;

                EnsureStylesExists(styles, ParagraphTypes.ImageCaption);
                EnsureStylesExists(styles, ParagraphTypes.TableCaption);

                styles.Save();

                var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().Where(p => !string.IsNullOrWhiteSpace(p.InnerText)).ToList();
                ApplyStyle(styles, paragraphs, paragraphList);
                
                doc.Save();
            }

            CleanFormat(document.FilePath);
            using (WordprocessingDocument doc = WordprocessingDocument.Open(document.FilePath, true))
            {
                _textCorrectionStrategy.ApplyCorrection(doc, template);
                _headingFirstCorrectionStrategies.ApplyCorrection(doc, template);
                _imageCorrectionStrategy.ApplyCorrection(doc, template);
                _imageCaptionCorrectionStrategy.ApplyCorrection(doc, template);
                _tableCorrectionStrategy.ApplyCorrection(doc, template);
                _cellCorrectionStrategy.ApplyCorrection(doc, template);
                _headerTableCorrectionStrategies.ApplyCorrection(doc, template);
                _tableCaptionCorrectionStrategy.ApplyCorrection(doc, template);

                doc.Save();
            }
            // TODO: достать исправленный документ
            return document;
        }

        public void ApplyStyle(Styles styles, List<Paragraph> paragraphs, List<ParagraphModel> paragraphList)
        {
            var stack = new Stack<int>();
            for (int i = 0; i < paragraphs.Count; i++)
            {
                var paragraph = paragraphs[i];
                var type = paragraphList.Where(x => x.Paragraph.ParagraphId == paragraph.ParagraphId).First().Type;

                ParagraphProperties paraProps = paragraph.Elements<ParagraphProperties>().FirstOrDefault();

                if (paraProps == null)
                {
                    paraProps = new ParagraphProperties();
                    paragraph.PrependChild(paraProps);
                }

                if (type == ParagraphTypes.FirstH.ToString())
                {
                    type = "heading 1";
                }
                else if (type == ParagraphTypes.SecondH.ToString())
                {
                    type = "heading 2";
                }
                else if (type == ParagraphTypes.ThirdH.ToString())
                {
                    type = "heading 3";
                }

                if (IsList(type))
                {
                    int level = DetermineListLevel(paragraphList, stack, type, paragraph);
                    ApplyNumbering(paraProps, level, type);
                }
                else
                {
                    var styleId = styles.Elements<Style>().Where(x => x.StyleName.Val == type).First().StyleId;

                    paraProps.ParagraphStyleId = new ParagraphStyleId() { Val = styleId };
                }
            }
        }

        private int DetermineListLevel(List<ParagraphModel> paragraphList, Stack<int> stack, string type, Paragraph paragraph)
        {
            int level = 0;

            if (stack.Count > 0)
            {
                var index = paragraphList.FindIndex(p => p.Paragraph.ParagraphId == paragraph.ParagraphId);

                var previousParagraph = index > 0 ? paragraphList[index - 1] : null; ;
                if (previousParagraph != null && IsList(previousParagraph.Type))
                {
                    if (previousParagraph.Type != type)
                    {
                        level = stack.Count > 0 ? stack.Pop() + 1 : 0;
                    }
                    else
                    {
                        level = stack.Count > 0 ? stack.Peek() : 0;
                    }
                }
                else
                {
                    level = 0;
                }
            }
            else
            {
                level = 0;
            }

            stack.Push(level);
            return level;
        }

        private void ApplyNumbering(ParagraphProperties paraProps, int level, string type)
        {
            NumberingId numberingId = new NumberingId() { Val = level == 0 ? 1 : 2 };
            NumberingProperties numberingProperties = new NumberingProperties(
                new NumberingLevelReference() { Val = level },
                numberingId
            );

            paraProps.Append(numberingProperties);
        }

        public bool IsList(string type)
        {
            return type == ParagraphTypes.Period.ToString() || type == ParagraphTypes.Bracket.ToString() || type == ParagraphTypes.Dash.ToString();
        }

        public async Task<DocumentModel> CheckDocumentAsync(DocumentModel document, FormattingTemplateModel template, string[] types)
        {
            var paragraphList = GetDocumentParagraphs(document);
            for (int i = 0; i < types.Length; i++)
            {
                paragraphList[i].Type = ParagraphTypesEnumExtensions.ToEnum(types[i]).ToString();
        }

            using (WordprocessingDocument doc = WordprocessingDocument.Open(document.FilePath, true))
            {

                var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().Where(p => !string.IsNullOrWhiteSpace(p.InnerText)).ToList();
                for (int i = 0; i < 1; i++)
                {
                    var paragraph = paragraphs[i];
                    //if (paragraphList[i].Type == ParagraphTypes.Heading.ToString())
                    //{
                    //    var issues = _headingFirstCorrectionStrategies.CheckFormatting(paragraph, template);

                    //    if (issues.Any())
                    //    {
                    //        AddCommentToParagraph(paragraph, issues);
                    //    }
                    //}
                }
                doc.Save();
            }
            return document;
        }

        public async Task<DocumentModel> EvaluateDocumentAsync(DocumentModel document, FormattingTemplateModel template, string[] types)
        {
            throw new NotImplementedException();
        }

        private void AddCommentToParagraph(Paragraph paragraph, List<string> commentText)
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

        public void CleanFormat(string filePath)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                foreach (var paragraph in body.Elements<Paragraph>())
                {
                    var paragraphProperties = paragraph.Elements<ParagraphProperties>().FirstOrDefault();
                    if (paragraphProperties != null)
                    {
                        var styleElement = paragraphProperties.Elements<ParagraphStyleId>().FirstOrDefault();
                        var numberingProperties = paragraphProperties.Elements<NumberingProperties>().FirstOrDefault();
                        paragraphProperties.RemoveAllChildren();

                        if (styleElement != null)
                        {
                            paragraphProperties.Append(styleElement);
                        }
                        if (numberingProperties != null)
                        {
                            paragraphProperties.Append(numberingProperties);
                        }
                    }

                    foreach (var run in paragraph.Elements<Run>())
                    {
                        var runProperties = run.Elements<RunProperties>().FirstOrDefault();
                        if (runProperties != null)
                        {
                            var styleElement = runProperties.Elements<RunStyle>().FirstOrDefault();
                            runProperties.RemoveAllChildren();
                            if (styleElement != null)
                            {
                                runProperties.Append(styleElement);
                            }
                        }
                    }
                }

                doc.Save();
            }
        }
    }
}