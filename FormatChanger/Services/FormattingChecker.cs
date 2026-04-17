using FormatChanger.Models;
using FormatChanger.Models.FormattingModels;

namespace FormatChanger.Services
{
	public static class FormattingChecker
	{
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

			if (!CompareNullable(actual.RunStyle.Color, expected.Color))
				issues.Add($"Цвет текста должен быть {expected.Color}");

			if (!CompareNullable(actual.RunStyle.FontSize, expected.FontSize.ToString()))
				issues.Add($"Размер шрифта должен быть {expected.FontSize}");

			CompareProperty("Междустрочный интервал", actual.SpacingLine, expected.LineSpacing.ToString(), issues);
			CompareProperty("Интервал перед", actual.SpacingBefore, expected.BeforeSpacing.ToString(), issues);
			CompareProperty("Интервал после", actual.SpacingAfter, expected.AfterSpacing.ToString(), issues);

			CompareProperty("Отступ первой строки", actual.IndentFirstLine, expected.FirstLine.ToString(), issues);
			CompareProperty("Отступ слева", actual.IndentLeft, expected.Left.ToString(), issues);
			CompareProperty("Отступ справа", actual.IndentRight, expected.Right.ToString(), issues);

			return issues;
		}

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