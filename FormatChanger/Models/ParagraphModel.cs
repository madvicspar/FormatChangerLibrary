using DocumentFormat.OpenXml.Wordprocessing;
using System.Reflection.Emit;

namespace FormatChanger.Models
{
    public static class ParagraphTypesEnumExtensions
    {
        // Преобразование enum в строку
        public static string ToString(this ParagraphTypes type)
        {
            switch (type)
            {
                case ParagraphTypes.Normal:
                    return "Normal";
                case ParagraphTypes.Heading:
                    return "Heading";
                case ParagraphTypes.List:
                    return "List";
                case ParagraphTypes.ImageCaption:
                    return "ImageCaption";
                case ParagraphTypes.TableCaption:
                    return "TableCaption";
                case ParagraphTypes.NoEdit:
                    return "no-edit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Неизвестный тип абзаца");
            }
        }

        // Преобразование строки в enum
        public static ParagraphTypes ToEnum(this string typeString)
        {
            switch (typeString)
            {
                case "Normal":
                    return ParagraphTypes.Normal;
                case "Heading":
                    return ParagraphTypes.Heading;
                case "List":
                    return ParagraphTypes.List;
                case "ImageCaption":
                    return ParagraphTypes.ImageCaption;
                case "TableCaption":
                    return ParagraphTypes.TableCaption;
                case "no-edit":
                    return ParagraphTypes.NoEdit;
                default:
                    throw new ArgumentException($"Некорректное значение для типа абзаца: {typeString}");
            }
        }
    }
    public enum ParagraphTypes
    {
        Normal = 0,
        Heading = 1,
        List = 2,
        ImageCaption = 3,
        TableCaption = 4,
        NoEdit = 5
    }
    public class ParagraphModel
    {
        public Paragraph Paragraph { get; set; }
        public string Type { get; set; }
        public string InnerText => Paragraph.InnerText;
    }
}
