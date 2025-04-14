using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models.Helpers;

namespace FormatChanger.Services.Interfaces
{
    public interface IParagraphNumbering
    {
        void Apply(WordprocessingDocument doc, List<ParagraphModel> paragraphs);
    }
}