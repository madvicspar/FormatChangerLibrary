using DocumentFormat.OpenXml.Wordprocessing;

namespace FormatChanger.Models
{
    public class ParagraphModel
    {
        public Paragraph Paragraph { get; set; }
        public string Type { get; set; }
        public string InnerText => Paragraph.InnerText;
    }
}
