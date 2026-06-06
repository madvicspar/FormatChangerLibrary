using System.Text.RegularExpressions;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services
{
	public static class FormattingChecker
	{
		/// <summary>
		/// Checks whether the caption text matches the template and separator.
		/// TextTemplate may contain "N" as a placeholder for a number, e.g. "Table N".
		/// </summary>
		public static List<string> CheckCaptionContent(ParagraphStyleProperties actual, ICaptionSettingsModel expected)
		{
			var issues = new List<string>();
			var text = actual.InnerText ?? string.Empty;

			if (!string.IsNullOrWhiteSpace(expected.TextTemplate))
			{
				// Build regex: "N" → \d+, escape everything else
				var rawTemplate = expected.TextTemplate;
				var regexPattern = "^" + Regex.Escape(rawTemplate).Replace("N", @"\d+");

				if (!string.IsNullOrWhiteSpace(expected.Separator))
					regexPattern += Regex.Escape(expected.Separator);

				if (!Regex.IsMatch(text, regexPattern))
				{
					var expectedPattern = string.IsNullOrWhiteSpace(expected.Separator)
						? rawTemplate
						: rawTemplate + expected.Separator + "...";
					issues.Add($"Текст подписи не соответствует шаблону. Ожидается начало: «{expectedPattern}», фактически: «{text}»");
				}
			}
			else if (!string.IsNullOrWhiteSpace(expected.Separator))
			{
				if (!text.Contains(expected.Separator))
					issues.Add($"Отсутствует разделитель «{expected.Separator}» в тексте подписи");
			}

			return issues;
		}
		public static List<string> Check(ParagraphStyleProperties actual, TextSettingsModel expected)
		{
			var issues = new List<string>();

			if (actual.RunStyle.Bold != expected.IsBold)
				issues.Add(expected.IsBold
					? "Должен быть полужирным"
					: "Не должен быть полужирным");

			if (actual.RunStyle.Italic != expected.IsItalic)
				issues.Add(expected.IsItalic
					? "Должен быть курсивом"
					: "Не должен быть курсивом");

			if (actual.RunStyle.Underline != expected.IsUnderscore)
				issues.Add(expected.IsUnderscore
					? "Должен быть подчёркнутым"
					: "Не должен быть подчёркнутым");

			if (!CompareNullable(actual.RunStyle.Color, expected.Color))
				issues.Add($"Цвет текста: {actual.RunStyle.Color ?? "не задан"}, должен быть {expected.Color}");

			if (!CompareNullable(actual.RunStyle.FontSize, expected.FontSize.ToString()))
				issues.Add($"Размер шрифта: {actual.RunStyle.FontSize ?? "не задан"}, должен быть {expected.FontSize}");

			if (!CompareNullable(actual.RunStyle.Font, expected.Font))
				issues.Add($"Шрифт: {actual.RunStyle.Font ?? "не задан"}, должен быть {expected.Font}");

			// Model stores raw OpenXML values: 240 = single, 360 = 1.5x (line spacing)
			// Resolver normalises by dividing by 240; bring the expected value to the same scale.
			CompareProperty("Междустрочный интервал", actual.SpacingLine,
				NormalizeRaw(expected.LineSpacing, 240.0), issues);

			// Before/after spacing: 20 OpenXML units = 1 pt
			CompareProperty("Интервал перед", actual.SpacingBefore,
				NormalizeRaw(expected.BeforeSpacing, 20.0), issues);
			CompareProperty("Интервал после", actual.SpacingAfter,
				NormalizeRaw(expected.AfterSpacing, 20.0), issues);

			CompareFirstLineIndent(actual.IndentFirstLine, expected.FirstLine.ToString(), issues);
			CompareProperty("Отступ слева", actual.IndentLeft, expected.Left.ToString(), issues);
			CompareProperty("Отступ справа", actual.IndentRight, expected.Right.ToString(), issues);

			if (!CompareNullable(actual.Justification, expected.Justification))
				issues.Add($"Выравнивание: {actual.Justification ?? "не задано"}, должно быть {expected.Justification}");

			return issues;
		}

		// Normalises a raw OpenXML value to the same scale used by FormattingResolver.Normalize
		private static string NormalizeRaw(float rawValue, double divisor) =>
			Math.Round(rawValue / divisor, 2).ToString();

		private static void CompareProperty(string name, string actual, string expected, List<string> issues)
		{
			if (!CompareNullable(actual, expected))
			{
				if (expected == "0" && actual == null) return;

				issues.Add($"{name}: {actual ?? "не задан"}, должен быть {expected}");
			}
		}

		/// <summary>
		/// Compares the first-line indent/hanging indent.
		/// A negative actual value means a hanging indent (w:hanging) — displayed without a minus sign.
		/// </summary>
		private static void CompareFirstLineIndent(string actual, string expected, List<string> issues)
		{
			if (CompareNullable(actual, expected)) return;
			if (expected == "0" && actual == null) return;

			// Resolver uses the current culture in ToString(), so parse with the same culture
			if (actual != null &&
				double.TryParse(actual, System.Globalization.NumberStyles.Any,
					System.Globalization.CultureInfo.CurrentCulture, out var v) && v < 0)
			{
				issues.Add($"Выступ первой строки: {Math.Abs(v).ToString(System.Globalization.CultureInfo.CurrentCulture)}, должен быть {expected} (отступ)");
			}
			else
			{
				issues.Add($"Отступ первой строки: {actual ?? "не задан"}, должен быть {expected}");
			}
		}

		public static List<string> CheckImage(ParagraphStyleProperties actual, ImageSettingsModel expected)
		{
			var issues = new List<string>();

			CompareProperty("Междустрочный интервал", actual.SpacingLine,
				NormalizeRaw(expected.LineSpacing, 240.0), issues);
			CompareProperty("Интервал перед", actual.SpacingBefore,
				NormalizeRaw(expected.BeforeSpacing, 20.0), issues);
			CompareProperty("Интервал после", actual.SpacingAfter,
				NormalizeRaw(expected.AfterSpacing, 20.0), issues);
			CompareFirstLineIndent(actual.IndentFirstLine, NormalizeRaw(expected.FirstLine, 567.0), issues);
			CompareProperty("Отступ слева", actual.IndentLeft,
				NormalizeRaw(expected.Left, 567.0), issues);
			CompareProperty("Отступ справа", actual.IndentRight,
				NormalizeRaw(expected.Right, 567.0), issues);

			if (!CompareNullable(actual.Justification, expected.Justification))
				issues.Add($"Выравнивание: {actual.Justification ?? "не задано"}, должно быть {expected.Justification}");

			return issues;
		}

		private static bool CompareNullable(string actual, string expected)
		{
			return (actual ?? "") == (expected ?? "");
		}
	}
}
