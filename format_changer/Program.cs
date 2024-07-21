using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using format_changer.Models;
using System.Text.RegularExpressions;

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
                        HeadingSettings h1 = GetHeading1();
                        heading1Style.RemoveAllChildren<StyleParagraphProperties>();
                        heading1Style.RemoveAllChildren<StyleRunProperties>();
                        heading1Style.AppendChild(h1.GetRunProperties());
                        heading1Style.AppendChild(h1.GetParagraphProperties());
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
        // tab/пробел сохраняются
        string filePath = "../../../data/temp — копия.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            StyleDefinitionsPart stylePart = doc.MainDocumentPart.StyleDefinitionsPart;

            if (stylePart != null)
            {
                Styles styles = stylePart.Styles;

                if (styles != null)
                {
                    Style heading2Style = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "2");

                    if (heading2Style != null)
                    {
                        HeadingSettings heading2 = GetHeading2();
                        heading2Style.RemoveAllChildren<StyleParagraphProperties>();
                        heading2Style.RemoveAllChildren<StyleRunProperties>();
                        heading2Style.AppendChild(heading2.GetRunProperties());
                        heading2Style.AppendChild(heading2.GetParagraphProperties());
                    }
                    else
                    {
                        Console.WriteLine("Style 'Heading2' not found.");
                    }
                }
            }
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
                        NormalSettings normal = GetNormal();
                        // Удаляем все свойства стиля
                        normalStyle.RemoveAllChildren<StyleParagraphProperties>();
                        normalStyle.RemoveAllChildren<StyleRunProperties>();
                        // изменяем свойства
                        normalStyle.AppendChild(normal.GetRunProperties());
                        normalStyle.AppendChild(normal.GetParagraphProperties());
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
        string filePath = "../../../data/temp — копия.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            ImageSettings imageStyle = GetImage();
            ImageSignatureSettings imageSignatureStyle = GetImageSignature();
            int imageIndex = 1;
            var paragraphs = doc.MainDocumentPart?.Document?.Body?.Descendants<Paragraph>().ToList();
            for (int i = 0; i < paragraphs.Count; i++)
            {
                var drawings = paragraphs[i].Descendants<Drawing>().ToList();
                if (drawings.Count > 0)
                {
                    paragraphs[i].ParagraphProperties = imageStyle.GetParagraphProperties();
                    bool isNextImageSignature = i + 1 < paragraphs.Count && true; // тут должна быть доп проверка на то, что следующий параграф - подпись к рисунку
                    if (IsImageSignature && isNextImageSignature)
                    {
                        var signatureParagraph = paragraphs[i + 1];
                        if (signatureParagraph != null)
                        {
                            if (!Regex.IsMatch(signatureParagraph.InnerText, imageSignatureStyle.SignatureTemplatePattern))
                            {
                                // добавить проверку на то, что подпись есть частично, чтобы было например "Рисунок 1 - Рисунок 1: Название рисунка"
                                string signature = $"Рисунок {imageIndex++} – {signatureParagraph.InnerText}";
                                signatureParagraph.RemoveAllChildren<Run>();
                                signatureParagraph.AppendChild<Run>(new Run(new Text(signature)));
                            }

                            signatureParagraph.ParagraphProperties = imageSignatureStyle.GetParagraphProperties();
                            signatureParagraph.Descendants<Run>().ToList().ForEach(x => x.RunProperties = imageSignatureStyle.GetRunProperties());
                        }
                    }
                }
            }
            doc.Save();
        }
    }

    /// <summary>
    /// Adds a table signature above the specified table in the Word document.
    /// The paragraph preceding the table is formatted as the table caption.
    /// </summary>
    /// <param name="doc">The WordprocessingDocument where the table is located</param>
    /// <param name="table">The table to which the signature will be added</param>
    public static void AddTableSignature(WordprocessingDocument doc, Table table, int tablesIndex)
    {
        TableSignatureSettings tableSignatureSettings = GetTableSignature();
        var tableIndex = doc.MainDocumentPart?.Document?.Body?.Elements().ToList().IndexOf(table);

        Paragraph nextParagraph = doc.MainDocumentPart?.Document?.Body.Elements().Skip((int)(tableIndex - 1)).OfType<Paragraph>().FirstOrDefault();

        if (nextParagraph != null)
        {
            if (!Regex.IsMatch(nextParagraph.InnerText, tableSignatureSettings.SignatureTemplatePattern))
            {
                // добавить проверку на то, что подпись есть частично, чтобы не было например "Таблица 1 - Таблица 1: Название таблицы"
                string signature = $"Таблица {tablesIndex++} – {nextParagraph.InnerText}";
                nextParagraph.RemoveAllChildren<Run>();
                nextParagraph.AppendChild<Run>(new Run(new Text(signature)));
            }

            nextParagraph.ParagraphProperties = tableSignatureSettings.GetParagraphProperties();

            nextParagraph.Descendants<Run>().ToList().ForEach(x => x.RunProperties = tableSignatureSettings.GetRunProperties());
        }
    }

    /// <summary>
    /// Adds spacing before the specified table by adjusting the after spacing of the preceding paragraph
    /// </summary>
    /// <param name="doc">The WordprocessingDocument where the table is located</param>
    /// <param name="table">The table to insert spacing before</param>
    /// <param name="tableStyle">The table settings to determine the spacing</param>
    public static void AddSpacingBeforeTable(WordprocessingDocument doc, Table table, TableSettings tableStyle)
    {
        var tableIndex = doc.MainDocumentPart?.Document?.Body?.Elements().ToList().IndexOf(table);

        Paragraph nextParagraph = doc.MainDocumentPart?.Document?.Body.Elements().Skip((int)(tableIndex - 1)).OfType<Paragraph>().FirstOrDefault();

        if (nextParagraph != null)
        {
            ParagraphProperties paragraphProperties;
            if (nextParagraph.ParagraphProperties == null)
            {
                paragraphProperties = new ParagraphProperties();
            }
            else
            {
                paragraphProperties = (ParagraphProperties)nextParagraph.ParagraphProperties.Clone();
            }
            paragraphProperties.SpacingBetweenLines = new SpacingBetweenLines { After = tableStyle.BeforeSpacing.ToString() }; // adjust the after spacing value as needed
            nextParagraph.ParagraphProperties = paragraphProperties;
        }
    }

    /// <summary>
    /// Adds spacing after the specified table by adjusting the before spacing of the next paragraph
    /// </summary>
    /// <param name="doc">The WordprocessingDocument where the table is located</param>
    /// <param name="table">The table to insert spacing after</param>
    /// <param name="tableStyle">The table settings to determine the spacing</param>
    public static void AddSpacingAfterTable(WordprocessingDocument doc, Table table, TableSettings tableStyle)
    {
        var tableIndex = doc.MainDocumentPart?.Document?.Body?.Elements().ToList().IndexOf(table);

        Paragraph nextParagraph = doc.MainDocumentPart?.Document?.Body.Elements().Skip((int)(tableIndex + 1)).OfType<Paragraph>().FirstOrDefault();

        if (nextParagraph != null)
        {
            ParagraphProperties paragraphProperties;
            if (nextParagraph.ParagraphProperties == null)
            {
                paragraphProperties = new ParagraphProperties();
            }
            else
            {
                paragraphProperties = (ParagraphProperties)nextParagraph.ParagraphProperties.Clone();
            }
            paragraphProperties.SpacingBetweenLines = new SpacingBetweenLines { Before = tableStyle.AfterSpacing.ToString() }; // adjust the after spacing value as needed
            nextParagraph.ParagraphProperties = paragraphProperties;
        }
    }

    public static void ChangeTable()
    {
        string filePath = "../../../data/temp — копия.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            var tables = doc.MainDocumentPart?.Document?.Body?.Descendants<Table>().ToList();
            for (int i = 0; i < tables.Count; i++)
            {
                TableSettings tableStyle = GetTable();
                TableCellsSettings tableCellsStyle = GetTableCells();
                if (tableStyle.BeforeSpacing != 0)
                {
                    AddSpacingBeforeTable(doc, tables[i], tableStyle);
                }

                if (tableStyle.AfterSpacing != 0)
                {
                    AddSpacingAfterTable(doc, tables[i], tableStyle);
                }

                if (tableStyle.IsTableSignature)
                {
                    AddTableSignature(doc, tables[i], i + 1);
                }

                foreach (TableCell cell in tables[i].Elements<TableRow>().SelectMany(row => row.Elements<TableCell>()))
                {
                    cell.Append(new TableCellProperties(
                        new TableCellMargin(
                            new TopMargin { Width = tableCellsStyle.TopMargin.ToString() },
                            new BottomMargin { Width = tableCellsStyle.BottomMargin.ToString() },
                            new LeftMargin { Width = tableCellsStyle.LeftMargin.ToString() },
                            new RightMargin { Width = tableCellsStyle.RightMargin.ToString() }
                        ),
                        new TableCellVerticalAlignment { Val = tableCellsStyle.GetVerticalAlignment().Val }
                    ));

                    foreach (Paragraph paragraph in cell.Elements<Paragraph>())
                    {
                        paragraph.ParagraphProperties = tableCellsStyle.GetParagraphProperties();

                        foreach (Run run in paragraph.Elements<Run>())
                        {
                            run.RunProperties = tableCellsStyle.GetRunProperties();
                        }
                    }
                }

                if (tableStyle.IsHeading)
                {
                    TableHeadingSettings tableHeadingStyle = GetTableHeading();
                    var headingRow = tables[i].Elements<TableRow>().FirstOrDefault();

                    foreach (var cell in headingRow.Elements<TableCell>())
                    {
                        foreach (var paragraph in cell.Elements<Paragraph>())
                        {
                            paragraph.ParagraphProperties = tableHeadingStyle.GetParagraphProperties();

                            foreach (var run in paragraph.Elements<Run>())
                            {
                                run.RunProperties = tableHeadingStyle.GetRunProperties();
                            }
                        }
                    }
                }
            }
            doc.Save();
        }
    }

    public static void AddPageNumbering()
    {
        string filePath = "../../../data/temp — копия.docx";

        using (WordprocessingDocument doc = WordprocessingDocument.Open(filePath, true))
        {
            FooterPart footerPart = doc.MainDocumentPart.AddNewPart<FooterPart>();
            string footerPartId = doc.MainDocumentPart.GetIdOfPart(footerPart);

            Footer footer = new Footer(new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId() { Val = "Footer" },
                    new Justification() { Val = JustificationValues.Center },
                    new SpacingBetweenLines() { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto },
                    new Indentation { Left = "0", Right = "0", FirstLine = "0" }
                ),
                new Run(
                    new SimpleField() { Instruction = "PAGE" })));

            footerPart.Footer = footer;

            IEnumerable<SectionProperties> sectionProperties = doc.MainDocumentPart.Document.Body.Elements<SectionProperties>();

            foreach (var sectionProperty in sectionProperties)
            {
                sectionProperty.RemoveAllChildren<FooterReference>();
                sectionProperty.PrependChild(new FooterReference()
                {
                    Id = footerPartId
                });
            }

            doc.Save();
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

    public static HeadingSettings GetHeading1()
    {
        return new HeadingSettings("Times New Roman", new Color() { Val = "000" },
        true, false, UnderlineValues.None, "32", "240", "0", "240", JustificationValues.Both, true, true, 6, 0, 0, 0, 0, true);
    }

    public static HeadingSettings GetHeading2()
    {
        return new HeadingSettings("Times New Roman", new Color() { Val = "000" },
        true, false, UnderlineValues.None, "28", "240", "240", "120", JustificationValues.Both, false, true, 6, 1, 0, 0, 0, true);
    }

    public static NormalSettings GetNormal()
    {
        return new NormalSettings("Times New Roman", new Color() { Val = "000" },
        false, false, UnderlineValues.None, "26", "360", "0", "0", JustificationValues.Both, 0, 0, (int)Math.Round(1.25f * TWIPS / INCH / 10.0) * 10);
    }

    public static ListSettings GetList()
    {
        return new ListSettings("Times New Roman", new Color() { Val = "000" },
        false, false, UnderlineValues.None, "26", "360", "0", "0", JustificationValues.Both, true, 1, 0, 710, 0, 855);
    }

    public static ImageSettings GetImage()
    {
        return new ImageSettings("240", "120", "0", JustificationValues.Center, 0, 0, 0, IsImageSignature);
    }

    public static ImageSignatureSettings GetImageSignature()
    {
        return new ImageSignatureSettings("Times New Roman", new Color() { Val = "000" },
        true, true, UnderlineValues.None, "24", "240", "0", "120", JustificationValues.Center, 0, 0, 0, @"^Рисунок \d+ – .*");
    }

    public static TableSettings GetTable()
    {
        return new TableSettings(true, true, "0", "120");
    }

    public static TableHeadingSettings GetTableHeading()
    {
        return new TableHeadingSettings("Times New Roman", new Color() { Val = "000" },
        true, false, UnderlineValues.None, "24", "240", "0", "0", JustificationValues.Center, false, false, 6, 0, 0, 0, 0, true);
    }

    public static TableCellsSettings GetTableCells()
    {
        return new TableCellsSettings("Times New Roman", new Color() { Val = "000" },
        false, false, UnderlineValues.None, "24", "240", "0", "0", JustificationValues.Left, TableVerticalAlignmentValues.Center, 0, 0, 0, 55, 55, 55, 55);
    }

    public static TableSignatureSettings GetTableSignature()
    {
        return new TableSignatureSettings("Times New Roman", new Color() { Val = "000" },
        false, false, UnderlineValues.None, "26", "240", "120", "0", JustificationValues.Both, 0, 0, 0, true, @"^Таблица \d+ – .*");
    }

    private static void Main(string[] args)
    {
        //GetProperty();
        //ChangeHeading1();
        ChangeHeading2();
        //ChangeHeading3();
        //ChangeHeading4();
        //ChangeHeading5();
        //ChangeNormal();
        //ChangeList();
        //ChangeImage();
        //GetProperty();
        //ChangeTable();
        //AddPageNumbering();
    }
}