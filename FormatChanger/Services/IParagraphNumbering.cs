using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public interface IParagraphNumbering
    {
        void Apply(WordprocessingDocument doc, List<ParagraphModel> paragraphs);
    }
}