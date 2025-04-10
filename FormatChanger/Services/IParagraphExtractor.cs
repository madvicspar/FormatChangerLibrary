using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public interface IParagraphExtractor
    {
        List<ParagraphModel>? Extract(WordprocessingDocument doc);
    }
}