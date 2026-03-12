using Microsoft.AspNetCore.Mvc;

namespace FormatChanger.WebAPI.Controllers
{
	[ApiController]
	[Route("api/v1/templates")]
	public class TemplatesControllerTests : ControllerBase
	{
		private readonly ITemplateService _templateService;

		public TemplatesControllerTests(ITemplateService templateService)
		{
			_templateService = templateService;
		}

		private static readonly List<TemplateDto> Templates = new()
		{
			new TemplateDto { templateId = 1, title = "Ñòàíäàðò ÐÈÑ-22", documentSettingsId = 1, textSettingsId = 1, headingSettingsId = 1, tableSettingsId = 1, listSettingsId = 1, imageSettingsId = 1 },
			new TemplateDto { templateId = 2, title = "ÃÎÑÒ ÌÁ-24", documentSettingsId = 2, textSettingsId = 2, headingSettingsId = 2, tableSettingsId = 2, listSettingsId = 2, imageSettingsId = 2 }
		};

		[HttpGet]
		public IActionResult Get([FromQuery] int page = 1, [FromQuery] int limit = 10)
		{
			return Ok(new { success = true, data = Templates });
		}

		[HttpGet("{id}")]
		public IActionResult GetById(long id)
		{
			var template = Templates.FirstOrDefault(t => t.templateId == id);
			if (template == null) return NotFound(new { success = false, error = "Not found" });
			return Ok(new { success = true, data = template });
		}

		[HttpPost]
		public IActionResult Create([FromBody] CreateTemplateDto dto)
		{
			var newId = Templates.Max(t => t.templateId) + 1;
			return CreatedAtAction(nameof(GetById), new { id = newId }, new { success = true, templateId = newId });
		}

		[HttpPut("{id}")]
		public IActionResult Update(long id, [FromBody] CreateTemplateDto dto)
		{
			var existingTemplate = Templates.FirstOrDefault(t => t.templateId == id);
			if (existingTemplate == null)
				return NotFound(new { success = false, error = "Template not found" });

			existingTemplate.title = dto.title;
			existingTemplate.documentSettingsId = dto.documentSettingsId;
			existingTemplate.textSettingsId = dto.textSettingsId;
			existingTemplate.headingSettingsId = dto.headingSettingsId;
			existingTemplate.tableSettingsId = dto.tableSettingsId;
			existingTemplate.listSettingsId = dto.listSettingsId;
			existingTemplate.imageSettingsId = dto.imageSettingsId;

			return Ok(new { status = "ok", templateId = existingTemplate.templateId });
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(long id)
		{
			var template = Templates.FirstOrDefault(t => t.templateId == id);
			if (template == null)
				return NotFound(new { success = false, error = "Template not found" });

			Templates.Remove(template);
			return Ok(new { status = "ok" });
		}

		public class TemplateDto { public long templateId { get; set; } public string title { get; set; } public long imageSettingsId { get; set; } public long documentSettingsId { get; set; } public long textSettingsId { get; set; } public long headingSettingsId { get; set; } public long tableSettingsId { get; set; } public long listSettingsId { get; set; } }
		public class CreateTemplateDto { public string title { get; set; } public long imageSettingsId { get; set; } public long documentSettingsId { get; set; } public long textSettingsId { get; set; } public long headingSettingsId { get; set; } public long tableSettingsId { get; set; } public long listSettingsId { get; set; } }
	}
}
