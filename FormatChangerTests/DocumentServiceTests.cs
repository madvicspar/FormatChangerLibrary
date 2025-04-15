using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Models.Helpers;
using FormatChanger.Services;
using FormatChanger.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FormatChangerTests
{
    public class DocumentServiceTests
    {
        private readonly Mock<IDocumentStorage> _storageMock = new();
        private readonly Mock<IParagraphExtractor> _extractorMock = new();
        private readonly Mock<IDocumentCorrector> _correctorMock = new();
        private readonly Mock<IDocumentChecker> _checkerMock = new();
        private readonly Mock<IParagraphStyler> _stylerMock = new();
        private readonly Mock<IParagraphNumbering> _numberingMock = new();

        private readonly DocumentService _service;

        public DocumentServiceTests()
        {
            _service = new DocumentService(
                _storageMock.Object,
                _extractorMock.Object,
                _correctorMock.Object,
                _checkerMock.Object,
                _stylerMock.Object,
                _numberingMock.Object);
        }
        [Fact]
        public async Task UploadDocumentAsync_CallsStorageSave()
        {
            // Arrange
            var formFile = new Mock<IFormFile>().Object;
            var expected = new DocumentModel { Id = 1, FilePath = "path.docx" };

            _storageMock.Setup(x => x.SaveAsync(formFile)).ReturnsAsync(expected);

            // Act
            var result = await _service.UploadDocumentAsync(formFile);

            // Assert
            Assert.Equal(expected, result);
            _storageMock.Verify(x => x.SaveAsync(formFile), Times.Once);
        }

        [Fact]
        public async Task GetDocumentByIdAsync_ReturnsCorrectDocument()
        {
            var doc = new DocumentModel { Id = 1, FilePath = "doc.docx" };
            _storageMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(doc);

            var result = await _service.GetDocumentByIdAsync(1);

            Assert.Equal(doc, result);
            _storageMock.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task CorrectDocumentAsync_ExtractsParagraphs()
        {
            // Arrange
            var extractorMock = new Mock<IParagraphExtractor>();
            var stylerMock = new Mock<IParagraphStyler>();
            var correctorMock = new Mock<IDocumentCorrector>();
            var numberingMock = new Mock<IParagraphNumbering>();

            var docModel = CreateFakeDocModel();

            extractorMock.Setup(e => e.Extract(It.IsAny<WordprocessingDocument>()))
                .Returns(new List<ParagraphModel>());

            var service = new DocumentService(
                storage: Mock.Of<IDocumentStorage>(),
                extractor: extractorMock.Object,
                corrector: correctorMock.Object,
                checker: Mock.Of<IDocumentChecker>(),
                styler: stylerMock.Object,
                numbering: numberingMock.Object
            );

            var template = new FormattingTemplateModel();
            var types = new[] { "heading", "body" };

            // Act
            await service.CorrectDocumentAsync(docModel, template, types);

            // Assert
            extractorMock.Verify(e => e.Extract(It.IsAny<WordprocessingDocument>()), Times.Once);
        }

        [Fact]
        public async Task CorrectDocumentAsync_EnsuresStylesExists()
        {
            // Arrange
            var extractorMock = new Mock<IParagraphExtractor>();
            var stylerMock = new Mock<IParagraphStyler>();
            var correctorMock = new Mock<IDocumentCorrector>();
            var numberingMock = new Mock<IParagraphNumbering>();

            var docModel = CreateFakeDocModel();

            extractorMock.Setup(e => e.Extract(It.IsAny<WordprocessingDocument>()))
                .Returns(new List<ParagraphModel>());

            var service = new DocumentService(
                storage: Mock.Of<IDocumentStorage>(),
                extractor: extractorMock.Object,
                corrector: correctorMock.Object,
                checker: Mock.Of<IDocumentChecker>(),
                styler: stylerMock.Object,
                numbering: numberingMock.Object
            );

            var template = new FormattingTemplateModel();
            var types = new[] { "heading", "body" };

            // Act
            await service.CorrectDocumentAsync(docModel, template, types);

            // Assert
            stylerMock.Verify(s => s.EnsureStylesExists(It.IsAny<Styles>(), ParagraphTypes.ImageCaption), Times.Once);
            stylerMock.Verify(s => s.EnsureStylesExists(It.IsAny<Styles>(), ParagraphTypes.TableCaption), Times.Once);
        }

        [Fact]
        public async Task CorrectDocumentAsync_AppliesStyles()
        {
            // Arrange
            var extractorMock = new Mock<IParagraphExtractor>();
            var stylerMock = new Mock<IParagraphStyler>();
            var correctorMock = new Mock<IDocumentCorrector>();
            var numberingMock = new Mock<IParagraphNumbering>();

            var docModel = CreateFakeDocModel();

            var paragraphs = new List<ParagraphModel>();
            extractorMock.Setup(e => e.Extract(It.IsAny<WordprocessingDocument>())).Returns(paragraphs);

            var service = new DocumentService(
                storage: Mock.Of<IDocumentStorage>(),
                extractor: extractorMock.Object,
                corrector: correctorMock.Object,
                checker: Mock.Of<IDocumentChecker>(),
                styler: stylerMock.Object,
                numbering: numberingMock.Object
            );

            var template = new FormattingTemplateModel();
            var types = new[] { "heading", "body" };

            // Act
            await service.CorrectDocumentAsync(docModel, template, types);

            // Assert
            stylerMock.Verify(s => s.ApplyStyles(It.IsAny<WordprocessingDocument>(), paragraphs, types), Times.Once);
        }

        [Fact]
        public async Task CorrectDocumentAsync_CleansFormat()
        {
            // Arrange
            var extractorMock = new Mock<IParagraphExtractor>();
            var stylerMock = new Mock<IParagraphStyler>();
            var correctorMock = new Mock<IDocumentCorrector>();
            var numberingMock = new Mock<IParagraphNumbering>();

            var docModel = CreateFakeDocModel();

            extractorMock.Setup(e => e.Extract(It.IsAny<WordprocessingDocument>()))
                .Returns(new List<ParagraphModel>());

            var service = new DocumentService(
                storage: Mock.Of<IDocumentStorage>(),
                extractor: extractorMock.Object,
                corrector: correctorMock.Object,
                checker: Mock.Of<IDocumentChecker>(),
                styler: stylerMock.Object,
                numbering: numberingMock.Object
            );

            var template = new FormattingTemplateModel();
            var types = new[] { "heading", "body" };

            // Act
            await service.CorrectDocumentAsync(docModel, template, types);

            // Assert
            stylerMock.Verify(s => s.CleanFormat(It.IsAny<WordprocessingDocument>()), Times.Once);
        }

        [Fact]
        public async Task CorrectDocumentAsync_AppliesStrategies()
        {
            // Arrange
            var extractorMock = new Mock<IParagraphExtractor>();
            var stylerMock = new Mock<IParagraphStyler>();
            var correctorMock = new Mock<IDocumentCorrector>();
            var numberingMock = new Mock<IParagraphNumbering>();

            var docModel = CreateFakeDocModel();

            var paragraphs = new List<ParagraphModel>();
            extractorMock.Setup(e => e.Extract(It.IsAny<WordprocessingDocument>())).Returns(paragraphs);

            var service = new DocumentService(
                storage: Mock.Of<IDocumentStorage>(),
                extractor: extractorMock.Object,
                corrector: correctorMock.Object,
                checker: Mock.Of<IDocumentChecker>(),
                styler: stylerMock.Object,
                numbering: numberingMock.Object
            );

            var template = new FormattingTemplateModel();
            var types = new[] { "heading", "body" };

            // Act
            await service.CorrectDocumentAsync(docModel, template, types);

            // Assert
            correctorMock.Verify(c => c.ApplyAllStrategiesAsync(It.IsAny<WordprocessingDocument>(), template, paragraphs), Times.Once);
        }

        [Fact]
        public async Task CorrectDocumentAsync_AppliesNumbering()
        {
            // Arrange
            var extractorMock = new Mock<IParagraphExtractor>();
            var stylerMock = new Mock<IParagraphStyler>();
            var correctorMock = new Mock<IDocumentCorrector>();
            var numberingMock = new Mock<IParagraphNumbering>();

            var docModel = CreateFakeDocModel();

            var paragraphs = new List<ParagraphModel>();
            extractorMock.Setup(e => e.Extract(It.IsAny<WordprocessingDocument>())).Returns(paragraphs);

            var service = new DocumentService(
                storage: Mock.Of<IDocumentStorage>(),
                extractor: extractorMock.Object,
                corrector: correctorMock.Object,
                checker: Mock.Of<IDocumentChecker>(),
                styler: stylerMock.Object,
                numbering: numberingMock.Object
            );

            var template = new FormattingTemplateModel();
            var types = new[] { "heading", "body" };

            // Act
            await service.CorrectDocumentAsync(docModel, template, types);

            // Assert
            numberingMock.Verify(n => n.Apply(It.IsAny<WordprocessingDocument>(), paragraphs), Times.Once);
        }

        private DocumentModel CreateFakeDocModel()
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".docx");

            using (var doc = WordprocessingDocument.Create(tempPath, WordprocessingDocumentType.Document))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(
                    new Body()
                );

                var stylePart = mainPart.AddNewPart<StyleDefinitionsPart>();
                stylePart.Styles = new Styles();
                stylePart.Styles.Save();
            }

            return new DocumentModel { FilePath = tempPath };
        }
    }
}