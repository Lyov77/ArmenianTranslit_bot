using System.Text;
using System.Text.RegularExpressions;

public static class Transliterator
{
    private static string NormalizeApostrophes(string input)
    {
        // Replace all apostrophe-like symbols with standard ASCII apostrophe '
        char[] apostrophes = { '’', '‘', '՚', '՛', 'ˈ' };
        foreach (var ch in apostrophes)
        {
            input = input.Replace(ch, '\'');
        }
        return input;
    }

    // Library for word exceptions
    private static readonly Dictionary<string, (string lower, string upper)> WordLibrary = new(StringComparer.OrdinalIgnoreCase)
    {
        ["em"] = ("եմ", "ԵՄ"),
        ["es"] = ("ես", "ԵՍ"),
        ["enq"] = ("ենք", "ԵՆՔ"),
        ["eq"] = ("եք", "ԵՔ"),
        ["en"] = ("են", "ԵՆ"),

        ["che"] = ("չէ", "ՉԷ"),
        ["chei"] = ("չէի", "ՉԷԻ"),
        ["cheir"] = ("չէիր", "ՉԷԻՐ"),
        ["cher"] = ("չէր", "ՉԷՐ"),
        ["cheinq"] = ("չէինք", "ՉԷԻՆՔ"),
        ["cheiq"] = ("չէիք", "ՉԷԻՔ"),
        ["chein"] = ("չէին", "ՉԷԻՆ"),

        ["ov"] = ("ով", "ՈՎ"),
        ["ovqer"] = ("ովքեր", "ՈՎՔԵՐ"),

        ["incheve"] = ("ինչևէ", "ԻՆՉԵՎԷ"),
        ["voreve"] = ("որևէ", "ՈՐԵՎԷ")
    };

    private static readonly Dictionary<string, (string lower, string upper)> SpecialCombinations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["p'"] = ("փ", "Փ"),
        ["t'"] = ("թ", "Թ"),
        ["ch'"] = ("ճ", "Ճ"),
        ["c'"] = ("ծ", "Ծ"),
        ["r'"] = ("ռ", "Ռ"),

        ["yev"] = ("և", "ԵՎ"),
        ["zh"] = ("ժ", "Ժ"),
        ["gh"] = ("ղ", "Ղ"),
        ["dz"] = ("ձ", "Ձ"),
        ["ch"] = ("չ", "Չ"),
        ["sh"] = ("շ", "Շ")
    };

    private static readonly Dictionary<string, (string lower, string upper)> SimpleMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["a"] = ("ա", "Ա"),
        ["b"] = ("բ", "Բ"),
        ["c"] = ("ց", "Ց"),
        ["d"] = ("դ", "Դ"),
        ["e"] = ("ե", "Ե"),
        ["f"] = ("ֆ", "Ֆ"),
        ["g"] = ("գ", "Գ"),
        ["h"] = ("հ", "Հ"),
        ["i"] = ("ի", "Ի"),
        ["j"] = ("ջ", "Ջ"),
        ["k"] = ("կ", "Կ"),
        ["l"] = ("լ", "Լ"),
        ["m"] = ("մ", "Մ"),
        ["n"] = ("ն", "Ն"),
        ["o"] = ("ո", "Ո"),
        ["p"] = ("պ", "Պ"),
        ["q"] = ("ք", "Ք"),
        ["r"] = ("ր", "Ր"),
        ["s"] = ("ս", "Ս"),
        ["t"] = ("տ", "Տ"),
        ["u"] = ("ու", "Ու"),
        ["v"] = ("վ", "Վ"),
        ["x"] = ("խ", "Խ"),
        ["y"] = ("յ", "Յ"),
        ["z"] = ("զ", "Զ"),
        ["@"] = ("ը", "Ը")
    };

    public static string Transliterate(string input)
    {
        input = NormalizeApostrophes(input);

        var words = Regex.Split(input, @"(\s+|[^a-zA-Z@']+)");
        var result = new StringBuilder();

        foreach (var word in words)
        {
            if (Regex.IsMatch(word, @"^[a-zA-Z@']+$"))
            {
                result.Append(TransliterateWord(word));
            }
            else
            {
                result.Append(word); // Punctuation, spaces, numbers, symbols
            }
        }

        return result.ToString();
    }

    private static string TransliterateWord(string word)
    {
        var lower = word.ToLowerInvariant();

        // Word exception handling
        if (WordLibrary.TryGetValue(lower, out var lib))
        {
            if (IsAllUpper(word))
                return lib.upper;
            if (IsTitleCase(word))
                return ToTitleCase(lib.lower);
            return lib.lower;
        }

        var result = new StringBuilder();
        int i = 0;

        while (i < word.Length)
        {
            string sub = word[i..];

            // Special combinations
            var match = SpecialCombinations
                .OrderByDescending(kv => kv.Key.Length)
                .FirstOrDefault(kv => sub.StartsWith(kv.Key, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(match.Key))
            {
                string original = word.Substring(i, match.Key.Length);
                if (IsAllUpper(original))
                    result.Append(match.Value.upper);
                else if (IsTitleCase(original))
                    result.Append(ToTitleCase(match.Value.lower));
                else
                    result.Append(match.Value.lower);

                i += match.Key.Length;
                continue;
            }

            // ye: as the first letters of the word → ե/Ե
            if (i == 0 && sub.StartsWith("ye", StringComparison.OrdinalIgnoreCase))
            {
                string original = word.Substring(i, 2);
                result.Append(IsAllUpper(original) ? "Ե" : IsTitleCase(original) ? "Ե" : "ե");
                i += 2;
                continue;
            }

            // "ev" special case
            if (sub.StartsWith("ev", StringComparison.OrdinalIgnoreCase))
            {
                string original = word.Substring(i, 2);
                bool isCapital = IsAllUpper(original) || IsTitleCase(original);
                result.Append(isCapital ? "Եվ" : "և");
                i += 2;
                continue;
            }

            // "vo" as word start
            if (i == 0 && sub.StartsWith("vo", StringComparison.OrdinalIgnoreCase))
            {
                string original = word.Substring(i, 2);
                result.Append(IsAllUpper(original) ? "Ո" : IsTitleCase(original) ? "Ո" : "ո");
                i += 2;
                continue;
            }

            // "o" at beginning → օ, else → ո
            if (char.ToLowerInvariant(word[i]) == 'o')
            {
                bool isStart = i == 0;
                string target = isStart ? "օ" : "ո";
                result.Append(char.IsUpper(word[i]) ? ToTitleCase(target) : target);
                i++;
                continue;
            }

            // "e" at beginning → Է, else → ե
            if (char.ToLowerInvariant(word[i]) == 'e')
            {
                bool isStart = i == 0;
                string target = isStart ? "Է" : "ե";
                result.Append(char.IsUpper(word[i]) ? ToTitleCase(target) : target);
                i++;
                continue;
            }

            // Simple letter map
            string key = word[i].ToString().ToLowerInvariant();
            if (SimpleMap.TryGetValue(key, out var val))
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

    private static bool IsAllUpper(string input) => input.All(char.IsUpper);

    private static bool IsTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        // Remove symbols before the check
        string clean = new string(input.Where(char.IsLetter).ToArray());

        return clean.Length > 0 &&
               char.IsUpper(clean[0]) &&
               clean.Skip(1).All(char.IsLower);
    }
    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        if (input.Length == 1) return input.ToUpperInvariant();
        return char.ToUpperInvariant(input[0]) + input[1..];
    }
}
