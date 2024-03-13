using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace format_changer
{
    public class Heading
    {
        public string StyleId { get; set; }
        public string Name { get; set; }
        public int UIPriority { get; set; }
        public int FontSize { get; set; }
        public string FontName { get; set; }
        public string FontColor { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }
        public string Alignment { get; set; }
        public bool PageBreakBefore { get; set; }
        public double LineSpacing { get; set; }
        public double SpacingBefore { get; set; }
        public double SpacingAfter { get; set; }
        public bool IsNumeric { get; set; }
        public string NumberingFormat { get; set; }
        public int NumberingLevelReference { get; set; }
    }
}
