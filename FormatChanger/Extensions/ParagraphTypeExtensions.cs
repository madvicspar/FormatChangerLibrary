using FormatChanger.Models.Helpers;

namespace FormatChanger.Extensions
{
    public static class ParagraphTypeExtensions
    {
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
}
