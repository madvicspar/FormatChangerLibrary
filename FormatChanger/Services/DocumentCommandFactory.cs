using FormatChanger.Services.Interfaces;

using static FormatChanger.Services.CheckDocumentCommand;

namespace FormatChanger.Services
{
	public class DocumentCommandFactory
	{
		private readonly IServiceProvider _provider;

		public DocumentCommandFactory(IServiceProvider provider)
		{
			_provider = provider;
		}

		public IDocumentCommand Create(int actionId)
		{
			return actionId switch
			{
				1 => _provider.GetRequiredService<CorrectDocumentCommand>(),
				2 => _provider.GetRequiredService<CheckDocumentCommand>(),
				3 => _provider.GetRequiredService<EvaluateDocumentCommand>(),
				_ => throw new ArgumentException("Unknown action")
			};
		}
	}
}