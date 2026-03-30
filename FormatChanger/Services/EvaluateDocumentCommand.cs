using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Services.Interfaces;

namespace FormatChanger.Services
{
	public class EvaluateDocumentCommand : IDocumentCommand
	{
		private readonly IDocumentService _documentService;

		public EvaluateDocumentCommand(IDocumentService documentService)
		{
			_documentService = documentService;
		}

		public async Task<DocumentModel> ExecuteAsync(
			DocumentModel document,
			FormattingTemplateModel template,
			string[] types)
		{
			return await _documentService.EvaluateDocumentAsync(document, template, types);
		}
	}
}