using System.Text;


namespace ArmenianTranslit_bot
{
    public static class WordTransliterator
    {
        public static string TransliterateWord(string word)
        {
            var lower = word.ToLowerInvariant();

            if (TransliterationData.WordLibrary.TryGetValue(lower, out var lib))
            {
                if (TextUtils.IsAllUpper(word))
                    return lib.upper;
                if (TextUtils.IsTitleCase(word))
                    return TextUtils.ToTitleCase(lib.lower);
                return lib.lower;
            }

            var result = new StringBuilder();
            int i = 0;

            while (i < word.Length)
            {
                string sub = word[i..];

                // Special combinations
                var match = TransliterationData.SpecialCombinations
                    .OrderByDescending(kv => kv.Key.Length)
                    .FirstOrDefault(kv => sub.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(match.Key))
                {
                    string original = word.Substring(i, match.Key.Length);
                    result.Append(TextUtils.IsAllUpper(original) ? match.Value.upper :
                                  TextUtils.IsTitleCase(original) ? TextUtils.ToTitleCase(match.Value.lower) :
                                  match.Value.lower);

                    i += match.Key.Length;
                    continue;
                }

                // Handle special positional cases
                if (i == 0 && sub.StartsWith("ye", StringComparison.OrdinalIgnoreCase))
                {
                    string original = word.Substring(i, 2);
                    result.Append(TextUtils.IsAllUpper(original) || TextUtils.IsTitleCase(original) ? "Ե" : "ե");
                    i += 2;
                    continue;
                }

                if (sub.StartsWith("ev", StringComparison.OrdinalIgnoreCase))
                {
                    string original = word.Substring(i, 2);
                    result.Append(TextUtils.IsAllUpper(original) ? "ԵՎ" :
                                  TextUtils.IsTitleCase(original) ? "Եվ" : "և");
                    i += 2;
                    continue;
                }

                if (i == 0 && sub.StartsWith("vo", StringComparison.OrdinalIgnoreCase))
                {
                    string original = word.Substring(i, 2);
                    result.Append(TextUtils.IsAllUpper(original) || TextUtils.IsTitleCase(original) ? "Ո" : "ո");
                    i += 2;
                    continue;
                }

                if (char.ToLowerInvariant(word[i]) == 'o')
                {
                    string target = (i == 0) ? "օ" : "ո";
                    result.Append(char.IsUpper(word[i]) ? TextUtils.ToTitleCase(target) : target);
                    i++;
                    continue;
                }

                if (char.ToLowerInvariant(word[i]) == 'e')
                {
                    string target = (i == 0) ? "Է" : "ե";
                    result.Append(char.IsUpper(word[i]) ? TextUtils.ToTitleCase(target) : target);
                    i++;
                    continue;
                }

                string key = word[i].ToString().ToLowerInvariant();
                if (TransliterationData.SimpleMap.TryGetValue(key, out var val))
                {
                    result.Append(char.IsUpper(word[i]) ? val.upper : val.lower);
                }
                else
                {
                    result.Append(word[i]);
                }

                i++;
            }

            return result.ToString();
        }
    }
}
