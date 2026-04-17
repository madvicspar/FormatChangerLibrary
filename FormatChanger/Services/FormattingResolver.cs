using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using FormatChanger.Models;

namespace FormatChanger.Services
{
	public class FormattingResolver
	{
		private const double LINE_SPACING_DIVISOR = 240.0;
		private const double SPACING_DIVISOR = 20.0;
		private const double INDENT_DIVISOR = 567.0;
		private const double FONT_SIZE_DIVISOR = 2.0;

		/// <summary>
		/// Получить форматирование абзаца
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public ParagraphStyleProperties ResolveParagraph(Paragraph p)
		{
			var styles = GetStyles(p);

			return new ParagraphStyleProperties
			{
				StyleId = p.ParagraphProperties?.ParagraphStyleId?.Val,
				RunStyle = ResolveRun(p, styles),

				SpacingLine = GetSpacing(p, styles, x => x.Line, LINE_SPACING_DIVISOR),
				SpacingBefore = GetSpacing(p, styles, x => x.Before?.Value, SPACING_DIVISOR),
				SpacingAfter = GetSpacing(p, styles, x => x.After?.Value, SPACING_DIVISOR),

				IndentFirstLine = GetIndent(p, styles, x => x.FirstLine?.Value),
				IndentLeft = GetIndent(p, styles, x => x.Left?.Value),
				IndentRight = GetIndent(p, styles, x => x.Right?.Value),
			};
		}

		#region Run resolution
		private RunStyleProperties ResolveRun(Paragraph p, IEnumerable<Style> styles)
		{
			var runs = p.Descendants<Run>().ToList();

			return new RunStyleProperties
			{
				Bold = runs.Any(r => IsBold(r, p, styles)),
				Italic = runs.Any(r => IsItalic(r, p, styles)),
				Color = runs.Select(r => GetColor(r, p, styles)).FirstOrDefault(x => x != null),
				FontSize = runs.Select(r => GetFontSize(r, p, styles)).FirstOrDefault(x => x != null)
			};
		}

		private bool IsBold(Run r, Paragraph p, IEnumerable<Style> styles)
		{
			if (r.RunProperties?.Bold != null)
				return r.RunProperties.Bold.Val ?? true;

			return GetBoolFromStyle(p, styles, s => s.StyleRunProperties?.Bold);
		}

		private bool IsItalic(Run r, Paragraph p, IEnumerable<Style> styles)
		{
			if (r.RunProperties?.Italic != null)
				return r.RunProperties.Italic.Val ?? true;

			return GetBoolFromStyle(p, styles, s => s.StyleRunProperties?.Italic);
		}

		private static string GetColor(Run r, Paragraph p, IEnumerable<Style> styles)
		{
			if (!string.IsNullOrEmpty(r.RunProperties?.Color?.Val))
				return r.RunProperties.Color.Val;

			return GetFromStyle(p, styles, s => s.Color?.Val);
		}

		private static string GetFontSize(Run r, Paragraph p, IEnumerable<Style> styles)
		{
			var val = r.RunProperties?.FontSize?.Val;
			if (val != null) return Normalize(val, FONT_SIZE_DIVISOR);

			return Normalize(GetFromStyle(p, styles, s => s.FontSize?.Val), FONT_SIZE_DIVISOR);
		}

		private static string GetFromStyle(Paragraph p, IEnumerable<Style> styles,
			Func<StyleRunProperties, string> selector)
		{
			return TraverseStyleHierarchy(p, styles,
				style => selector(style.StyleRunProperties));
		}
		#endregion

		#region Paragraph resolution
		private static string GetSpacing(Paragraph p, IEnumerable<Style> styles,
		Func<SpacingBetweenLines, string> selector, double div)
		{
			var val = p.ParagraphProperties?.SpacingBetweenLines != null
				? selector(p.ParagraphProperties.SpacingBetweenLines)
				: null;

			if (val != null) return Normalize(val, div);

			return Normalize(GetFromStyle(p, styles, s => selector(s?.SpacingBetweenLines)), div);
		}

		private static string GetIndent(Paragraph p, IEnumerable<Style> styles,
		Func<Indentation, string> selector)
		{
			var val = p.ParagraphProperties?.Indentation != null
				? selector(p.ParagraphProperties.Indentation)
				: null;

			if (val != null) return Normalize(val, INDENT_DIVISOR);

			return Normalize(GetFromStyle(p, styles, s => selector(s?.Indentation)), INDENT_DIVISOR);
		}

		private static string GetFromStyle(Paragraph p, IEnumerable<Style> styles,
			Func<StyleParagraphProperties, string> selector)
		{
			return TraverseStyleHierarchy(p, styles,
				style => selector(style.StyleParagraphProperties));
		}
		#endregion

		#region Helpers
		private static T TraverseStyleHierarchy<T>(Paragraph p, IEnumerable<Style> styles,
			Func<Style, T> selector)
		{
			var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;

			while (!string.IsNullOrEmpty(styleId))
			{
				var style = styles.FirstOrDefault(s => s.StyleId == styleId);
				if (style == null) break;

				var val = selector(style);
				if (val != null) return val;

				styleId = style.BasedOn?.Val;
			}

			return default;
		}

		private static bool GetBoolFromStyle(Paragraph p, IEnumerable<Style> styles,
		Func<Style, OpenXmlElement> selector)
		{
			var styleId = p.ParagraphProperties?.ParagraphStyleId?.Val;

			while (!string.IsNullOrEmpty(styleId))
			{
				var style = styles.FirstOrDefault(s => s.StyleId == styleId);
				if (style == null) break;

				var el = selector(style);
				if (el != null) return true;

				styleId = style.BasedOn?.Val;
			}

			return false;
		}

		private static IEnumerable<Style> GetStyles(Paragraph p)
		{
			return p.Ancestors<Document>()
				.FirstOrDefault()?
				.MainDocumentPart?
				.StyleDefinitionsPart?
				.Styles?
				.Elements<Style>() ?? Enumerable.Empty<Style>();
		}

		private static string Normalize(string val, double divisor)
		{
			if (string.IsNullOrEmpty(val)) return null;
			if (double.TryParse(val, out var v))
				return Math.Round(v / divisor, 2).ToString();
			return val;
		}
		#endregion
	}
}