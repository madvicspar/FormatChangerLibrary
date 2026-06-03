using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Strategies
{
    public class TableCellCorrectionStrategy : ElementCorrectionStrategyBase<CellSettingsModel>
    {
        public override CellSettingsModel GetSettings(FormattingTemplateModel template) =>
            template.TableSettings.CellSettings;

        public override void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
        {
            var settings = GetSettings(template);
            var tables = doc.MainDocumentPart?.Document?.Body?.Descendants<Table>().ToList();
            if (tables == null)
                return;

            var paraProps = CreateParagraphProperties(settings.TextSettings);
            var runProps = CreateRunProperties(settings.TextSettings);

            foreach (var table in tables)
            {
                foreach (var cell in table.Descendants<TableCell>())
                {
                    ApplyCellLayout(cell, settings);
                    ApplyCellProperties(cell, paraProps, runProps);
                }
            }
        }

        private void ApplyCellProperties(
            TableCell cell,
            ParagraphProperties paraProps,
            RunProperties runProps)
        {
            foreach (var paragraph in cell.Elements<Paragraph>())
            {
                paragraph.RemoveAllChildren<ParagraphProperties>();
                paragraph.PrependChild((ParagraphProperties)paraProps.CloneNode(true));

                foreach (var run in paragraph.Elements<Run>())
                {
                    run.RemoveAllChildren<RunProperties>();
                    run.PrependChild((RunProperties)runProps.CloneNode(true));
                }
            }
        }

        private void ApplyCellLayout(TableCell cell, CellSettingsModel settings)
        {
            var props = cell.Elements<TableCellProperties>().FirstOrDefault();
            if (props == null)
            {
                props = new TableCellProperties();
                cell.PrependChild(props);
            }

            props.RemoveAllChildren<TableCellMargin>();
            props.RemoveAllChildren<TableCellVerticalAlignment>();

            props.AppendChild(new TableCellMargin(
                new LeftMargin { Width = (settings.LeftPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                new RightMargin { Width = (settings.RightPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                new TopMargin { Width = (settings.TopPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = (settings.BottomPadding * 20).ToString(), Type = TableWidthUnitValues.Dxa }
            ));

            // TODO: парсер
            props.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Top });
        }

    }
}
