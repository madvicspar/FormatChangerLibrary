using DocumentFormat.OpenXml.Wordprocessing;
using format_changer.Utilities;
using System.ComponentModel.DataAnnotations.Schema;

namespace format_changer.Models
{
    public class TableCellsSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Font { get; set; }
        public Color Color { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public bool IsUnderscore { get; set; }
        public float FontSize { get; set; }
        public float LineSpacing { get; set; }
        public float BeforeSpacing { get; set; }
        public float AfterSpacing { get; set; }
        public string HorizontalJustification { get; set; }
        public string VerticalJustification { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }
        public float FirstLine { get; set; }
        public float TopMargin { get; set; }
        public float BottomMargin { get; set; }
        public float LeftMargin { get; set; }
        public float RightMargin { get; set; }

        public TableCellsSettings(string font, Color color, bool isBold, bool isItalic, UnderlineValues underline,
            string fontSize, string lineSpacing, string beforeSpacing, string afterSpacing, JustificationValues horizontalJustification, TableVerticalAlignmentValues varticalJustification,
            int left, int right, int firstLine, float topMargin, float bottomMargin, float leftMargin, float rightMargin)
        {
            Font = font;
            Color = color;
            IsBold = isBold;
            IsItalic = isItalic;
            IsUnderscore = underline != UnderlineValues.None;
            FontSize = float.Parse(fontSize);
            LineSpacing = float.Parse(lineSpacing);
            BeforeSpacing = float.Parse(beforeSpacing);
            AfterSpacing = float.Parse(afterSpacing);
            HorizontalJustification = JustificationConverter.Parse(horizontalJustification);
            VerticalJustification = VerticalAligngmentConverter.Parse(varticalJustification);
            Left = left;
            Right = right;
            FirstLine = firstLine;
            TopMargin = topMargin;
            BottomMargin = bottomMargin;
            LeftMargin = leftMargin;
            RightMargin = rightMargin;
        }

        public RunProperties GetRunProperties()
        {
            var runProperties = new RunProperties(
                new RunFonts { Ascii = Font, HighAnsi = Font },
                new Color { Val = Color.Val },
                new Bold { Val = IsBold },
                new Italic { Val = IsItalic },
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
                new Justification { Val = JustificationConverter.Parse(HorizontalJustification) }
            );

            return paragraphProperties;
        }

        public TableCellMargin GetTableCellMargin()
        {
            var tableCellMargin = new TableCellMargin(
                new TopMargin { Width = TopMargin.ToString() },
                new BottomMargin { Width = BottomMargin.ToString() },
                new LeftMargin { Width = LeftMargin.ToString() },
                new RightMargin { Width = RightMargin.ToString() }
            );

            return tableCellMargin;
        }

        public TableCellVerticalAlignment GetVerticalAlignment()
        {
            return new TableCellVerticalAlignment { Val = VerticalAligngmentConverter.Parse(VerticalJustification) };
        }
    }
}