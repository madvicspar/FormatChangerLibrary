
using DocumentFormat.OpenXml.Packaging;

using FormatChanger.WebAPI.Models.FormattingModels;
using FormatChanger.WebAPI.Models.Helpers;
using FormatChanger.WebAPI.Services.Interfaces;

namespace FormatChanger.WebAPI.Services
{
	public class DocumentCorrector : IDocumentCorrector
	{
		private readonly IElementCorrectionStrategy<TextSettingsModel> _textCorrectionStrategy;
		private readonly IElementCorrectionStrategy<HeadingSettingsModel> _headingFirstCorrectionStrategies;
		private readonly IElementCorrectionStrategy<ImageSettingsModel> _imageCorrectionStrategy;
		private readonly IElementCorrectionStrategy<ImageCaptionSettingsModel> _imageCaptionCorrectionStrategy;
		private readonly IElementCorrectionStrategy<TableSettingsModel> _tableCorrectionStrategy;
		private readonly IElementCorrectionStrategy<CellSettingsModel> _cellCorrectionStrategy;
		private readonly IElementCorrectionStrategy<HeaderSettingsModel> _headerTableCorrectionStrategies;
		private readonly IElementCorrectionStrategy<TableCaptionSettingsModel> _tableCaptionCorrectionStrategy;
		public DocumentCorrector(IElementCorrectionStrategy<TextSettingsModel> textStrategy,
			IElementCorrectionStrategy<HeadingSettingsModel> h1Strategy,
			IElementCorrectionStrategy<ImageSettingsModel> imageStrategy,
			IElementCorrectionStrategy<TableSettingsModel> tableCorrectionStrategy,
			IElementCorrectionStrategy<CellSettingsModel> cellCorrectionStrategy,
			IElementCorrectionStrategy<HeaderSettingsModel> headerTableCorrectionStrategies,
			IElementCorrectionStrategy<ImageCaptionSettingsModel> imageCaptionCorrectionStrategy,
			IElementCorrectionStrategy<TableCaptionSettingsModel> tableCaptionCorrectionStrategy)
		{
			_textCorrectionStrategy = textStrategy;
			_headingFirstCorrectionStrategies = h1Strategy;
			_imageCorrectionStrategy = imageStrategy;
			_tableCorrectionStrategy = tableCorrectionStrategy;
			_cellCorrectionStrategy = cellCorrectionStrategy;
			_headerTableCorrectionStrategies = headerTableCorrectionStrategies;
			_imageCaptionCorrectionStrategy = imageCaptionCorrectionStrategy;
			_tableCaptionCorrectionStrategy = tableCaptionCorrectionStrategy;
		}
		public async Task ApplyAllStrategiesAsync(WordprocessingDocument doc, FormattingTemplateModel template, List<ParagraphModel> paragraphs)
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
	}
}