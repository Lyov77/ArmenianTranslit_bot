using System.Text;
using System.Text.RegularExpressions;

public static class Transliterator
{
    private static readonly Dictionary<string, (string lower, string upper)> SpecialCombinations = new(StringComparer.OrdinalIgnoreCase)
    {
        ["yev"] = ("և", "ԵՎ"),
        ["zh"] = ("ժ", "Ժ"),
        ["gh"] = ("ղ", "Ղ"),
        ["dz"] = ("ձ", "Ձ"),
        ["ts"] = ("ծ", "Ծ"), // спец-обработка ts в конце
        ["ch"] = ("չ", "Չ"),
        ["sh"] = ("շ", "Շ")
    };

    private static readonly Dictionary<string, (string lower, string upper)> SimpleMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["a"] = ("ա", "Ա"),
        ["b"] = ("բ", "Բ"),
        ["c"] = ("ց", "Ց"),
        ["d"] = ("դ", "Դ"),
        ["e"] = ("է", "Է"), // Изменение для буквы "e"
        ["f"] = ("ֆ", "Ֆ"),
        ["g"] = ("գ", "Գ"),
        ["h"] = ("հ", "Հ"),
        ["i"] = ("ի", "Ի"),
        ["j"] = ("ջ", "Ջ"),
        ["k"] = ("կ", "Կ"),
        ["l"] = ("լ", "Լ"),
        ["m"] = ("մ", "Մ"),
        ["n"] = ("ն", "Ն"),
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

    private static readonly Dictionary<string, (string lower, string upper)> WholeWordMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["e"] = ("է", "Է"),
        ["el"] = ("էլ", "ԷԼ"),
        ["em"] = ("եմ", "ԵՄ"),
        ["es"] = ("ես", "ԵՍ"),
        ["en"] = ("են", "ԵՆ"),
        ["enq"] = ("ենք", "ԵՆՔ"),
        ["eq"] = ("եք", "ԵՔ"),

        // Новые исключения для слова "che"
        ["che"] = ("չէ", "ՉԷ"),
        ["cheinq"] = ("չէինք", "ՉԷԻՆՔ"),
        ["cheir"] = ("չէիր", "ՉԷԻՐ"),
        ["cheiq"] = ("չէիք", "ՉԷԻՔ"),
        ["chein"] = ("չէին", "ՉԷԻՆ"),
        ["cher"] = ("չէր", "ՉԷՐ"),
        ["chei"] = ("չէի", "ՉԷԻ"),

