using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using format_changer.Models;

public class Program
{
    public static bool IsImageSignature = true;
    public const float INCH = 2.5399978f;
    public const int TWIPS = 1440;
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

    public static void ChangeList()
    {
        // регистр первого символа
        // в списках может быть разная настройка закрепов, у нас например 1, 2 и предпоследний
        // маркер
        // добавить выбор формата номера в зависимости конца пункта (точка/точка с запятой) + арабская/римская.
        // вложенность
        // сейчас номер стиля выбран из того, какой стоит в документе, но в кадлом документе могут быть свои настройки,
        // так что нужно искать номер стиля
        // маркированные списки пока делаются просто текстом
        string filePath = "../../../data/temp.docx";
        int targetListStyleId = 35;

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            MainDocumentPart mainPart = doc.MainDocumentPart;

            NumberingDefinitionsPart numberingPart;
            if (mainPart.NumberingDefinitionsPart == null)
            {
                numberingPart = mainPart.AddNewPart<NumberingDefinitionsPart>();
                numberingPart.Numbering = new Numbering();
            }
            else
            {
                numberingPart = mainPart.NumberingDefinitionsPart;
            }

            Numbering numbering = numberingPart.Numbering;
            AbstractNum abstractNum = new AbstractNum() { AbstractNumberId = targetListStyleId };

            Level level = new Level()
            {
                LevelIndex = 0,
                StartNumberingValue = new StartNumberingValue() { Val = 1 },
                NumberingFormat = new NumberingFormat() { Val = NumberFormatValues.Decimal },
                LevelText = new LevelText() { Val = "%1)" }
            };

            abstractNum.Append(level);
            numbering.Append(abstractNum);

            NumberingInstance numberingInstance = new NumberingInstance() { NumberID = targetListStyleId };
            numberingInstance.Append(new AbstractNumId() { Val = targetListStyleId });
            numbering.Append(numberingInstance);

            var list = GetList();
            var runProperties = list.GetRunProperties();
            foreach (var paragraph in mainPart.Document.Body.Descendants<Paragraph>())
            {
                if (paragraph.ParagraphProperties != null && paragraph.ParagraphProperties.NumberingProperties != null)
                {
                    paragraph.ParagraphProperties.NumberingProperties.Remove();
                    paragraph.ParagraphProperties = new ParagraphProperties();
                    paragraph.ParagraphProperties.NumberingProperties = new NumberingProperties(
                        new NumberingId() { Val = targetListStyleId },
                        new NumberingLevelReference() { Val = 0 }
                    );
                    paragraph.ParagraphProperties.Indentation = new Indentation() { Left = "850", FirstLine = "285" };
                    paragraph.ParagraphProperties.SpacingBetweenLines = new SpacingBetweenLines() { After = "0", Line = "360", LineRule = LineSpacingRuleValues.Auto };
                    foreach (var run in paragraph.Descendants<Run>())
                    {
                        run.RunProperties = (RunProperties)runProperties.Clone();
                    }
                }
            }
            doc.Save();
        }
    }
    public static void GetListStyles()
    {
        string filePath = "../../../data/temp.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            MainDocumentPart mainPart = doc.MainDocumentPart;
            NumberingDefinitionsPart numberingPart = mainPart.NumberingDefinitionsPart;

            foreach (NumberingInstance numberingInstance in numberingPart.Numbering.Elements<NumberingInstance>())
            {
                AbstractNum abstractNum = numberingPart.Numbering.Elements<AbstractNum>().FirstOrDefault(an => an.AbstractNumberId == numberingInstance.AbstractNumId.Val);

                if (abstractNum != null)
                {
                    if (IsNumericList(abstractNum))
                    {
                        Console.WriteLine($"List style ID: {numberingInstance.AbstractNumId.Val}");
                        Console.WriteLine($"Marker: {GetMarker(abstractNum)}");
                    }
                }
            }
        }
    }

    static bool IsNumericList(AbstractNum abstractNum)
    {
        return abstractNum?.GetFirstChild<Level>()?.GetFirstChild<NumberingFormat>().Val == NumberFormatValues.Decimal;
    }

    static string GetMarker(AbstractNum abstractNum)
    {
        Level level = abstractNum?.GetFirstChild<Level>();
        if (level != null)
        {
            return level?.GetFirstChild<LevelText>().Val;
        }
        return string.Empty;
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
                        // нумерация рисунков, шаблон подписи регуляркой
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
        false, false, UnderlineValues.None, "26", "360", "0", "0", JustificationValues.Both, 0, 0, (int)Math.Round(1.25f * TWIPS / INCH / 10.0) * 10);
    }

    public static List GetList()
    {
        return new List("Times New Roman", new Color() { Val = "000" },
        false, false, UnderlineValues.None, "26", "360", "0", "0", JustificationValues.Both, true, 1, 0, 710, 0, 855);
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
        //ChangeList();
        //ChangeImage();
        //GetProperty();
    }
}