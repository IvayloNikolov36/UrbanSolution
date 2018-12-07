namespace UrbanSolution.Web.Infrastructure.Extensions
{
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string Shortify(this string input, int length)
        {
            if (input == null)
            {
                return input;
            }

            return input.Length > length ? $"{input.Substring(length)}..." : input;
        }

        public static string ToFriendlyUrl(this string text)
        {
            return Regex.Replace(text, @"[^A-Za-z0-9_\.~]+", "-").ToLower();
        }

    }
}
