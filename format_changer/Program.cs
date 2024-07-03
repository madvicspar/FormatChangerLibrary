using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using format_changer.Models;

public class Program
{
    public static bool IsImageSignature = true;
    public static void ChangeHeading1()
    {
        // tab/пробел сохраняются
        string filePath = "../../../data/temp.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            StyleDefinitionsPart stylePart = doc.MainDocumentPart.StyleDefinitionsPart;

            if (stylePart != null)
            {
                Styles styles = stylePart.Styles;

                if (styles != null)
                {
                    Style heading1Style = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "1");

                    if (heading1Style != null)
                    {
                        Heading h1 = GetHeading1();
                        heading1Style.RemoveAllChildren<StyleParagraphProperties>();
                        heading1Style.RemoveAllChildren<StyleRunProperties>();
                        heading1Style.AppendChild(h1.GetRunProperties());
                        heading1Style.AppendChild(h1.GetParagraphProperties());
                        Console.WriteLine("Style 'Heading1' modified successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Style 'Heading1' not found.");
                    }
                }
            }
            doc.Save();
        }
    }

    public static void ChangeHeading2()
    {
        string filePath = "../../../data/Заголовок первого уровня.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {

            doc.Save();
        }
    }

    public static void ChangeHeading3()
    {
        string filePath = "../../../data/Заголовок первого уровня.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {

        }
    }

    public static void ChangeHeading4()
    {
        string filePath = "../../../data/test.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
           
        }
    }

    public static void ChangeHeading5()
    {
        string filePath = "../../../data/test.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {

        }
    }

    public static void ChangeNormal()
    {
        string filePath = "../../../data/temp.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            StyleDefinitionsPart stylePart = doc.MainDocumentPart.StyleDefinitionsPart;

            if (stylePart != null)
            {
                Styles styles = stylePart.Styles;

                if (styles != null)
                {
                    // Найдем стиль "Heading1"
                    Style normalStyle = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "a");

                    if (normalStyle != null)
                    {
                        Normal normal = GetNormal();
                        // Удаляем все свойства стиля
                        normalStyle.RemoveAllChildren<StyleParagraphProperties>();
                        normalStyle.RemoveAllChildren<StyleRunProperties>();
                        // изменяем свойства
                        normalStyle.AppendChild(normal.GetRunProperties());
                        normalStyle.AppendChild(normal.GetParagraphProperties());
                        Console.WriteLine("Style 'Normal' modified successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Style 'Normal' not found.");
                    }
                }
            }
            doc.Save();
        }
    }

    public static void ChangeListItem()
    {
        // не учитывается немаркированный список
        string filePath = "../../../data/test.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {

        }
    }

    public static void ChangeImage()
    {
        string filePath = "../../../data/temp.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            Image imageStyle = GetImage();
            var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().ToList();
            for (int i = 0; i < paragraphs.Count; i++)
            {
                var drawings = paragraphs[i].Descendants<Drawing>().ToList();
                if (drawings.Any())
                {
                    paragraphs[i].ParagraphProperties = imageStyle.GetParagraphProperties();
                    if (IsImageSignature && i + 1 < paragraphs.Count)
                    {
                        // по-хорошему, надо добавить проверку на то, что следующий параграф - подпись к рисунку как-нибудь (например, по шаблону)
                        // нумерация рисунков, шаблок подписи регуляркой
                        ImageSignature imageSignatureStyle = GetImageSignature();
                        paragraphs[i + 1].ParagraphProperties = imageSignatureStyle.GetParagraphProperties();
                        paragraphs[i + 1].Descendants<Run>().ToList().ForEach(x => x.RunProperties = imageSignatureStyle.GetRunProperties());
                    }
                }
            }
            Console.WriteLine("ok");
            doc.Save();
        }
    }

    // с таблицей вообще страшно работать

    public static void ChangeTableSignature()
    {
        string filePath = "../../../data/test.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            StyleDefinitionsPart stylePart = doc.MainDocumentPart.StyleDefinitionsPart;

            if (stylePart != null)
            {
                Styles styles = stylePart.Styles;

                if (styles != null)
                {
                    // Создаем новый стиль "Caption" для подписей к рисункам
                    Style captionStyle = new Style()
                    {
                        Type = StyleValues.Paragraph,
                        StyleId = "TableSignature",
                        CustomStyle = true
                    };

                    // Добавляем новый стиль в коллекцию
                    //styles.AppendChild(captionStyle);

                    captionStyle.AppendChild(new StyleName() { Val = "TableSignature" });
                    captionStyle.AppendChild(new Name() { Val = "TableSignature" });
                    captionStyle.AppendChild(new BasedOn() { Val = "Normal" });
                    captionStyle.AppendChild(new NextParagraphStyle() { Val = "NormalTable" });
                    captionStyle.AppendChild(new UIPriority() { Val = 11 });

                    captionStyle.AppendChild(
                        new StyleParagraphProperties(
                            new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "120", After = "0" },
                            new Indentation { Left = "0", Right = "0", FirstLine = "0" },
                            new Justification { Val = JustificationValues.Left }
                        )
                    );

                    captionStyle.AppendChild(
                        new StyleRunProperties(
                            new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new Color { Val = "000000" },
                            new Bold { Val = false },
                            new Italic { Val = false },
                            new Underline { Val = UnderlineValues.None },
                            new FontSize { Val = "26" }
                        )
                    );
                    styles.AppendChild(captionStyle);

                    Console.WriteLine("Style 'TableSignature' created successfully.");
                }
                stylePart.Styles.Save();
            }
        }
    }

    public static void GetProperty()
    {
        string filePath = "../../../data/temp.docx";

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
                        if (style.StyleName is not null)
                        {
                            Console.WriteLine($"Style Name: {style.StyleName.Val}");
                            Console.WriteLine($"Based On: {style.Descendants<BasedOn>().FirstOrDefault()?.Val}");
                            Console.WriteLine($"Next Paragraph Style: {style.Descendants<NextParagraphStyle>().FirstOrDefault()?.Val}");
                            Console.WriteLine($"UI Priority: {style.Descendants<UIPriority>().FirstOrDefault()?.Val}");
                        }

                        Console.WriteLine();
                    }
                }
            }
        }
    }

    public static Heading GetHeading1()
    {
        return new Heading("Times New Roman", new Color() { Val = "000" },
        true, false, UnderlineValues.None, "32", "240", "0", "240", JustificationValues.Both, true, true, 6, 0, 0, 0, 0, true);
    }

    public static Normal GetNormal()
    {
        return new Normal("Times New Roman", new Color() { Val = "000" },
        false, false, UnderlineValues.None, "26", "360", "0", "0", JustificationValues.Both, 0, 0, 710);
    }

    public static Image GetImage()
    {
        return new Image("240", "120", "0", JustificationValues.Center, 0, 0, 0, IsImageSignature);
    }

    public static ImageSignature GetImageSignature()
    {
        return new ImageSignature("Times New Roman", new Color() { Val = "000" },
        true, true, UnderlineValues.None, "24", "240", "0", "120", JustificationValues.Center, 0, 0, 0);
    }

    private static void Main(string[] args)
    {
        //GetProperty();
        //ChangeHeading1();
        //ChangeHeading2();
        //ChangeHeading3();
        //ChangeHeading4();
        //ChangeHeading5();
        //ChangeNormal();
        //ChangeListItem();
        //ChangeImage();
        //GetProperty();
    }
}