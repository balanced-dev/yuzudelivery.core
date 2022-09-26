using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YuzuDelivery.Core
{
    public static class StringExtensions
    {
        public const string PathDelimiter = "/";

        public static string RemoveFirstCharacter(this string str)
        {
            return str.Substring(1);
        }

        public static string RemoveFirstForwardSlash(this string str)
        {
            if (str.StartsWith("/"))
                return str.Substring(1);
            else
                return str;
        }

        public static string FirstCharacterToLower(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static string FirstCharacterToUpper(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsUpper(str, 0))
                return str;

            return Char.ToUpperInvariant(str[0]) + str.Substring(1);
        }

        public static string CamelToSentenceCase(this string input)
        {
            string output = Regex.Replace(input, @"\p{Lu}", m => " " + m.Value.ToLowerInvariant());
            output = char.ToUpperInvariant(output[0]) + output.Substring(1);
            output = output.Trim();
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(output.ToLower());
        }

        public static string SentenceToCamelCase(this string input)
        {
            string output = Regex.Replace(input, @"\p{Lu}", m => " " + m.Value.ToLowerInvariant());
            output = char.ToUpperInvariant(output[0]) + output.Substring(1);
            return output;
        }

        public static int ToInt(this object obj)
        {
            if (obj != null)
                return obj.ToString().ToInt();
            else
                return 0;
        }

        public static int ToInt(this string str)
        {
            int output;
            if (int.TryParse(str, out output))
                return output;
            else
                return 0;
        }

        public static bool ToBool(this object obj)
        {
            if (obj != null)
                return obj.ToString().ToBool();
            else
                return false;
        }

        public static bool ToBool(this string str)
        {
            bool output;
            if (str == "1")
                return true;
            else if (bool.TryParse(str, out output))
                return output;
            else
                return false;
        }

    }


}
