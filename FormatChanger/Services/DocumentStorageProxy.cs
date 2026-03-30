using FormatChanger.Models;
using FormatChanger.Services.Interfaces;

namespace FormatChanger.Services
{
	public class DocumentStorageProxy : IDocumentStorage
	{
		private readonly IDocumentStorage _realStorage;
		private readonly Dictionary<long, DocumentModel> _cache = new();

		public DocumentStorageProxy(IDocumentStorage realStorage)
		{
			_realStorage = realStorage;
		}

		public async Task<DocumentModel> SaveAsync(IFormFile file)
		{
			var document = await _realStorage.SaveAsync(file);
			_cache[document.Id] = document;

			return document;
		}

		public async Task<DocumentModel> GetByIdAsync(long id)
		{
			if (_cache.TryGetValue(id, out var cached))
			{
				return cached;
			}

			var document = await _realStorage.GetByIdAsync(id);

			if (document != null)
			{
				_cache[id] = document;
			}

			return document;
		}
	}
}