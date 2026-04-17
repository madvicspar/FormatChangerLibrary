using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services.Strategies
{
	public class TextCorrectionStrategy : ElementCorrectionStrategyBase<TextSettingsModel>
	{
		public override TextSettingsModel GetSettings(FormattingTemplateModel template) =>
			template.TextSettings;

		public override void ApplyCorrection(WordprocessingDocument doc, FormattingTemplateModel template)
		{
			var settings = GetSettings(template);
			var styles = doc.MainDocumentPart?.StyleDefinitionsPart?.Styles;
			if (styles == null) return;

			var runProps = CreateRunProperties(settings);
			var paraProps = CreateParagraphProperties(settings);

			ApplyToStyle(styles, "Normal", runProps, paraProps);
		}

		public override List<string> CheckFormatting(Paragraph paragraph, FormattingTemplateModel template)
		{
			var issues = new List<string>();
			var settings = GetSettings(template);

			var runProps = CreateRunProperties(settings);
			var paraProps = CreateParagraphProperties(settings);

			var actualRunProps = paragraph.Descendants<RunProperties>().FirstOrDefault();
			var actualParaProps = paragraph.ParagraphProperties;

			string styleId = paragraph.ParagraphProperties?.ParagraphStyleId?.Val;
			Style style = null;
			StyleRunProperties styleRunProps = null;
			StyleParagraphProperties styleParagraphProps = null;

			IEnumerable<Style> styles = new List<Style>();
			var document = paragraph.Ancestors<Document>().FirstOrDefault();
			if (document != null)
			{
				var stylePart = document.MainDocumentPart?.StyleDefinitionsPart;
				if (stylePart != null)
				{
					styles = stylePart.Styles.Elements<Style>();
					style = stylePart.Styles.Elements<Style>().FirstOrDefault(s => s.StyleId == styleId);
					styleRunProps = style?.StyleRunProperties;
					styleParagraphProps = style?.StyleParagraphProperties;
				}
			}

			CompareRunProperties(paragraph, runProps, styles, issues);
			CompareParagraphProperties(paragraph, actualParaProps, paraProps, styles, issues);

			return issues;
		}

		private void CompareRunProperties(Paragraph p, RunProperties expected, IEnumerable<Style> styles, List<string> issues)
		{
			var runs = p.Descendants<Run>().ToList();

			if (!runs.Any())
				return;

			var expectedFont = expected.RunFonts?.Ascii?.Value
				?? expected.RunFonts?.ComplexScript?.Value;

			var expectedSize = NormalizeFontSize(expected.FontSize?.Val);
			var expectedColor = expected.Color?.Val ?? "000000";
			var expectedBold = expected.Bold?.Val ?? false;
			var expectedItalic = expected.Italic?.Val ?? false;

			bool hasWrongBold = false;
			bool hasWrongItalic = false;
			bool hasWrongColor = false;
			bool hasWrongSize = false;

			foreach (var run in runs)
			{
				var bold = IsBold(run, p);
				var italic = IsItalic(run, p);
				var color = GetColor(run, p) ?? "000000";
				var size = GetFontSize(run, p);

				if (bold != expectedBold)
					hasWrongBold = true;

				if (italic != expectedItalic)
					hasWrongItalic = true;

				if (color != expectedColor)
					hasWrongColor = true;

				if (size != expectedSize)
					hasWrongSize = true;
			}

			if (hasWrongBold)
				issues.Add(expectedBold ? "Должен быть полужирным" : "Не должен быть полужирным");

			if (hasWrongItalic)
				issues.Add(expectedItalic ? "Должен быть курсивом" : "Не должен быть курсивом");

			if (hasWrongColor)
				issues.Add($"Цвет текста должен быть {expectedColor}");

			if (hasWrongSize)
				issues.Add($"Размер шрифта должен быть {expectedSize}");
		}

		private bool IsBold(Run r, Paragraph p)
		{
			if (r.RunProperties?.Bold?.Val != null)
				return r.RunProperties.Bold.Val;

			if (r.RunProperties?.BoldComplexScript?.Val != null)
				return r.RunProperties.BoldComplexScript.Val;

			if (p.ParagraphProperties?.GetFirstChild<RunProperties>()?.Bold?.Val != null)
				return p.ParagraphProperties?.GetFirstChild<RunProperties>()?.Bold.Val;

			if (p.ParagraphProperties?.GetFirstChild<RunProperties>()?.BoldComplexScript?.Val != null)
				return p.ParagraphProperties?.GetFirstChild<RunProperties>()?.BoldComplexScript.Val;

			return false;
		}

		private bool IsItalic(Run r, Paragraph p)
		{
			if (r.RunProperties?.Italic?.Val != null)
				return r.RunProperties.Italic.Val;

			if (r.RunProperties?.ItalicComplexScript?.Val != null)
				return r.RunProperties.ItalicComplexScript.Val;

			if (p.ParagraphProperties?.GetFirstChild<RunProperties>()?.Italic?.Val != null)
				return p.ParagraphProperties?.GetFirstChild<RunProperties>()?.Italic.Val;

			if (p.ParagraphProperties?.GetFirstChild<RunProperties>()?.ItalicComplexScript?.Val != null)
				return p.ParagraphProperties?.GetFirstChild<RunProperties>()?.ItalicComplexScript.Val;

			return false;
		}

		private string GetColor(Run r, Paragraph p)
		{
			var color = r.RunProperties?.Color?.Val;
			if (!string.IsNullOrEmpty(color))
				return color;

			color = p.ParagraphProperties?.GetFirstChild<RunProperties>()?.Color?.Val;
			if (!string.IsNullOrEmpty(color))
				return color;

			return "auto";
		}

		private string GetFontSize(Run r, Paragraph p)
		{
			var val = r.RunProperties?.FontSize?.Val
				   ?? r.RunProperties?.FontSizeComplexScript?.Val
				   ?? p.ParagraphProperties?.GetFirstChild<RunProperties>()?.FontSize?.Val
				   ?? p.ParagraphProperties?.GetFirstChild<RunProperties>()?.FontSizeComplexScript?.Val;

			return NormalizeFontSize(val);
		}

		private string NormalizeFontSize(string val)
		{
			if (string.IsNullOrEmpty(val)) return null;

			if (int.TryParse(val, out int size))
				return (size / 2).ToString(); // Word хранит *2

			return val;
		}

		private void CompareProperty(string propertyName, string actualValue, string expectedValue, List<string> issues)
		{
			if (actualValue != expectedValue)
			{
				if (expectedValue == "0" && actualValue == null)
					return;
				issues.Add($"{propertyName}: {actualValue ?? "не задан"}, должен быть {expectedValue}");
			}
		}

		private void CompareParagraphProperties(Paragraph p, ParagraphProperties actual, ParagraphProperties expected, IEnumerable<Style> styles, List<string> issues)
		{
			string GetSpacingValue(string value, double denominator)
			{
				if (double.TryParse(value, out double parsedValue))
				{
					return Math.Round(parsedValue / denominator, 2).ToString();
				}
				return null;
			}

			var actualSpacingLine = GetSpacingValue(actual?.SpacingBetweenLines?.Line ?? GetPropertyValue(p, styles, rp => rp?.SpacingBetweenLines?.Line, srp => srp?.SpacingBetweenLines?.Line), 240);
			var expectedSpacingLine = GetSpacingValue(expected.SpacingBetweenLines.Line, 240);
			var actualSpacingBefore = GetSpacingValue(actual?.SpacingBetweenLines?.Before?.Value ?? GetPropertyValue(p, styles, rp => rp?.SpacingBetweenLines?.Before?.Value, srp => srp?.SpacingBetweenLines?.Before?.Value), 20);
			var expectedSpacingBefore = GetSpacingValue(expected.SpacingBetweenLines.Before?.Value, 20);
			var actualSpacingAfter = GetSpacingValue(actual?.SpacingBetweenLines?.After?.Value ?? GetPropertyValue(p, styles, rp => rp?.SpacingBetweenLines?.After?.Value, srp => srp?.SpacingBetweenLines?.After?.Value), 20);
			var expectedSpacingAfter = GetSpacingValue(expected.SpacingBetweenLines.After?.Value, 20);
			var actualIndentationFirstLine = GetSpacingValue(actual?.Indentation?.FirstLine?.Value ?? GetPropertyValue(p, styles, rp => rp?.Indentation?.FirstLine?.Value, srp => srp?.Indentation?.FirstLine?.Value), 567);
			var expectedIndentationFirstLine = GetSpacingValue(expected.Indentation?.FirstLine?.Value, 567);
			var actualIndentationLeft = GetSpacingValue(actual?.Indentation?.Left?.Value ?? GetPropertyValue(p, styles, rp => rp?.Indentation?.Left?.Value, srp => srp?.Indentation?.Left?.Value), 567);
			var expectedIndentationLeft = GetSpacingValue(expected.Indentation?.Left?.Value, 567);
			var actualIndentationRight = GetSpacingValue(actual?.Indentation?.Right?.Value ?? GetPropertyValue(p, styles, rp => rp?.Indentation?.Right?.Value, srp => srp?.Indentation?.Right?.Value), 567);
			var expectedIndentationRight = GetSpacingValue(expected.Indentation?.Right?.Value, 567);

			CompareProperty("Междустрочный интервал", actualSpacingLine, expectedSpacingLine, issues);
			CompareProperty("Интервал перед", actualSpacingBefore, expectedSpacingBefore, issues);
			CompareProperty("Интервал после", actualSpacingAfter, expectedSpacingAfter, issues);
			CompareProperty("Отступ первой строки", actualIndentationFirstLine, expectedIndentationFirstLine, issues);
			CompareProperty("Отступ слева", actualIndentationLeft, expectedIndentationLeft, issues);
			CompareProperty("Отступ справа", actualIndentationRight, expectedIndentationRight, issues);
		}

		private T GetPropertyValue<T>(
			Paragraph p,
			IEnumerable<Style> styles,
			Func<ParagraphProperties, T> runPropSelector,
			Func<StyleParagraphProperties, T> stylePropSelector)
			where T : class
		{
			var actual = runPropSelector(p.Descendants<ParagraphProperties>().FirstOrDefault());
			if (actual != null)
				return actual;

			var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;
			if (string.IsNullOrEmpty(styleId)) return null;

			var style = styles.FirstOrDefault(s => s.StyleId == styleId);
			if (style != null)
			{
				var fromStyle = stylePropSelector(style.StyleParagraphProperties);
				if (fromStyle != null)
					return fromStyle;
			}

			styleId = styles.FirstOrDefault(s => s.StyleId == styleId)?.BasedOn?.Val;
			while (!string.IsNullOrEmpty(styleId))
			{
				var parentStyle = styles.FirstOrDefault(s => s.StyleId == styleId);
				if (parentStyle == null) break;

				var parentStyleProp = stylePropSelector(parentStyle.StyleParagraphProperties);
				if (parentStyleProp != null)
					return parentStyleProp;

				styleId = parentStyle.BasedOn?.Val;
			}

			return null;
		}
	}
}
