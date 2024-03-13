using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

public class Program
{
    public static void ChangeHeading1()
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
                                new Justification { Val = JustificationValues.Center },
                                new PageBreakBefore(),
                                new Tabs(
                                    new TabStop() { Val = TabStopValues.Left, Position = 360, Leader = TabStopLeaderCharValues.None }),
                                new NumberingProperties
                                {
                                    NumberingId = new NumberingId() { Val = 6 },
                                    NumberingLevelReference = new NumberingLevelReference() { Val = 0 },
                                    
                                },
                                new NumberingFormat { Format = "decimal" })
                            );
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
        }
    }

    public static void ChangeHeading2()
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
                    // Найдем стиль "Heading1"
                    Style heading1Style = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "2");

                    if (heading1Style != null)
                    {
                        // Изменяем свойства стиля
                        heading1Style.Descendants<Name>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Italic>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Bold>().FirstOrDefault()?.Remove();
                        heading1Style.AppendChild(
                            new StyleParagraphProperties(
                                new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "240", After = "120" },
                                new Indentation { Left = "0", Right = "0", FirstLine = "0" },
                                new Justification { Val = JustificationValues.Center },
                                new Tabs(
                                    new TabStop() { Val = TabStopValues.Left, Position = 360, Leader = TabStopLeaderCharValues.None }),
                                new NumberingProperties
                                {
                                    NumberingId = new NumberingId() { Val = 6 },
                                    NumberingLevelReference = new NumberingLevelReference() { Val = 1 }
                                },
                                new NumberingFormat { Format = NumberFormatValues.Decimal.ToString() }
                            ));
                        heading1Style.AppendChild(
                            new StyleRunProperties(
                                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new Color { Val = "000000" },
                                new Bold { Val = true },
                                new Italic { Val = false },
                                new Underline { Val = UnderlineValues.None },
                                new FontSize { Val = "28" }));
                        Console.WriteLine("Style 'Heading1' modified successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Style 'Heading1' not found.");
                    }
                }
            }
        }
    }

    public static void ChangeHeading3()
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
                    // Найдем стиль "Heading1"
                    Style heading1Style = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "3");

                    if (heading1Style != null)
                    {
                        // Изменяем свойства стиля
                        heading1Style.Descendants<Name>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Italic>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Bold>().FirstOrDefault()?.Remove();
                        heading1Style.AppendChild(
                            new StyleParagraphProperties(
                                new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "160", After = "80" },
                                new Indentation { Left = "0", Right = "0", FirstLine = "0" },
                                new Justification { Val = JustificationValues.Center },
                                new Tabs(
                                    new TabStop() { Val = TabStopValues.Left, Position = 360, Leader = TabStopLeaderCharValues.None }),
                                new NumberingProperties
                                {
                                    NumberingId = new NumberingId() { Val = 6 },
                                    NumberingLevelReference = new NumberingLevelReference() { Val = 2 }
                                },
                                new NumberingFormat { Format = NumberFormatValues.Decimal.ToString() }
                            ));
                        heading1Style.AppendChild(
                            new StyleRunProperties(
                                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new Color { Val = "000000" },
                                new Bold { Val = true },
                                new Italic { Val = false },
                                new Underline { Val = UnderlineValues.None },
                                new FontSize { Val = "26" }));
                        Console.WriteLine("Style 'Heading1' modified successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Style 'Heading1' not found.");
                    }
                }
            }
        }
    }

    public static void ChangeHeading4()
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
                    // Найдем стиль "Heading1"
                    Style heading1Style = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "4");

                    if (heading1Style != null)
                    {
                        // Изменяем свойства стиля
                        heading1Style.Descendants<Name>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Italic>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Bold>().FirstOrDefault()?.Remove();
                        heading1Style.AppendChild(
                            new StyleParagraphProperties(
                                new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "160", After = "80" },
                                new Indentation { Left = "0", Right = "0", FirstLine = "0" },
                                new Justification { Val = JustificationValues.Center },
                                new Tabs(
                                    new TabStop() { Val = TabStopValues.Left, Position = 360, Leader = TabStopLeaderCharValues.None }),
                                new NumberingProperties
                                {
                                    NumberingId = new NumberingId() { Val = 6 },
                                    NumberingLevelReference = new NumberingLevelReference() { Val = 3 }
                                },
                                new NumberingFormat { Format = NumberFormatValues.Decimal.ToString() }
                            ));
                        heading1Style.AppendChild(
                            new StyleRunProperties(
                                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new Color { Val = "000000" },
                                new Bold { Val = true },
                                new Italic { Val = false },
                                new Underline { Val = UnderlineValues.None },
                                new FontSize { Val = "26" }));
                        Console.WriteLine("Style 'Heading1' modified successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Style 'Heading1' not found.");
                    }
                }
            }
        }
    }

    public static void ChangeHeading5()
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
                    // Найдем стиль "Heading1"
                    Style heading1Style = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "5");

                    if (heading1Style != null)
                    {
                        // Изменяем свойства стиля
                        heading1Style.Descendants<Name>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Italic>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Bold>().FirstOrDefault()?.Remove();
                        heading1Style.AppendChild(
                            new StyleParagraphProperties(
                                new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "160", After = "80" },
                                new Indentation { Left = "0", Right = "0", FirstLine = "0" },
                                new Justification { Val = JustificationValues.Center },
                                new Tabs(
                                    new TabStop() { Val = TabStopValues.Left, Position = 360, Leader = TabStopLeaderCharValues.None }),
                                new NumberingProperties
                                {
                                    NumberingId = new NumberingId() { Val = 6 },
                                    NumberingLevelReference = new NumberingLevelReference() { Val = 4 }
                                },
                                new NumberingFormat { Format = NumberFormatValues.Decimal.ToString() }
                            ));
                        heading1Style.AppendChild(
                            new StyleRunProperties(
                                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new Color { Val = "000000" },
                                new Bold { Val = true },
                                new Italic { Val = false },
                                new Underline { Val = UnderlineValues.None },
                                new FontSize { Val = "26" }));
                        Console.WriteLine("Style 'Heading1' modified successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Style 'Heading1' not found.");
                    }
                }
            }
        }
    }

    public static void ChangeNormal()
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
                    // Найдем стиль "Heading1"
                    Style heading1Style = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "a");

                    if (heading1Style != null)
                    {
                        // Изменяем свойства стиля
                        heading1Style.Descendants<Name>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Italic>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Bold>().FirstOrDefault()?.Remove();
                        heading1Style.AppendChild(
                            new StyleParagraphProperties(
                                new SpacingBetweenLines { Line = "360", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" },
                                new Indentation { Left = "0", Right = "0", FirstLine = "710" },
                                new Justification { Val = JustificationValues.Both }
                            ));
                        heading1Style.AppendChild(
                            new StyleRunProperties(
                                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new Color { Val = "000000" },
                                new Bold { Val = false },
                                new Italic { Val = false },
                                new Underline { Val = UnderlineValues.None },
                                new FontSize { Val = "26" }));
                        Console.WriteLine("Style 'Heading1' modified successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Style 'Heading1' not found.");
                    }
                }
            }
        }
    }

    public static void ChangeListItem()
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
                    // Найдем стиль "Heading1"
                    Style heading1Style = styles.Elements<Style>().FirstOrDefault(style => style.StyleId == "-1");

                    if (heading1Style != null)
                    {
                        // Изменяем свойства стиля
                        heading1Style.Descendants<Name>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Italic>().FirstOrDefault()?.Remove();
                        heading1Style.Descendants<Bold>().FirstOrDefault()?.Remove();
                        heading1Style.AppendChild(
                            new StyleParagraphProperties(
                                new SpacingBetweenLines { Line = "360", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" },
                                new Indentation { Left = "850", Right = "0", FirstLine = "285" },
                                new Justification { Val = JustificationValues.Both },
                                new Tabs(
                                    new TabStop() { Val = TabStopValues.Left, Position = 360, Leader = TabStopLeaderCharValues.None }),
                                new NumberingProperties
                                {
                                    NumberingId = new NumberingId() { Val = 1 },
                                    NumberingLevelReference = new NumberingLevelReference() { Val = 3 }
                                },
                                new NumberingFormat { Format = NumberFormatValues.Decimal.ToString() }
                            ));
                        heading1Style.AppendChild(
                            new StyleRunProperties(
                                new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                                new Color { Val = "000000" },
                                new Bold { Val = false },
                                new Italic { Val = false },
                                new Underline { Val = UnderlineValues.None },
                                new FontSize { Val = "26" }));
                        Console.WriteLine("Style 'Heading1' modified successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Style 'Heading1' not found.");
                    }
                }
            }
        }
    }

    public static void ChangeImage()
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
                        StyleId = "Caption",
                        CustomStyle = true
                    };

                    // Добавляем новый стиль в коллекцию

                    captionStyle.AppendChild(new StyleName() { Val = "Caption" });
                    captionStyle.AppendChild(new Name() { Val = "Caption" });
                    captionStyle.AppendChild(new BasedOn() { Val = "Normal" });
                    captionStyle.AppendChild(new NextParagraphStyle() { Val = "CaptionSignature" });
                    captionStyle.AppendChild(new UIPriority() { Val = 10 });

                    captionStyle.AppendChild(
                        new StyleParagraphProperties(
                            new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "120", After = "0" },
                            new Indentation { Left = "0", Right = "0", FirstLine = "0" },
                            new Justification { Val = JustificationValues.Center },
                            new NumberingProperties(
                                new NumberingLevelReference { Val = 0 },
                                new NumberingId { Val = 1 }
                            ),
                            new KeepNext()
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
                    //captionStyle.AppendChild(
                    //    new LevelText { Val = "Рисунок \u2002- " }
                    //);

                    Console.WriteLine("Style 'Caption' created successfully.");
                }
            }
            var paragraphs = doc.MainDocumentPart.Document.Body.Elements<Paragraph>().ToList();
            var image = paragraphs[2];

            // Создаем новый элемент Paragraph с применением стиля "Caption"
            var newImageParagraph = new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "Caption" }));

            // Копируем все ранее существующие элементы из старого параграфа в новый
            foreach (var element in image.Elements())
            {
                newImageParagraph.AppendChild(element.CloneNode(true));
            }

            // Заменяем старый параграф на новый в коллекции
            image.InsertAfterSelf(newImageParagraph);
            image.Remove();

        }
    }

    public static void ChangeImageSignature()
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
                        StyleId = "CaptionSignature",
                        CustomStyle = true
                    };

                    // Добавляем новый стиль в коллекцию
                    //styles.AppendChild(captionStyle);

                    captionStyle.AppendChild(new StyleName() { Val = "CaptionSignature" });
                    captionStyle.AppendChild(new Name() { Val = "CaptionSignature" });
                    captionStyle.AppendChild(new BasedOn() { Val = "Normal" });
                    captionStyle.AppendChild(new NextParagraphStyle() { Val = "Normal" });
                    captionStyle.AppendChild(new UIPriority() { Val = 11 });

                    captionStyle.AppendChild(
                        new StyleParagraphProperties(
                            new SpacingBetweenLines { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "120" },
                            new Indentation { Left = "0", Right = "0", FirstLine = "0" },
                            new Justification { Val = JustificationValues.Center },
                            new NumberingProperties(
                                new NumberingLevelReference { Val = 0 },
                                new NumberingId { Val = 1 }
                            )
                        )
                    );

                    captionStyle.AppendChild(
                        new StyleRunProperties(
                            new RunFonts { Ascii = "Times New Roman", HighAnsi = "Times New Roman" },
                            new Color { Val = "000000" },
                            new Bold { Val = true },
                            new Italic { Val = true },
                            new Underline { Val = UnderlineValues.None },
                            new FontSize { Val = "24" }
                        )
                    );

                    captionStyle.AppendChild(
                        new NumberingProperties(
                            new NumberingLevelReference { Val = 0 },
                            new NumberingId { Val = 1 }
                        )
                    );
                    captionStyle.AppendChild(
                        new NumberingFormat { Format = NumberFormatValues.Decimal.ToString() }
                    );
                    styles.AppendChild(captionStyle);
                    //captionStyle.AppendChild(
                    //    new LevelText { Val = "Рисунок \u2002- " }
                    //);

                    Console.WriteLine("Style 'Caption' created successfully.");
                }
                stylePart.Styles.Save();
            }
        }
    }

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
        string filePath = "../../../data/test.docx";

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
                        // Вывод остальных свойств...

                        Console.WriteLine();
                    }
                }
            }
        }
    }

    private static void Main(string[] args)
    {
        //GetProperty();
        ChangeHeading1();
        //ChangeHeading2();
        //ChangeHeading3();
        //ChangeHeading4();
        //ChangeHeading5();
        //ChangeNormal();
        //ChangeListItem();
        //ChangeImage();
        //ChangeImageSignature();
        //GetProperty();


        // когда на заголовке первого уровня уже была нумерация - ничего не менялось вообще
    }
}