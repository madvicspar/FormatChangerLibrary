using DocumentFormat.OpenXml.Wordprocessing;

namespace format_changer.Utilities
{
    public class JustificationConverter
    {
        private static readonly Dictionary<JustificationValues, string> JustificationMap = new Dictionary<JustificationValues, string>
        {
            { JustificationValues.Center, "Center" },
            { JustificationValues.Left, "Left" },
            { JustificationValues.Right, "Right" }
        };

        public static string Parse(JustificationValues value)
        {
            if (JustificationMap.TryGetValue(value, out string result))
            {
                return result;
            }
            return "Both";
        }
        public static JustificationValues Parse(string value)
        {
            return value.ToLower() switch
            {
                "center" => JustificationValues.Center,
                "left" => JustificationValues.Left,
                "right" => JustificationValues.Right,
                _ => JustificationValues.Both
            };
        }
    }
}
