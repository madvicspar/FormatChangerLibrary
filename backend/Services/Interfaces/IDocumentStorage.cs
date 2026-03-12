using FormatChanger.WebAPI.Models;
namespace FormatChanger.WebAPI.Services.Interfaces
{
	public interface IDocumentStorage
	{
		Task<DocumentModel> SaveAsync(IFormFile file);
		Task<DocumentModel> GetByIdAsync(long id);
	}
}