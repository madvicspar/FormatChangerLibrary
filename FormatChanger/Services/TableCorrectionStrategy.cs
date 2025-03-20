using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
    public class TableCorrectionStrategy : IElementCorrectionStrategy<TableSettingsModel>
    {
        public TableSettingsModel GetSettings(FormattingTemplateModel template)
        {
            return template.TableSettings;
        }
        public RunProperties GetRunProperties(TableSettingsModel settings)
        {
            return new RunProperties();
        }

        public ParagraphProperties GetParagraphProperties(TableSettingsModel settings)
        {
            return new ParagraphProperties();
        }

        public void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            // TODO: выравнивание по ширине окна и по содержимому
            var settings = GetSettings(template);
            var tables = doc.MainDocumentPart?.Document?.Body?.Descendants<Table>().ToList();
            if (tables == null) return;

            foreach (var table in tables)
            {
                ApplyTableProperties(table, settings);
                //ApplyCellProperties(table, settings);
            }
        }

        public void ApplyTableProperties(Table table, TableSettingsModel settings)
        {
            // Устанавливаем интервал до и после таблицы


            ApplyCellMarginDefault(table, settings);
        }

        public void ApplyCellMarginDefault(Table table, TableSettingsModel settings)
        {
            // TODO: в бд хранить все в dxa, реализовать перевод, в интерфейсе - пт и см
            var tableProperties = table.Elements<TableProperties>().FirstOrDefault();

            if (tableProperties == null)
            {
                tableProperties = new TableProperties();
                table.PrependChild(tableProperties);
            }

            tableProperties.RemoveAllChildren<TableCellMarginDefault>();
            tableProperties.RemoveAllChildren<TableCellVerticalAlignment>();
            tableProperties.RemoveAllChildren<Justification>();

            var tableCellMargins = new TableCellMarginDefault(
                new LeftMargin { Width = (settings.CellSettings.LeftPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                new RightMargin { Width = (settings.CellSettings.RightPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                new TopMargin { Width = (settings.CellSettings.TopPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = (settings.CellSettings.BottomPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa }
            );

            tableProperties.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            tableProperties.AppendChild(new Justification { Val=JustificationValues.Center });
            tableProperties.AppendChild(tableCellMargins);
        }
    }
    public class TableCellCorrectionStrategy : IElementCorrectionStrategy<CellSettingsModel>
    {
        public CellSettingsModel GetSettings(FormattingTemplateModel template)
        {
            return template.TableSettings.CellSettings;
        }
        public RunProperties GetRunProperties(CellSettingsModel settings)
        {
            return new RunProperties(
                new RunFonts { Ascii = settings.TextSettings.Font, HighAnsi = settings.TextSettings.Font },
                new Color { Val = settings.TextSettings.Color },
                new Bold { Val = settings.TextSettings.IsBold },
                new Italic { Val = settings.TextSettings.IsItalic },
                new Underline { Val = settings.TextSettings.IsUnderscore ? UnderlineValues.Single : UnderlineValues.None },
                new FontSize { Val = (settings.TextSettings.FontSize * 2).ToString() }
            );
        }

        public ParagraphProperties GetParagraphProperties(CellSettingsModel settings)
        {
            return new ParagraphProperties(
                new SpacingBetweenLines
                {
                    Line = settings.TextSettings.LineSpacing.ToString(),
                    LineRule = LineSpacingRuleValues.Auto,
                    Before = settings.TextSettings.BeforeSpacing.ToString(),
                    After = settings.TextSettings.AfterSpacing.ToString()
                },
                new Indentation
                {
                    Left = settings.TextSettings.Left.ToString(),
                    Right = settings.TextSettings.Right.ToString(),
                    FirstLine = ((int)(settings.TextSettings.FirstLine * 567)).ToString()
                },
                new Justification { Val = JustificationConverter.Parse(settings.TextSettings.Justification) },
                new KeepNext { Val = settings.TextSettings.KeepWithNext }
            );
        }

        public void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var tables = doc.MainDocumentPart?.Document?.Body?.Descendants<Table>().ToList();
            if (tables == null) return;

            foreach (var table in tables)
            {
                var cells = table.Descendants<TableCell>().ToList();
                foreach (var cell in cells)
                {
                    var cellProperties = cell.Elements<TableCellProperties>().FirstOrDefault();
                    if (cellProperties == null)
                    {
                        cellProperties = new TableCellProperties();
                        cell.PrependChild(cellProperties);
                    }

                    // в пт
                    cellProperties.RemoveAllChildren<TableCellMargin>();

                    var margins = new TableCellMargin(
                        new LeftMargin { Width = (settings.LeftPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                        new RightMargin { Width = (settings.RightPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                        new TopMargin { Width = (settings.TopPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                        new BottomMargin { Width = (settings.BottomPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa }
                    );

                    cellProperties.AppendChild(margins);

                    // TODO: парсер
                    // Устанавливаем выравнивание текста в ячейке
                    var verticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Top };

                    cellProperties.AppendChild(verticalAlignment);

                    var paragraphProperties = GetParagraphProperties(settings);
                    var runProperties = GetRunProperties(settings);

                    foreach (Paragraph paragraph in cell.Elements<Paragraph>())
                    {
                        paragraph.RemoveAllChildren<ParagraphProperties>();
                        paragraph.PrependChild(paragraphProperties.CloneNode(true));

                        foreach (Run run in paragraph.Elements<Run>())
                        {
                            run.RemoveAllChildren<RunProperties>();
                            run.PrependChild(runProperties.CloneNode(true));
                        }
                    }
                }
            }
        }
    }

    public class TableHeaderCorrectionStrategy : IElementCorrectionStrategy<HeaderSettingsModel>
    {
        public HeaderSettingsModel GetSettings(FormattingTemplateModel template)
        {
            return template.TableSettings.HeaderSettings;
        }
        public RunProperties GetRunProperties(HeaderSettingsModel settings)
        {
            return new RunProperties(
                new RunFonts { Ascii = settings.CellSettings.TextSettings.Font, HighAnsi = settings.CellSettings.TextSettings.Font },
                new Color { Val = settings.CellSettings.TextSettings.Color },
                new Bold { Val = settings.CellSettings.TextSettings.IsBold },
                new Italic { Val = settings.CellSettings.TextSettings.IsItalic },
                new Underline { Val = settings.CellSettings.TextSettings.IsUnderscore ? UnderlineValues.Single : UnderlineValues.None },
                new FontSize { Val = (settings.CellSettings.TextSettings.FontSize * 2).ToString() }
            );
        }

        public ParagraphProperties GetParagraphProperties(HeaderSettingsModel settings)
        {
            return new ParagraphProperties(
                new SpacingBetweenLines
                {
                    Line = settings.CellSettings.TextSettings.LineSpacing.ToString(),
                    LineRule = LineSpacingRuleValues.Auto,
                    Before = settings.CellSettings.TextSettings.BeforeSpacing.ToString(),
                    After = settings.CellSettings.TextSettings.AfterSpacing.ToString()
                },
                new Indentation
                {
                    Left = settings.CellSettings.TextSettings.Left.ToString(),
                    Right = settings.CellSettings.TextSettings.Right.ToString(),
                    FirstLine = ((int)(settings.CellSettings.TextSettings.FirstLine * 567)).ToString()
                },
                new Justification { Val = JustificationConverter.Parse(settings.CellSettings.TextSettings.Justification) },
                new KeepNext { Val = settings.CellSettings.TextSettings.KeepWithNext }
            );
        }

        public void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var tables = doc.MainDocumentPart?.Document?.Body?.Descendants<Table>().ToList();
            if (tables == null) return;

            foreach (var table in tables)
            {
                var firstRow = table.Elements<TableRow>().FirstOrDefault();
                if (firstRow == null) continue;

                var rowProperties = firstRow.Elements<TableRowProperties>().FirstOrDefault();
                if (rowProperties == null)
                {
                    rowProperties = new TableRowProperties();
                    firstRow.PrependChild(rowProperties);
                }

                rowProperties.RemoveAllChildren<TableHeader>();

                if (settings.HasRepetitions)
                {
                    rowProperties.AppendChild(new TableHeader());
                }

                var cells = firstRow.Elements<TableCell>().ToList();
                foreach (var cell in cells)
                {
                    var cellProperties = cell.Elements<TableCellProperties>().FirstOrDefault();
                    if (cellProperties == null)
                    {
                        cellProperties = new TableCellProperties();
                        cell.PrependChild(cellProperties);
                    }

                    // в пт
                    cellProperties.RemoveAllChildren<TableCellMargin>();

                    var margins = new TableCellMargin(
                        new LeftMargin { Width = (settings.CellSettings.LeftPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                        new RightMargin { Width = (settings.CellSettings.RightPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                        new TopMargin { Width = (settings.CellSettings.TopPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                        new BottomMargin { Width = (settings.CellSettings.BottomPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa }
                    );

                    cellProperties.AppendChild(margins);

                    // TODO: парсер
                    // Устанавливаем выравнивание текста в ячейке
                    var verticalAlignment = new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Top };

                    cellProperties.AppendChild(verticalAlignment);

                    var paragraphProperties = GetParagraphProperties(settings);
                    var runProperties = GetRunProperties(settings);

                    foreach (Paragraph paragraph in cell.Elements<Paragraph>())
                    {
                        paragraph.RemoveAllChildren<ParagraphProperties>();
                        paragraph.PrependChild(paragraphProperties.CloneNode(true));

                        foreach (Run run in paragraph.Elements<Run>())
                        {
                            run.RemoveAllChildren<RunProperties>();
                            run.PrependChild(runProperties.CloneNode(true));
                        }
                    }
                }
            }
        }
    }
}
