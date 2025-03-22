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
                case ParagraphTypes.FirstH:
                    return "FirstH";
                case ParagraphTypes.SecondH:
                    return "SecondH";
                case ParagraphTypes.ThirdH:
                    return "ThirdH";
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
                case "FirstH":
                    return ParagraphTypes.FirstH;
                case "SecondH":
                    return ParagraphTypes.SecondH;
                case "ThirdH":
                    return ParagraphTypes.ThirdH;
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
        ImageCaption = 1,
        TableCaption = 2,
        Period = 3,
        Bracket = 4,
        Dash = 5,
        FirstH = 6,
        SecondH = 7,
        ThirdH = 8,
        NoEdit = 9
    }
    public class ParagraphModel
    {
        public Paragraph Paragraph { get; set; }
        public string Type { get; set; }
        public string InnerText => Paragraph.InnerText;
    }
}
