using DocumentFormat.OpenXml.Wordprocessing;

namespace format_changer.Utilities
{
    public class VerticalAligngmentConverter
    {
        private static readonly Dictionary<TableVerticalAlignmentValues, string> VerticalAlignmentsMap = new Dictionary<TableVerticalAlignmentValues, string>
        {
            { TableVerticalAlignmentValues.Center, "Center" },
            { TableVerticalAlignmentValues.Top, "Top" },
            { TableVerticalAlignmentValues.Bottom, "Bottom" }
        };

        public static string Parse(TableVerticalAlignmentValues value)
        {
            if (VerticalAlignmentsMap.TryGetValue(value, out string result))
            {
                return result;
            }
            return "Both";
        }
        public static TableVerticalAlignmentValues Parse(string value)
        {
            return value.ToLower() switch
            {
                "center" => TableVerticalAlignmentValues.Center,
                "top" => TableVerticalAlignmentValues.Top,
                "bottom" => TableVerticalAlignmentValues.Bottom,
                _ => TableVerticalAlignmentValues.Center
            };
        }
    }
}
