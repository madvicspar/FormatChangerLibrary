using System.Text.RegularExpressions;
using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services
{
	public static class FormattingChecker
	{
		/// <summary>
		/// Проверяет соответствие текста подписи шаблону и разделителю.
		/// TextTemplate может содержать «N» как плейсхолдер для номера, например «Таблица N».
		/// </summary>
		public static List<string> CheckCaptionContent(ParagraphStyleProperties actual, ICaptionSettingsModel expected)
		{
			var issues = new List<string>();
			var text = actual.InnerText ?? string.Empty;

			if (!string.IsNullOrWhiteSpace(expected.TextTemplate))
			{
				// Строим regex: «N» → \d+, остальное экранируем
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

			// Модель хранит сырые значения OpenXML: 240 = одинарный, 360 = полуторный (для интервала строк)
			// Resolver нормализует: делит на 240. Приводим ожидаемое к тем же единицам.
			CompareProperty("Междустрочный интервал", actual.SpacingLine,
				NormalizeRaw(expected.LineSpacing, 240.0), issues);

			// Интервалы до/после: 20 единиц OpenXML = 1 пт
			CompareProperty("Интервал перед", actual.SpacingBefore,
				NormalizeRaw(expected.BeforeSpacing, 20.0), issues);
			CompareProperty("Интервал после", actual.SpacingAfter,
				NormalizeRaw(expected.AfterSpacing, 20.0), issues);

			CompareProperty("Отступ первой строки", actual.IndentFirstLine, expected.FirstLine.ToString(), issues);
			CompareProperty("Отступ слева", actual.IndentLeft, expected.Left.ToString(), issues);
			CompareProperty("Отступ справа", actual.IndentRight, expected.Right.ToString(), issues);

			if (!CompareNullable(actual.Justification, expected.Justification))
				issues.Add($"Выравнивание: {actual.Justification ?? "не задано"}, должно быть {expected.Justification}");

			return issues;
		}

		// Нормализует raw-значение OpenXML к той же шкале, что и FormattingResolver.Normalize
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

		private static bool CompareNullable(string actual, string expected)
		{
			return (actual ?? "") == (expected ?? "");
		}
	}
}
