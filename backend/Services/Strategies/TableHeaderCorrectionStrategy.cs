using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using FormatChanger.WebAPI.Models.FormattingModels;

namespace FormatChanger.WebAPI.Services.Strategies
{
	public class TableHeaderCorrectionStrategy : ElementCorrectionStrategyBase<HeaderSettingsModel>
	{
		public override HeaderSettingsModel GetSettings(FormattingTemplateModel template) =>
			template.TableSettings.HeaderSettings;

		public override void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
		{
			var settings = GetSettings(template);
			var tables = doc.MainDocumentPart?.Document?.Body?.Descendants<Table>().ToList();
			if (tables == null)
				return;

			var paraProps = (ParagraphProperties)CreateParagraphProperties(settings.CellSettings.TextSettings).CloneNode(true);
			var runProps = (RunProperties)CreateRunProperties(settings.CellSettings.TextSettings).CloneNode(true);

			foreach (var table in tables)
			{
				var firstRow = table.Elements<TableRow>().FirstOrDefault();
				if (firstRow == null)
					continue;

				ConfigureTableHeaderRow(firstRow, settings);

				foreach (var cell in firstRow.Elements<TableCell>())
				{
					ApplyCellLayout(cell, settings.CellSettings);

					foreach (var paragraph in cell.Elements<Paragraph>())
					{
						paragraph.RemoveAllChildren<ParagraphProperties>();
						paragraph.PrependChild(paraProps.CloneNode(true));

						foreach (var run in paragraph.Elements<Run>())
						{
							run.RemoveAllChildren<RunProperties>();
							run.PrependChild(runProps.CloneNode(true));
						}
					}
				}
			}
		}

		private void ConfigureTableHeaderRow(TableRow row, HeaderSettingsModel settings)
		{
			var props = row.Elements<TableRowProperties>().FirstOrDefault();
			if (props == null)
			{
				props = new TableRowProperties();
				row.PrependChild(props);
			}

			props.RemoveAllChildren<TableHeader>();

			if (settings.HasRepetitions)
			{
				props.AppendChild(new TableHeader());
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
			props.AppendChild(new TableCellVerticalAlignment { Val = TableVerticalAlignmentValues.Top });
		}

		public List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
		{
			throw new NotImplementedException();
		}
	}
}
