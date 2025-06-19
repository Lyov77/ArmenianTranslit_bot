using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmenianTranslit_bot
{
    public static class TextNormalizer
    {
        public static string NormalizeApostrophes(string input)
        {
            char[] apostrophes = { '’', '‘', '՚', '՛', 'ˈ' };
            foreach (var ch in apostrophes)
            {
                input = input.Replace(ch, '\'');
            }
            return input;
        }
    }
}
