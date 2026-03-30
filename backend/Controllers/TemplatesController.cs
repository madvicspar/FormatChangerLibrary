using Microsoft.AspNetCore.Mvc;

namespace FormatChanger.WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TemplatesController : ControllerBase
	{
		private readonly ITemplateService _templateService;

		public TemplatesController(ITemplateService templateService)
		{
			_templateService = templateService;
		}

		[HttpGet]
		public async Task<IActionResult> GetTemplates()
		{
			var templates = await _templateService.GetTemplatesAsync();
			return Ok(templates);
		}
	}
}
