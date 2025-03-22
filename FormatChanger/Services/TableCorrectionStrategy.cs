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

        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }
    }
}
