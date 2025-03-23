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
            using (WordprocessingDocument doc = WordprocessingDocument.Open(document.FilePath, true))
            {
                var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>()
                    .ToList();

                if (paragraphs == null) return null;

                var paragraphModels = new List<ParagraphModel>();

                var styles = doc.MainDocumentPart.StyleDefinitionsPart.Styles;
                for (int i = 0; i < paragraphs.Count; i++)
                {
                    var currentParagraph = paragraphs[i];
                    var paragraphModel = new ParagraphModel
                    {
                        Paragraph = currentParagraph,
                        Type = GetParagraphType(currentParagraph, paragraphs, i, styles)
                    };

                    paragraphModels.Add(paragraphModel);
                }

                return paragraphModels.Where(p => !string.IsNullOrEmpty(p.Paragraph.InnerText) && !p.Paragraph.Ancestors<TableCell>().Any()).ToList();
            }
        }


        private string GetParagraphType(Paragraph currentParagraph, List<Paragraph> paragraphs, int index, Styles styles)
        {
            var styleId = currentParagraph.ParagraphProperties?.ParagraphStyleId?.Val;
            var style = styles.Elements<Style>().Where(x => x.StyleId == styleId)?.FirstOrDefault();

            if (style != null)
            {
                // Проверка стиля абзаца
                if (style.StyleName.Val == "heading 1")
                {
                    return ParagraphTypes.FirstH.ToString(); // Заголовок первого уровня
                }
                if (style.StyleName.Val == "heading 2")
                {
                    return ParagraphTypes.SecondH.ToString(); // Заголовок второго уровня
                }
                if (style.StyleName.Val == "heading 3")
                {
                    return ParagraphTypes.ThirdH.ToString(); // Заголовок третьего уровня
                }
            }

            // Проверка таблицы после текущего абзаца
            if (index < paragraphs.Count - 1 && IsTable(paragraphs[index + 1]))
            {
                return ParagraphTypes.TableCaption.ToString(); // Подпись к таблице
            }

            // Проверка изображения перед текущим абзацем
            if (index > 0 && IsImage(paragraphs[index - 1]))
            {
                return ParagraphTypes.ImageCaption.ToString(); // Подпись к изображению
            }

            return ParagraphTypes.Normal.ToString(); // Обычный абзац
        }

        private bool IsImage(Paragraph paragraph)
        {
            // Проверка, является ли абзац изображением (например, на основе наличия элемента Drawing)
            return paragraph.Descendants<Drawing>().Any();
        }

        private bool IsTable(Paragraph paragraph)
        {
            // Проверка, является ли абзац частью таблицы (например, на основе наличия элемента Table)
            return paragraph.Ancestors<TableCell>().Any();
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

                var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().Where(p => !string.IsNullOrEmpty(p.InnerText) && !p.Ancestors<TableCell>().Any()).ToList();
                ApplyStyle(styles, paragraphs, paragraphList);

                doc.Save();

                CleanFormat(doc);

                _textCorrectionStrategy.ApplyCorrection(doc, template);
                _headingFirstCorrectionStrategies.ApplyCorrection(doc, template);
                _imageCorrectionStrategy.ApplyCorrection(doc, template);
                _imageCaptionCorrectionStrategy.ApplyCorrection(doc, template);
                _tableCorrectionStrategy.ApplyCorrection(doc, template);
                _cellCorrectionStrategy.ApplyCorrection(doc, template);
                _headerTableCorrectionStrategies.ApplyCorrection(doc, template);
                _tableCaptionCorrectionStrategy.ApplyCorrection(doc, template);

                AddNumberingToDocument(doc);

                Stack<int> levels = new Stack<int>();
                for (int i = 0; i < paragraphs.Count; i++)
                {
                    var paragraph = paragraphs[i];
                    var type = paragraphList.Where(x => x.Paragraph.ParagraphId == paragraph.ParagraphId).First().Type;

                    ParagraphProperties paraProps = paragraph.Elements<ParagraphProperties>().FirstOrDefault();

                    if (IsList(type))
                    {
                        int level = DetermineListLevel(paragraphList, levels, type, paragraph);
                        if (type == ParagraphTypes.Dash.ToString())
                        {
                            ApplyNumbering(paraProps, level, 1001);
                        }
                        else if (type == ParagraphTypes.Period.ToString())
                        {
                            ApplyNumbering(paraProps, level, 1002);
                        }
                        else if (type == ParagraphTypes.Bracket.ToString())
                        {
                            ApplyNumbering(paraProps, level, 1003); ;
                        }

                    }
                }

                doc.Save();
            }
            // TODO: достать исправленный документ
            return document;
        }

        public void ApplyStyle(Styles styles, List<Paragraph> paragraphs, List<ParagraphModel> paragraphList)
        {
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
                    type = "Normal";
                }

                var styleId = styles.Elements<Style>().Where(x => x.StyleName.Val == type).First().StyleId;

                paraProps.ParagraphStyleId = new ParagraphStyleId() { Val = styleId };
            }
        }

        private int DetermineListLevel(List<ParagraphModel> paragraphList, Stack<int> stack, string type, Paragraph paragraph)
        {
            int level = 0;

            if (stack.Count > 0)
            {
                var index = paragraphList.FindIndex(p => p.Paragraph.ParagraphId == paragraph.ParagraphId);

                var previousParagraph = index > 0 ? paragraphList[index - 1] : null;

                if (previousParagraph != null && IsList(previousParagraph.Type))
                {
                    if (previousParagraph.Type != type)
                    {
                        // Если маркер другой, то переходим на новый уровень
                        level = stack.Count > 0 ? stack.Peek() + 1 : 0;
                    }
                    else
                    {
                        // Если маркер тот же, уровень остается на прежнем уровне
                        level = stack.Peek();
                    }
                }
                else
                {
                    // Если предыдущий абзац не список, начинаем новый список с уровня 0
                    level = 0;
                }
            }
            else
            {
                // Если стек пуст, начинаем с уровня 0
                level = 0;
            }

            stack.Push(level); // Добавляем текущий уровень в стек
            return level;
        }

        private void ApplyNumbering(ParagraphProperties paraProps, int level, int numberingId)
        {
            NumberingProperties numberingProperties = new NumberingProperties(
                new NumberingLevelReference { Val = level }, // Уровень нумерации
                new NumberingId { Val = numberingId } // Идентификатор NumberingInstance
            );

            paraProps.Append(numberingProperties);

            // Получаем или создаем элемент Indentation
            Indentation indentation = paraProps.Elements<Indentation>().FirstOrDefault();

            // Если Indentation не существует, создаем его
            if (indentation == null)
            {
                indentation = new Indentation();
                paraProps.AppendChild(indentation); // Добавляем Indentation к ParagraphProperties
            }

            // Устанавливаем отступы
            // Слева: 1.5 * (level + 1)
            indentation.Left = ((int)((2 + 0.5 * level) * 567)).ToString(); // Отступ слева в EMU

            // Для первой строки: 0.5 EMU
            indentation.Hanging = ((int)(0.5 * 567)).ToString();

            //Indentation indentation = paraProps.Elements<Indentation>().FirstOrDefault();

            //if (indentation == null)
            //{
            //    indentation = new Indentation()
            //        { Left = (1.5).ToString() };

            //}
            //paraProps.AppendChild(indentation);
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

        public void CleanFormat(WordprocessingDocument doc)
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
        }

        public void AddNumberingToDocument(WordprocessingDocument doc)
        {
            // Получаем или создаем часть NumberingPart
            var numberingPart = doc.MainDocumentPart.NumberingDefinitionsPart
                ?? doc.MainDocumentPart.AddNewPart<NumberingDefinitionsPart>();

            if (numberingPart.Numbering == null)
            {
                numberingPart.Numbering = new Numbering();
            }

            var numbering = numberingPart.Numbering;

            // Создаем AbstractNum для маркированного списка
            var bulletAbstractNum = new AbstractNum()
            {
                AbstractNumberId = 1001 // Идентификатор AbstractNum
            };

            AppendLevel(bulletAbstractNum, 0, "-", NumberFormatValues.Bullet);
            AppendLevel(bulletAbstractNum, 1, "-", NumberFormatValues.Bullet);
            AppendLevel(bulletAbstractNum, 2, "-", NumberFormatValues.Bullet);

            // Создаем AbstractNum для нумерованного списка "1."
            var numberedDotAbstractNum = new AbstractNum()
            {
                AbstractNumberId = 1002 // Идентификатор AbstractNum
            };

            AppendLevel(numberedDotAbstractNum, 0, "1.", NumberFormatValues.Decimal);
            AppendLevel(numberedDotAbstractNum, 1, "1.", NumberFormatValues.Decimal);
            AppendLevel(numberedDotAbstractNum, 2, "1.", NumberFormatValues.Decimal);

            //// Создаем AbstractNum для нумерованного списка "1)"
            var numberedParenthesisAbstractNum = new AbstractNum()
            {
                AbstractNumberId = 1003 // Идентификатор AbstractNum
            };

            AppendLevel(numberedParenthesisAbstractNum, 0, "1)", NumberFormatValues.Decimal);
            AppendLevel(numberedParenthesisAbstractNum, 1, "1)", NumberFormatValues.Decimal);
            AppendLevel(numberedParenthesisAbstractNum, 2, "1)", NumberFormatValues.Decimal);

            // Добавляем AbstractNum в Numbering
            numbering.Append(bulletAbstractNum);
            numbering.Append(numberedDotAbstractNum);
            numbering.Append(numberedParenthesisAbstractNum);

            // Создаем экземпляры NumberingInstance
            var bulletNum = new NumberingInstance(new AbstractNumId { Val = 1001 }) { NumberID = 1001 };
            var numberedDotNum = new NumberingInstance(new AbstractNumId { Val = 1002 }) { NumberID = 1002 };
            var numberedParenthesisNum = new NumberingInstance(new AbstractNumId { Val = 1003 }) { NumberID = 1003 };

            // Добавляем NumberingInstance в Numbering
            numbering.Append(bulletNum);
            numbering.Append(numberedDotNum);
            numbering.Append(numberedParenthesisNum);

            // Сохраняем изменения
            numberingPart.Numbering.Save();
        }
        public void AppendLevel(AbstractNum num, int level, string marker, NumberFormatValues type)
        {
            num.AppendChild(new Level(
                    new StartNumberingValue { Val = 1 },
                    new NumberingFormat { Val = type },
                    new LevelText { Val = marker },
                    new LevelJustification { Val = LevelJustificationValues.Left },
                    new LevelSuffix { Val = LevelSuffixValues.Space }
                )
            { LevelIndex = level });
        }
    }
}