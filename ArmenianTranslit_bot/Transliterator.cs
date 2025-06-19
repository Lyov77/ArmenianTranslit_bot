using ArmenianTranslit_bot;
using System.Text;
using System.Text.RegularExpressions;

public static class Transliterator
{   
        public static string Transliterate(string input)
        {
            input = TextNormalizer.NormalizeApostrophes(input);
            var words = Regex.Split(input, @"(\s+|[^a-zA-Z@']+)");
            var result = new StringBuilder();

            foreach (var word in words)
            {
                if (Regex.IsMatch(word, @"^[a-zA-Z@']+$"))
                {
                    result.Append(WordTransliterator.TransliterateWord(word));
                }
                else
                {
                    result.Append(word);
                }
            }

            return result.ToString();
        } 
}
