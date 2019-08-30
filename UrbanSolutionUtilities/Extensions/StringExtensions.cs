namespace UrbanSolutionUtilities.Extensions
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string Shortify(this string input, int maxLength)
        {
            if (input == null)
            {
                return null;
            }

            return input.Length > maxLength ? $"{input.Substring(0, maxLength)}..." : input;
        }

        public static string ToFriendlyUrl(this string text)
        {
            return Regex.Replace(text, @"[^A-Za-z0-9_\.~]+", "-").ToLower();
        }

        public static string SeparateStringByCapitals(this string str)
        {
            string pattern = @"[A-Z][a-z]+";
            Regex rgx = new Regex(pattern);

            string result = string.Empty;
            MatchCollection matches = rgx.Matches(str);
            foreach (Match match in matches)
            {
                result += match.Value + " ";
            }

            return result.TrimEnd();
        }
    }
}
