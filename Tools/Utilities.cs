using Discord;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Iyoku.Utilities
{
    class Utilities
    {
        public static T JsonWalker<T>(JObject OriginalJson, string Path)
        {
            string[] SplitedPath = Path.Split('/');
            string ValueName = SplitedPath.Last();
            JObject FinalJson = OriginalJson;

            foreach (string Part in SplitedPath.SkipLast(1))
            {
                if (FinalJson[Part].Type == JTokenType.Array)
                {
                    FinalJson = FinalJson.Value<JArray>(Part)[0].ToObject<JObject>();
                }
                else
                {
                    FinalJson = FinalJson[Part].ToObject<JObject>();
                }
            }

            return FinalJson.Value<T>(ValueName);
        }

        public static Emoji[] MakeEmojiArray(params string[] Emojis)
        {
            List<Emoji> EmojisList = new List<Emoji>();

            foreach (string s in Emojis)
            {
                EmojisList.Add(new Emoji(s));
            }

            return EmojisList.ToArray();
        }

        public static Color ColorFromHex(string HexValue)
        {
            try
            {
                return new Color(uint.Parse(HexValue, System.Globalization.NumberStyles.HexNumber));
            }
            catch
            {
                return new Color(0);
            }
        }
    }
}
