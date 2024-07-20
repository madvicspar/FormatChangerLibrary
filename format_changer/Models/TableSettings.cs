using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel.DataAnnotations.Schema;

namespace format_changer.Models
{
    public class TableSettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsTableSignature { get; set; }
        public bool IsHeading { get; set; }
        public float BeforeSpacing { get; set; }
        public float AfterSpacing { get; set; }

        public TableSettings(bool isTableSignature, bool isHeading, string beforeSpacing, string afterSpacing)
        {
            IsTableSignature = isTableSignature;
            IsHeading = isHeading;
            BeforeSpacing = float.Parse(beforeSpacing);
            AfterSpacing = float.Parse(afterSpacing);
        }

        public ParagraphProperties GetParagraphProperties()
        {
            var paragraphProperties = new ParagraphProperties(
                new SpacingBetweenLines { LineRule = LineSpacingRuleValues.Auto, Before = BeforeSpacing.ToString(), After = AfterSpacing.ToString() }
            );

            return paragraphProperties;
        }
    }
}