using DocumentFormat.OpenXml.Packaging;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Interfaces
{
    public interface IElementCorrectionStrategy<T>
    {
        /// <summary>
        /// Исправляет форматирование элемента на основе заданных настроек форматирования
        /// </summary>
        /// <param name="template">Шаблон форматирования</param>
        void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template);
        /// <summary>
        /// Генерирует список несоответствий в форматировании элемента на основе заданных настроек форматирования
        /// </summary>
        /// <param name="template">Шаблон форматирования</param>
        List<string> CheckFormatting(ParagraphStyleProperties actual, FormattingTemplateModel template);
    }
}