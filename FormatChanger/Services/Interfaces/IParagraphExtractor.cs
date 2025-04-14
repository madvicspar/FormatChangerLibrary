using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models.Helpers;

namespace FormatChanger.Services.Interfaces
{
    public interface IParagraphExtractor
    {
        List<ParagraphModel>? Extract(WordprocessingDocument doc);
    }
}