using Discord;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Make a HTML text reading-friendly
        /// </summary>
        /// <param name="WebString">A HTML text</param>
        /// <returns>The text human-friendly</returns>
        public static string Clarify(string WebString)
        {
            if (WebString == null)
            {
                return null;
            }
            return GetPlainTextFromHtml(WebUtility.HtmlDecode(WebString));
        }

        /// <summary>
        /// Get plain text from HTML's one using Regex
        /// </summary>
        /// <param name="htmlString">An HTML string</param>
        /// <returns>The plain text of the HTML string</returns>
        public static string GetPlainTextFromHtml(string htmlString)
        {
            if (htmlString == null)
            {
                return null;
            }

            const string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);

            return htmlString;
        }
    }
}
