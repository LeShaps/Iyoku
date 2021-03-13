using Discord;
using Discord.Commands;
using Iyoku.Data;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Iyoku.Extensions;
using Discord.WebSocket;
using System.Linq;

namespace Iyoku.Modules
{
    class SourceUploaderModule : ModuleBase
    {
        [Command("Get info from")]
        public async Task GetSauceFrom([Remainder]string Url)
        {
            await ReplyAsync(await GetSource(Url));
        }

        public static void UpdateSauceUploadInfos()
        {
            if (!(JsonConvert.DeserializeObject<JObject>(File.ReadAllText("Loggers/SauceUploads.json")) is JObject SauceConfig))
            {
                Globals.SiteUploadConfigured = false;
                return;
            }

            JArray InfosToGet = SauceConfig.Value<JArray>("Servers");

            foreach (JObject server in InfosToGet.Children())
            {
                AddUploadChannels(server.ToObject<ServerUploadInfos>());
            }
        }

        public async static Task<string> GetSource(string url)
        {
            var html = await Globals.Asker.GetStringAsync($"https://saucenao.com/search.php?db=999&url={Uri.EscapeDataString(url)}");
            if (!html.Contains("<div id=\"middle\""))
                return null;

            var subHTML = html.Split(new[] { "<td class=\"resulttablecontent\">" }, StringSplitOptions.None)[1];
            var compatibility = float.Parse(Regex.Match(subHTML, "<div class=\"resultsimilarityinfo\">([0-9]{2,3}\\.[0-9]{1,2})%<\\/div>").Groups[1].Value, CultureInfo.InvariantCulture);
            string PossibleResult = Regex.Match(subHTML, "<a href=\"(.*?)\">").Groups[1].Value;
            string ConvertedResult = PossibleResult.Split(new[] { "\"" }, StringSplitOptions.None)[0];

            if (compatibility > 80)
                return ConvertedResult;
            else
                return "https://pm1.narvii.com/7081/1a5ebacb749c39000e4052a9ceb198dd69735996r1-1152-1584v2_hq.jpg";
        }

        public async static Task UploadToSauce(IMessage message, bool Hell)
        {
            var images = message.Attachments;
            
            foreach(var img in images)
            {
                await MakeUpload(img.Url, message.Channel.Name.Replace("-", " "), Hell);
            }
            if (message.Embeds.Count > 0)
            {
                foreach (Embed emb in message.Embeds)
                {
                    await MakeUpload(emb.Image.Value.Url, message.Channel.Name.Replace("-", " "), Hell);
                }
            }
        }

        private async static Task MakeUpload(string Url, string ChannelName, bool Hell)
        {
            try
            {
                var infos = new SourceInfos
                {
                    image = Url.Replace(":large", ""),
                    channel = ChannelName,
                    source = await GetSource(Url),
                    hell = Hell
                };

                var payload = JsonConvert.SerializeObject(infos);

                var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

                var result = await Globals.Asker.PostAsync("https://sauce.shaps.work/upload.php", httpContent);
            }
            catch (HttpRequestException e)
            {
                if (e.Message.Contains("429"))
                {
                    await Task.Delay(500);
                    await MakeUpload(Url, ChannelName, Hell);
                }
            }
        }

        private static void AddUploadChannels(ServerUploadInfos Server)
        {
            SocketGuild Guild = Globals.Client.GetGuild(Server.Id);

            Server.JailCategories
                .ForEach(jail => Guild.GetChannelsOfCategory(jail)
                .ForEach(channel => Globals.JailChannels.Add(channel.Id)));

            Server.HellCategories
                .ForEach(hell => Guild.GetChannelsOfCategory(hell)
                .ForEach(channel => Globals.HellChannels.Add(channel.Id)));

            Server.CustomChannels
                .ForEach(custom => CheckCustomChannel(custom));

            Server.ExcludedChannels
                .Where(ignore => Globals.JailChannels.Contains(ignore))
                .ToList()
                .ForEach(ignore => Globals.JailChannels.Remove(ignore));

            Server.ExcludedChannels
                .Where(ignore => Globals.HellChannels.Contains(ignore))
                .ToList()
                .ForEach(ignore => Globals.HellChannels.Remove(ignore));
        }

        private static void CheckCustomChannel(CustomChannel chan)
        {
            if (chan.Hell)
                Globals.HellChannels.Add(chan.Id);
            else
                Globals.JailChannels.Add(chan.Id);
        }
    }
}
