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
                case ParagraphTypes.Period:
                    return "Period";
                case ParagraphTypes.Bracket:
                    return "Bracket";
                case ParagraphTypes.Dash:
                    return "Dash";
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
                case "Period":
                    return ParagraphTypes.Period;
                case "Bracket":
                    return ParagraphTypes.Bracket;
                case "Dash":
                    return ParagraphTypes.Dash;
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
        ImageCaption = 2,
        TableCaption = 3,
        Period = 4,
        Bracket = 5,
        Dash = 6,
        NoEdit = 7
    }
    public class ParagraphModel
    {
        public Paragraph Paragraph { get; set; }
        public string Type { get; set; }
        public string InnerText => Paragraph.InnerText;
    }
}
