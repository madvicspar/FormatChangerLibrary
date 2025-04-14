using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Strategies
{
    public class TableCorrectionStrategy : ElementCorrectionStrategyBase<TableSettingsModel>
    {
        public override TableSettingsModel GetSettings(FormattingTemplateModel template) =>
            template.TableSettings;

        public override void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            // TODO: выравнивание по ширине окна и по содержимому
            var settings = GetSettings(template);
            var tables = doc.MainDocumentPart?.Document?.Body?.Descendants<Table>().ToList();
            if (tables == null)
                return;

            foreach (var table in tables)
            {
                ApplyTableProperties(table, settings);
            }
        }

        public void ApplyTableProperties(Table table, TableSettingsModel settings)
        {
            // TODO: интервал до и после таблицы
            // TODO: в бд хранить все в dxa, реализовать перевод, в интерфейсе - пт и см
            var props = table.Elements<TableProperties>().FirstOrDefault();

            if (props == null)
            {
                props = new TableProperties();
                table.PrependChild(props);
            }

            props.RemoveAllChildren<TableCellMarginDefault>();
            props.RemoveAllChildren<TableCellVerticalAlignment>();
            props.RemoveAllChildren<Justification>();

            props.AppendChild(new TableCellMarginDefault(
                new LeftMargin { Width = (settings.CellSettings.LeftPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                new RightMargin { Width = (settings.CellSettings.RightPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                new TopMargin { Width = (settings.CellSettings.TopPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = (settings.CellSettings.BottomPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa }
            ));

            props.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Center });
            props.AppendChild(new Justification { Val = JustificationValues.Center });
        }

        public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
        {
            throw new NotImplementedException();
        }
    }
}
