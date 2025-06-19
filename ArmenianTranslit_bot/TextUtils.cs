using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmenianTranslit_bot
{
    public static class TextUtils
    {
        public static bool IsAllUpper(string input) => input.All(char.IsUpper);

        public static bool IsTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            string clean = new string(input.Where(char.IsLetter).ToArray());

            return clean.Length > 0 &&
                   char.IsUpper(clean[0]) &&
                   clean.Skip(1).All(char.IsLower);
        }

        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return char.ToUpperInvariant(input[0]) + input[1..];
        }
    }
}
