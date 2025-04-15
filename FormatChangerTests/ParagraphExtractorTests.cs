using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.Helpers;
using FormatChanger.Services;

namespace FormatChangerTests
{
    public class ParagraphExtractorTests
    {
        private ParagraphExtractor _extractor = new();
        [Fact]
        public void Extract_Returns_FirstH_For_Heading1_Style()
        {
            // Arrange
            using var stream = CreateDocWithParagraph("heading 1");
            using var doc = WordprocessingDocument.Open(stream, false);

            // Act
            var result = _extractor.Extract(doc);

            // Assert
            Assert.Single(result);
            Assert.Equal(ParagraphTypes.FirstH.ToString(), result[0].Type);
        }

        [Fact]
        public void Extract_Returns_SecondH_For_Heading2_Style()
        {
            // Arrange
            using var stream = CreateDocWithParagraph("heading 2");
            using var doc = WordprocessingDocument.Open(stream, false);

            // Act
            var result = _extractor.Extract(doc);

            // Assert
            Assert.Single(result);
            Assert.Equal(ParagraphTypes.SecondH.ToString(), result[0].Type);
        }

        [Fact]
        public void Extract_Returns_ThirdH_For_Heading3_Style()
        {
            // Arrange
            using var stream = CreateDocWithParagraph("heading 3");
            using var doc = WordprocessingDocument.Open(stream, false);

            // Act
            var result = _extractor.Extract(doc);

            // Assert
            Assert.Single(result);
            Assert.Equal(ParagraphTypes.ThirdH.ToString(), result[0].Type);
        }

        [Fact]
        public void Extract_Returns_ImageCaption()
        {
            // Arrange
            using var stream = CreateDocWithImageAndCaption();
            using var doc = WordprocessingDocument.Open(stream, false);

            // Act
            var result = _extractor.Extract(doc);

            // Assert
            Assert.Single(result);
            Assert.Equal(ParagraphTypes.ImageCaption.ToString(), result[0].Type);
        }

        [Fact]
        public void Extract_Returns_TableCaption()
        {
            // Arrange
            using var stream = CreateDocWithTableAndCaption();
            using var doc = WordprocessingDocument.Open(stream, false);

            // Act
            var result = _extractor.Extract(doc);

            // Assert
            Assert.Single(result);
            Assert.Equal(ParagraphTypes.TableCaption.ToString(), result[0].Type);
        }

        private MemoryStream CreateDocWithParagraph(string? styleName)
        {
            var styleId = styleName.Replace(" ", "").ToLower();
            var stream = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());

                var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                stylesPart.Styles = new Styles();
                if (!string.IsNullOrEmpty(styleName))
                {
                    var style = new Style
                    {
                        Type = StyleValues.Paragraph,
                        StyleId = styleId,
                        StyleName = new StyleName { Val = styleName }
                    };
                    stylesPart.Styles.Append(style);
                }

                var p = new Paragraph(
                    new ParagraphProperties(
                        new ParagraphStyleId { Val = styleId ?? "normal" }
                    ),
                    new Run(new Text("Sample text"))
                );

                mainPart.Document.Body.Append(p);
                mainPart.Document.Save();
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        private MemoryStream CreateDocWithImageAndCaption()
        {
            var stream = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());

                var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                stylesPart.Styles = new Styles();

                var drawingPara = new Paragraph(new Run(new Drawing()));
                var captionPara = new Paragraph(new Run(new Text("Caption")));

                mainPart.Document.Body.Append(drawingPara, captionPara);
                mainPart.Document.Save();
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private MemoryStream CreateDocWithTableAndCaption()
        {
            var stream = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());

                var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                stylesPart.Styles = new Styles();

                var captionPara = new Paragraph(new Run(new Text("Table caption")));
                var table = new Table(new TableRow(new TableCell(new Paragraph(new Run(new Text("cell"))))));

                mainPart.Document.Body.Append(captionPara);
                mainPart.Document.Body.Append(table);
                mainPart.Document.Save();
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}