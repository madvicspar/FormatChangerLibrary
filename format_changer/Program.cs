using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

internal class Program
{

    // open xml
    public static void ChangeFormat()
    {
        string filePath = "../../../header1.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            StyleDefinitionsPart stylePart = doc.MainDocumentPart.StyleDefinitionsPart;

            if (stylePart != null)
            {
                Styles styles = stylePart.Styles;

                if (styles != null)
                {
                    // Найдем стиль "Heading1"
                    Style heading1Style = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "1");

                    if (heading1Style != null)
                    {
                        // Изменяем свойства стиля
                        heading1Style.Descendants<Name>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Italic>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Bold>().FirstOrDefault()?.Remove();
                        heading1Style.AppendChild(
                            new StyleParagraphProperties(
                                new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0",  After = "240" },
                                new Indentation { Left = "0", Right = "0", FirstLine = "0" },
                                new Justification { Val = JustificationValues.Center }));
                        heading1Style.AppendChild(
                            new StyleRunProperties(
                                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new Color { Val = "000000" },
                                new Bold { Val = true },
                                new Italic { Val = false },
                                new Underline { Val = UnderlineValues.None },
                                new FontSize { Val = "32" }));
                        Console.WriteLine("Style 'Heading1' modified successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Style 'Heading1' not found.");
                    }
                }
            }

            // Задаем свойства стиля
            // Добавьте остальные свойства ...

            //customHeading1Style.StyleParagraphProperties = new StyleParagraphProperties(
            //    new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, After = "240" },
            //    new Indentation { Left = "0", Right = "0", FirstLine = "0" },
            //    new Justification { Val = JustificationValues.Center }
            //);

            //customHeading1Style.StyleRunProperties = new StyleRunProperties(
            //    new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
            //    new Color { Val = "000000" },
            //    new Bold { Val = true },
            //    new Italic { Val = false },
            //    new FontSize { Val = "32" }
            //);

            //doc.MainDocumentPart. = stylePart;

            //// Применяем стиль к первой строке в файле
            //Paragraph firstParagraph = doc.MainDocumentPart.Document.Body.Elements<Paragraph>().FirstOrDefault();
            //if (firstParagraph != null)
            //{
            //    firstParagraph.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId { Val = "CustomHeading1" });
            //}

            //stylePart.Styles.Append(customHeading1Style);
            //stylePart.Styles.Save();

            //Console.WriteLine("Done");
        }
    }

    public static void GetProperty()
    {
        string filePath = "../../../header1.docx";

        using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
        {
            StyleDefinitionsPart stylePart = wordDoc.MainDocumentPart.StyleDefinitionsPart;

            if (stylePart != null)
            {
                Styles styles = stylePart.Styles;

                if (styles != null)
                {
                    foreach (Style style in styles.Elements<Style>())
                    {
                        Console.WriteLine($"Style ID: {style.StyleId}");
                        Console.WriteLine($"Style Name: {style.StyleName.Val}");
                        Console.WriteLine($"Based On: {style.Descendants<BasedOn>().FirstOrDefault()?.Val}");
                        Console.WriteLine($"Next Paragraph Style: {style.Descendants<NextParagraphStyle>().FirstOrDefault()?.Val}");
                        Console.WriteLine($"UI Priority: {style.Descendants<UIPriority>().FirstOrDefault()?.Val}");
                        // Вывод остальных свойств...

                        Console.WriteLine();
                    }
                }
            }
        }
    }

    private static void Main(string[] args)
    {
        ChangeFormat();
    }
}