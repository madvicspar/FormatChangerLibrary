using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace FormatChanger.Services
{
    // TODO: add other style strategies and glodal document strategy
    public interface IElementCorrectionStrategy<T>
    {
        void ApplyCorrection(WordprocessingDocument doc, T settings);
        RunProperties GetRunProperties(T settings);
        ParagraphProperties GetParagraphProperties(T settings);
    }
}