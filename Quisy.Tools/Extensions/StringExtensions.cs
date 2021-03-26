using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Quisy.Tools.Extensions
{
    public static class StringExtensions
    {
        public static bool IsAnyNullOrWhiteSpace(this string theString, params string[] stringsToValidate)
        {
            return string.IsNullOrWhiteSpace(theString) || stringsToValidate?.Any(string.IsNullOrWhiteSpace) != false;
        }

        public static bool IsValidGuid(this string value)
        {
            return !string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out var priceGuid) &&
                   Guid.Empty != priceGuid;
        }

        public static bool IsValidEmail(this string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                return false;

            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(emailAddress,
                @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                RegexOptions.IgnoreCase);

        }

        public static string FormatHtml(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            while (value.Contains("  "))
                value = value.Replace("  ", " ");

            value = HttpUtility.HtmlDecode(value);
            value = Regex.Replace(value, "<[^>]*>", string.Empty);
            value = Regex.Replace(value, @"\t|\n|\r", "");

            return value;
        }

        public static string FormatCurrency(this string price)
        {
            if (string.IsNullOrWhiteSpace(price))
                return price;

            if(price.Contains("$"))
                price = price.Substring(price.IndexOf("$"));

            if (price.Contains("."))
                price = price.Substring(0, price.IndexOf("."));

            return price.Replace(",", string.Empty).Replace("$", string.Empty)
                .Replace(" ", string.Empty).Replace(" ", string.Empty);//the 2nd is not a whitespace, IDK what it is
        }
    }
}
