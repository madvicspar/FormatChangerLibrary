using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.FormattingModels;
using FormatChanger.Models.Helpers;

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
            var body = doc.MainDocumentPart?.Document?.Body;
            var tables = body?.Descendants<Table>().ToList();
            if (!tables.Any())
                return;

            var normalStyleId = doc.MainDocumentPart.StyleDefinitionsPart.Styles.Elements<Style>()
                .FirstOrDefault(s => s.StyleName?.Val == ParagraphTypes.Normal.ToString())?.StyleId;

            foreach (var table in tables)
            {
                ApplyTableProperties(table, settings);
                if (settings.AfterSpacing != 0)
                {
                    TrySetSpacingAfterTable(body.Elements<OpenXmlElement>().ToList(), table, settings.AfterSpacing, normalStyleId);
                }
            }
        }

        public void ApplyTableProperties(Table table, TableSettingsModel settings)
        {
            // TODO: интервал до
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

        /// <summary>
        /// Устанавливает интервал перед следующим абзацем после таблицы,
        /// если он имеет стиль "Обычный текст" (в том числе и списки).
        /// </summary>
        private void TrySetSpacingAfterTable(List<OpenXmlElement> bodyElements, Table table, float spacing, string normalStyleId)
        {
            var index = bodyElements.IndexOf(table);
            if (index < 0 || index >= bodyElements.Count - 1)
                return;

            if (bodyElements[index + 1] is Paragraph nextParagraph)
            {
                var styleId = nextParagraph.ParagraphProperties?.ParagraphStyleId?.Val;
                if (string.IsNullOrEmpty(styleId) || styleId == normalStyleId)
                {
                    SetSpacingAfterTable(nextParagraph, spacing);
                }
            }
        }

        private void SetSpacingAfterTable(Paragraph nextParagraph, float spaceAfter)
        {
            var props = nextParagraph.ParagraphProperties ?? new ParagraphProperties();
            nextParagraph.ParagraphProperties ??= props;

            var spacing = props.GetFirstChild<SpacingBetweenLines>() ?? new SpacingBetweenLines();
            props.RemoveAllChildren<SpacingBetweenLines>();
            props.Append(spacing);

            spacing.Before = spaceAfter.ToString();
        }

    }
}
