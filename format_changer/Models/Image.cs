using DocumentFormat.OpenXml.Wordprocessing;
using format_changer.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace format_changer.Models
{
    public class Image
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public float LineSpacing { get; set; }
        public float BeforeSpacing { get; set; }
        public float AfterSpacing { get; set; }
        public string Justification { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }
        public float FirstLine { get; set; }
        public bool IsKeepWithNext { get; set; }

        public Image(string lineSpacing, string beforeSpacing, string afterSpacing, JustificationValues justification,
            int left, int right, int firstLine, bool isKeepWithNext)
        {
            LineSpacing = float.Parse(lineSpacing);
            BeforeSpacing = float.Parse(beforeSpacing);
            AfterSpacing = float.Parse(afterSpacing);
            Justification = JustificationConverter.Parse(justification);
            Left = left;
            Right = right;
            FirstLine = firstLine;
            IsKeepWithNext = isKeepWithNext;
        }

        public RunProperties GetRunProperties()
        {
            return new RunProperties();
        }

        public ParagraphProperties GetParagraphProperties()
        {
            return new ParagraphProperties(
                new KeepNext { Val = IsKeepWithNext },
                new SpacingBetweenLines
                {
                    Line = LineSpacing.ToString(),
                    LineRule = LineSpacingRuleValues.Auto,
                    Before = BeforeSpacing.ToString(),
                    After = AfterSpacing.ToString()
                },
                new Indentation { Left = Left.ToString(), Right = Right.ToString(), FirstLine = FirstLine.ToString() },
                new Justification { Val = JustificationConverter.Parse(Justification) }
            );
        }
    }
}