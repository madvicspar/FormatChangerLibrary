using DocumentFormat.OpenXml.Wordprocessing;
using System.Reflection.Emit;

namespace FormatChanger.Models
{
    public static class ParagraphTypesEnumExtensions
    {
        // Преобразование enum в строку
        public static string ToEnumString(this ParagraphTypes type)
        {
            switch (type)
            {
                case ParagraphTypes.Normal:
                    return "normal";
                case ParagraphTypes.Heading:
                    return "heading";
                case ParagraphTypes.List:
                    return "list";
                case ParagraphTypes.ImageCaption:
                    return "caption-image";
                case ParagraphTypes.TableCaption:
                    return "caption-table";
                case ParagraphTypes.NoEdit:
                    return "no-edit";
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Неизвестный тип абзаца");
            }
        }

        // Преобразование строки в enum
        public static ParagraphTypes ToEnum(this string typeString)
        {
            // Приводим строку к нижнему регистру и пробуем разобрать
            typeString = typeString.ToLower();

            switch (typeString)
            {
                case "normal":
                    return ParagraphTypes.Normal;
                case "heading":
                    return ParagraphTypes.Heading;
                case "list":
                    return ParagraphTypes.List;
                case "caption-image":
                    return ParagraphTypes.ImageCaption;
                case "caption-table":
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
