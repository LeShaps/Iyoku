using System;
using Discord.Commands;
using Discord;
using System.Diagnostics;
using System.Threading.Tasks;
using Iyoku.Utilities;
using Iyoku.Data;
using Iyoku.Modules;
using Discord.WebSocket;
using System.Linq;

using Iyoku.Db;
using Iyoku.Extensions;

namespace Iyoku
{
    class Program
    {
        
        public readonly CommandService commands = new CommandService();

        public Program()
        {
            Globals.Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            Globals.Client.Log += Loggers.LogEventAsync;
        }

        static async Task Main()
        {
            try
            {
                await new Program().MainAsync().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (Debugger.IsAttached)
                    throw;
                Console.WriteLine(e.ToString());
            }

        }

        public async Task MainAsync()
        {
            await Loggers.LogEventAsync(new LogMessage(LogSeverity.Info, "Init...", "Stating Iyoku")).ConfigureAwait(false);

            Globals.InitConfig();
            await Globals.Db.InitAsync();
            
            await Loggers.LogEventAsync(new LogMessage(LogSeverity.Info, "Setup", "Initializing Modules...")).ConfigureAwait(false);

            await commands.AddModuleAsync<CommunicationModule>(null);
            await commands.AddModuleAsync<CollectionModule>(null);
            await commands.AddModuleAsync<CollectionManagementModule>(null);
            await commands.AddModuleAsync<SourceUploaderModule>(null);

            Globals.Client.MessageReceived += SourceCheck;
            Globals.Client.MessageReceived += HandleMessageAsync;
            Globals.Client.MessageReceived += CollectionModifierCheck;
            Globals.Client.Ready += InitJailChannels;

            Globals.Client.ChannelCreated += CheckNewJailChannel;

            commands.Log += Loggers.LogEventAsync;

            await Globals.Client.LoginAsync(TokenType.Bot, Globals.BotToken);
            await Globals.Client.StartAsync();

            await Task.Delay(-1).ConfigureAwait(false);
        }

        private Task CheckNewJailChannel(SocketChannel arg)
        {
            if (!Globals.SiteUploadConfigured)
                return;
            SourceUploaderModule.UpdateSauceUploadInfos();

            return Task.CompletedTask;
        }

        private Task InitJailChannels()
        {
            SourceUploaderModule.UpdateSauceUploadInfos();
    
            return Task.CompletedTask;
        }

        private async Task SourceCheck(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage msg) || !Globals.SiteUploadConfigured)
                return;

            SocketGuildChannel ChannelForTest = msg.Channel as SocketGuildChannel;
            if (Globals.JailChannels.Contains(ChannelForTest.Id) || Globals.HellChannels.Contains(ChannelForTest.Id))
            {
                await SourceUploaderModule.UploadToSauce(msg, Globals.HellChannels.Contains(ChannelForTest.Id));
            }
        }

        private Task CollectionModifierCheck(SocketMessage arg)
        {
            if (Globals.InConfigCollections.Any(x => x.AwaitUser() == arg.Author.Id)) {
                Collection Config = Globals.InConfigCollections.Where(x => x.AwaitUser() == arg.Author.Id).FirstOrDefault();
                
            }
            // To finish
            throw new NotImplementedException();
        }

        private async Task HandleMessageAsync(SocketMessage arg)
        {
            if (arg.Author.Id == Globals.Client.CurrentUser.Id || arg.Author.IsBot)
                return;

            if (!(arg is SocketUserMessage msg))
                return;
            int pos = 0;
            if (msg.HasMentionPrefix(Globals.Client.CurrentUser, ref pos) || msg.HasStringPrefix("iy.", ref pos))
            {
                var context = new SocketCommandContext(Globals.Client, msg);
                await commands.ExecuteAsync(context, pos, null);
            }
        }
    }
}
