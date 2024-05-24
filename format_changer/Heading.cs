using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace format_changer
{
    public class Heading
    {
        public RunFonts Font { get; set; }
        public Color Color { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public UnderlineValues Underline { get; set; }
        public string FontSize { get; set; }
        public string LineSpacing { get; set; }
        public string BeforeSpacing { get; set; }
        public string AfterSpacing { get; set; }
        public JustificationValues Justification { get; set; }
        public bool IsPageBreakBefore { get; set; }
        public bool IsNumbered { get; set; }
        public int NumberingId { get; set; }
        public int NumberingLevelReference { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int FirstLine { get; set; }

        public Heading(RunFonts font, Color color, bool bold, bool italic, UnderlineValues underline,
            string fontSize, string lineSpacing, string beforeSpacing, string afterSpacing, JustificationValues justification,
            bool isPageBreakBefore, bool isNumbered, int numberingId, int numberingLevelReference, int left, int right, int firstLine)
        {
            Font = font;
            Color = color;
            Bold = bold;
            Italic = italic;
            Underline = underline;
            FontSize = fontSize;
            LineSpacing = lineSpacing;
            BeforeSpacing = beforeSpacing;
            AfterSpacing = afterSpacing;
            Justification = justification;
            IsPageBreakBefore = isPageBreakBefore;
            IsNumbered = isNumbered;
            NumberingId = numberingId;
            NumberingLevelReference = numberingLevelReference;
            Left = left;
            Right = right;
            FirstLine = firstLine;
        }

        public RunProperties GetRunProperties()
        {
            var runProperties = new RunProperties(
                new RunFonts { Ascii = Font.Ascii, HighAnsi = Font.HighAnsi },
                new Color { Val = Color.Val },
                new Bold { Val = Bold },
                new Italic { Val = Italic },
                new Underline { Val = new EnumValue<UnderlineValues>(Underline) },
                new FontSize { Val = FontSize }
            );

            return runProperties;
        }

        public ParagraphProperties GetParagraphProperties()
        {
            var paragraphProperties = new ParagraphProperties(
                new SpacingBetweenLines { Line = LineSpacing, LineRule = LineSpacingRuleValues.Auto, Before = BeforeSpacing, After = AfterSpacing },
                new Indentation { Left = Left.ToString(), Right = Right.ToString(), FirstLine = FirstLine.ToString() },
                new Justification { Val = Justification }
            );

            if (IsPageBreakBefore)
                paragraphProperties.AddChild(new PageBreakBefore());
            if (IsNumbered)
            {
                paragraphProperties.AddChild(new NumberingProperties
                {
                    NumberingId = new NumberingId() { Val = NumberingId },
                    NumberingLevelReference = new NumberingLevelReference() { Val = NumberingLevelReference },
                });
            }

            return paragraphProperties;
        }
    }
}
