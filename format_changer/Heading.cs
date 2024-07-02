using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;

namespace format_changer
{
    public class Heading
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Font { get; set; }
        public Color Color { get; set; }
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool IsUnderscore { get; set; }
        public float FontSize { get; set; }
        public float LineSpacing { get; set; }
        public float BeforeSpacing { get; set; }
        public float AfterSpacing { get; set; }
        public string Justification { get; set; }
        public bool IsPageBreakBefore { get; set; }
        public bool IsNumbered { get; set; }
        public int NumberingId { get; set; }
        public int NumberingLevelReference { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }
        public float FirstLine { get; set; }

        public Heading(string font, Color color, bool bold, bool italic, UnderlineValues underline,
            string fontSize, string lineSpacing, string beforeSpacing, string afterSpacing, JustificationValues justification,
            bool isPageBreakBefore, bool isNumbered, int numberingId, int numberingLevelReference, int left, int right, int firstLine)
        {
            Font = font;
            Color = color;
            Bold = bold;
            Italic = italic;
            IsUnderscore = underline != UnderlineValues.None;
            FontSize = float.Parse(fontSize);
            LineSpacing = float.Parse(lineSpacing);
            BeforeSpacing = float.Parse(beforeSpacing);
            AfterSpacing = float.Parse(afterSpacing);
            Justification = Parse(justification);
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
                new RunFonts { Ascii = Font, HighAnsi = Font },
                new Color { Val = Color.Val },
                new Bold { Val = Bold },
                new Italic { Val = Italic },
                new Underline { Val = IsUnderscore ? UnderlineValues.Single : UnderlineValues.None },
                new FontSize { Val = FontSize.ToString() }
            );

            return runProperties;
        }

        public ParagraphProperties GetParagraphProperties()
        {
            var paragraphProperties = new ParagraphProperties(
                new SpacingBetweenLines { Line = LineSpacing.ToString(), LineRule = LineSpacingRuleValues.Auto, Before = BeforeSpacing.ToString(), After = AfterSpacing.ToString() },
                new Indentation { Left = Left.ToString(), Right = Right.ToString(), FirstLine = FirstLine.ToString() },
                new Justification { Val = Parse(Justification) }
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

        public JustificationValues Parse(string value)
        {
            return value.ToLower() == "center" ? JustificationValues.Center : value.ToLower() == "left" ? JustificationValues.Left : value.ToLower() == "right" ? JustificationValues.Right : JustificationValues.Both;
        }

        public string Parse(JustificationValues value)
        {
            return value == JustificationValues.Center ? "center" : value == JustificationValues.Left ? "left" : value == JustificationValues.Right ? "right" : "both";
        }
    }
}
