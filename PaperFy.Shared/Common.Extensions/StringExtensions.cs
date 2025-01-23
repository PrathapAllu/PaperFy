namespace PaperFy.Shared.Common.Extensions
{
    public static class StringExtensions
	{
		public static string AddSpacesBetweenCapitalLetters(this string input)
		{
			return string.Concat(input.Select((char i) => char.IsUpper(i) ? $" {i}" : i.ToString())).TrimStart();
		}
	}
}