        // Используемые последовательности с "e"
        ["einq"] = ("էինք", "ԷԻՆՔ"),
        ["eir"] = ("էիր", "ԷԻՐ"),
        ["eiq"] = ("էիք", "ԷԻՔ"),
        ["ein"] = ("էին", "ԷԻՆ"),
        ["er"] = ("էր", "ԷՐ"),
        ["ei"] = ("էի", "ԷԻ")
    };

    // Новые последовательности с приоритетом по длине
    private static readonly Dictionary<string, (string lower, string upper)> SpecialSequences = new()
    {
        ["einq"] = ("էինք", "ԷԻՆՔ"),
        ["eir"] = ("էիր", "ԷԻՐ"),
        ["eiq"] = ("էիք", "ԷԻՔ"),
        ["ein"] = ("էին", "ԷԻՆ"),
        ["er"] = ("էր", "ԷՐ"),
        ["ei"] = ("էի", "ԷԻ")
    };

    public static string Transliterate(string input)
    {
        var words = Regex.Split(input, @"(\s+|[^a-zA-Z@]+)");
        var result = new StringBuilder();

        foreach (var word in words)
        {
            if (Regex.IsMatch(word, @"^[a-zA-Z@]+$"))
            {
                result.Append(TransliterateWord(word));
            }
            else
            {
                result.Append(word); // Пробелы, цифры, пунктуация — сохраняем
            }
        }

        return result.ToString();
    }

    private static string TransliterateWord(string word)
    {
        var lower = word.ToLowerInvariant();

        if (WholeWordMap.TryGetValue(lower, out var special))
        {
            return IsAllUpper(word) ? special.upper : special.lower;
        }

        var result = new StringBuilder();
        int i = 0;

        while (i < word.Length)
        {
            string sub = word[i..];

            // Обработка новых последовательностей (er, ei, eir и т.д.) — в порядке убывания длины
            foreach (var seq in SpecialSequences.OrderByDescending(k => k.Key.Length))
            {
                if (sub.StartsWith(seq.Key, StringComparison.OrdinalIgnoreCase))
                {
                    string original = word.Substring(i, seq.Key.Length);
                    bool isUpper = original.All(char.IsUpper);
                    bool isCapital = char.IsUpper(original[0]) && original.Skip(1).All(c => char.IsLower(c));

                    string toAppend = isUpper ? seq.Value.upper :
                                      isCapital ? ToTitleCase(seq.Value.lower) :
                                      seq.Value.lower;

                    result.Append(toAppend);
                    i += seq.Key.Length;
                    goto ContinueLoop;
                }
            }

            // --- ev: если есть заглавная — перевести как ԵՎ, иначе как և ---
            if (sub.Length >= 2 && sub[..2].Equals("ev", StringComparison.OrdinalIgnoreCase))
            {
                string original = word.Substring(i, 2);
                bool isCapital = char.IsUpper(original[0]) || char.IsUpper(original[1]);
                result.Append(isCapital ? "ԵՎ" : "և");
                i += 2;
                continue;
            }

            // --- vo в начале слова → ո ---
            if (i == 0 && sub.StartsWith("vo", StringComparison.OrdinalIgnoreCase))
            {
                string original = word.Substring(i, 2);
                result.Append(IsAllUpper(original) ? "Ո" : "ո");
                i += 2;
                continue;
            }

            // --- ts: в конце слова → տս, иначе → ծ ---
            if (sub.Length >= 2 && sub[..2].Equals("ts", StringComparison.OrdinalIgnoreCase))
            {
                string original = word.Substring(i, 2);
                bool isAtEnd = (i + 2 == word.Length);

                result.Append(isAtEnd
                    ? (IsAllUpper(original) ? "ՏՍ" : "տս")
                    : (IsAllUpper(original) ? "Ծ" : "ծ"));

                i += 2;
                continue;
            }

            // --- другие комбинации (zh, ch, sh, etc) ---
            var match = SpecialCombinations.Keys
                .Where(k => k != "ts")
                .OrderByDescending(k => k.Length)
                .FirstOrDefault(k => sub.StartsWith(k, StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                string original = word.Substring(i, match.Length);
                var val = SpecialCombinations[match];
                result.Append(IsAllUpper(original) ? val.upper : val.lower);
                i += match.Length;
                continue;
            }

            // --- o: в начале слова → օ, иначе → ո ---
            char ch = word[i];
            if (char.ToLowerInvariant(ch) == 'o')
            {
                bool isStart = i == 0;
                string target = isStart ? "օ" : "ո";
                result.Append(char.IsUpper(ch) ? ToTitleCase(target) : target);
                i++;
                continue;
            }

            // --- e: если в начале, то как "Է", иначе как "ե", кроме исключений ---
            if (char.ToLowerInvariant(ch) == 'e')
            {
                bool isStart = i == 0;
                string target = isStart && !IsInWholeWordExceptions(word)
                    ? "Է" // Если "e" в начале и это не исключение
                    : "ե"; // В других случаях "e" — это "ե"
                result.Append(char.IsUpper(ch) ? ToTitleCase(target) : target);
                i++;
                continue;
            }

            // --- простая замена ---
            string key = ch.ToString().ToLowerInvariant();
            if (SimpleMap.TryGetValue(key, out var map))
            {
                result.Append(char.IsUpper(ch) ? map.upper : map.lower);
            }
            else
            {
                result.Append(ch);
            }

            i++;

        ContinueLoop:
            ;
        }

        return result.ToString();
    }

    private static bool IsAllUpper(string input) => input.All(char.IsUpper);

    private static string ToTitleCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        if (input.Length == 1) return input.ToUpperInvariant();
        return char.ToUpperInvariant(input[0]) + input[1..];
    }

    // Проверка, входит ли слово в список исключений для буквы "e"
    private static bool IsInWholeWordExceptions(string word)
    {
        var exceptions = new HashSet<string>(WholeWordMap.Keys, StringComparer.OrdinalIgnoreCase);
        return exceptions.Contains(word.ToLowerInvariant());
    }
}
