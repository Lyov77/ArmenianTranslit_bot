using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmenianTranslit_bot
{
    public static class TransliterationData
    {
        public static readonly Dictionary<string, (string lower, string upper)> WordLibrary = new(StringComparer.OrdinalIgnoreCase)
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
            ["voreve"] = ("որևէ", "ՈՐԵՎԷ"),

            ["xzhdzhut'yun"] = ("խժդժություն", "ԽԺԴԺՈՒԹՅՈՒՆ"),
            ["dzhkam"] = ("դժկամ", "ԴԺԿԱՄ")
        };

        public static readonly Dictionary<string, (string lower, string upper)> SpecialCombinations = new(StringComparer.OrdinalIgnoreCase)
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

        public static readonly Dictionary<string, (string lower, string upper)> SimpleMap = new(StringComparer.OrdinalIgnoreCase)
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
    }
}
