using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using Iyoku.Db;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Discord.WebSocket;

namespace Iyoku.Data
{
    class Globals
    {
        /* Config */
        public static string BotToken { get; private set; }
        public static DiscordSocketClient Client;

        /* DB and behaviour */
        public static DbSystem Db = new DbSystem();
        public static HttpClient Asker = new HttpClient();

        /* Configuration */
        public static List<Collection> InConfigCollections = new List<Collection>();
        public static List<ulong> JailChannels = new List<ulong>();
        public static List<ulong> HellChannels = new List<ulong>();
        public static bool SiteUploadConfigured = true;
        
        public static void InitConfig()
        {
            if (!File.Exists("Loggers/Credentials.json"))
                throw new FileNotFoundException($"You must have a \"Credentials.json\" file located in {AppDomain.CurrentDomain.BaseDirectory}Loggers");
            JObject ConfigurationJson = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("Loggers/Credentials.json"));
            if (ConfigurationJson["botToken"] == null || ConfigurationJson["ownerId"] == null || ConfigurationJson["ownerStr"] == null)
                throw new FileNotFoundException("Missing critical informations in Credentials.json, please complete mandatory informations before continuing");

            BotToken = ConfigurationJson.Value<string>("botToken");
        }
    }

    [Serializable]
    public enum CollectionType
    {
        Public,
        Server,
        Private
    }
}
