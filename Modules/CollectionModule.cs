using Discord.Commands;
using System.Threading.Tasks;
using Iyoku.Data;
using Iyoku.Db;
using Discord;
using System.Collections.Generic;
using System.Linq;

namespace Iyoku.Modules
{
    class CollectionModule : ModuleBase
    {
        [Command("Show my collections")]
        public async Task ShowCollections()
        {
            User CustomUser = null;
            if (!await Globals.Db.UserExists(Context.User.Id.ToString()))
            {
                CustomUser = await Globals.Db.InitUser(Context.User);
                await Globals.Db.AddUserCollection(Context.User, new Collection("Public", CollectionType.Public, CustomUser));
                await Globals.Db.AddUserCollection(Context.User, new Collection(Context.Guild.Name, CollectionType.Server, CustomUser, Context));
                await Globals.Db.AddUserCollection(Context.User, new Collection("Private", CollectionType.Private, CustomUser));
            }

            CustomUser = await Globals.Db.GetUser(Context.User);

            if (CustomUser == null) {
                await ReplyAsync("Something went wrong, I couldn't find you in the db");
                return;
            }
            await ReplyAsync("", false, CreateCollectionName(CustomUser, Context));
        }

        [Command("Create new public Collection")]
        public async Task CreatePublicCollection([Remainder]string CollectionName)
        {
            User CurrentUser = await Globals.Db.GetUser(Context.User);
            if (CurrentUser == null) {
                await ReplyAsync("Something when wrong, I couldn't find you in the db");
                return;
            }

            if (CurrentUser.Collections.Any(x => x.Name == CollectionName && x.Type == CollectionType.Public)) {
                await ReplyAsync("You can't create two public Collections with the same name");
                return;
            }
            await Globals.Db.AddUserCollection(Context.User, new Collection(CollectionName, CollectionType.Public, CurrentUser));

            await ReplyAsync($"Your new public collection {CollectionName} as been succesfully created");
        }

        [Command("Create new guild Collection")]
        public async Task CreateGuildCollection([Remainder]string Name)
        {
            User CurrentUser = await Globals.Db.GetUser(Context.User);
            if (CurrentUser == null)
            {
                await ReplyAsync("Something when wrong, I couldn't find you in the db");
                return;
            }

            if (CurrentUser.Collections.Any(x => x.Name == Name && x.Type == CollectionType.Server))
            {
                await ReplyAsync("You can't create two guild Collections with the same name");
                return;
            }

            await Globals.Db.AddUserCollection(Context.User, new Collection(Name, CollectionType.Server, CurrentUser, Context));

            await ReplyAsync($"Your new Guild Collection {Name} as been succesfully created");
        }

        [Command("Create new private Collection")]
        public async Task CreatePrivateCollection([Remainder]string Name)
        {
            User CurrentUser = await Globals.Db.GetUser(Context.User);
            if (CurrentUser == null)
            {
                await ReplyAsync("Something when wrong, I couldn't find you in the db");
                return;
            }

            if (CurrentUser.Collections.Any(x => x.Name == Name && x.Type == CollectionType.Private))
            {
                await ReplyAsync("You can't create two private Collections with the same name");
                return;
            }

            await Globals.Db.AddUserCollection(Context.User, new Collection(Name, CollectionType.Private, CurrentUser));

            await ReplyAsync($"Your new private Collection {Name} as been succesfully created");
        }

        [Command("Delete collection")]
        public async Task DeleteCollection([Remainder]string CollectionName)
        {
            User CurrentUser = await Globals.Db.GetUser(Context.User);
            if (CurrentUser == null) {
                await ReplyAsync("Something went wrong, I can't find you in the db");
                return;
            }

            if (!CurrentUser.Collections.Any(x => x.Name == CollectionName)) {
                await ReplyAsync("The collection you want to delete doesn't exist");
                return;
            }

            CurrentUser.Collections.Remove(CurrentUser.Collections.Where(x => x.Name == CollectionName).FirstOrDefault());
            await Globals.Db.UpdateUser(CurrentUser);

            await ReplyAsync("The collection have been sucessfully removed");
        }

        [Command("Display collections of")]
        public async Task DisplayOtherConnection([Remainder]string Name)
        {
            User BasicUser = await Globals.Db.GetUser(Name);

            if (BasicUser == null) {
                await ReplyAsync("I couldn't find this user in the db");
                return;
            }

            await ReplyAsync("", false, CreateCollectionName(BasicUser, Context));
        }

        public static Embed CreateCollectionName(User user, ICommandContext Context)
        {
            List<Collection> UserCollections = user.Collections;
            List<Collection> WriteCollections = user.Collections.Where(x => x.AllowCurrentDisplay(Context)).ToList();

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "Your collections",
                Description = "Here's a list of your collections:",
                Color = new Color(16234901)
            };

            embed.Fields = new List<EmbedFieldBuilder>();
            string PublicCollections = WriteCollectionString(WriteCollections, CollectionType.Public);
            string GuildCollections = WriteCollectionString(WriteCollections, CollectionType.Server);
            string PrivateCollections = WriteCollectionString(WriteCollections, CollectionType.Private);

            if (PublicCollections != null)
            {
                embed.Fields.Add(new EmbedFieldBuilder
                {
                    Name = "Public",
                    Value = PublicCollections,
                    IsInline = true
                });
            }
            if (GuildCollections != null)
            {
                embed.Fields.Add(new EmbedFieldBuilder
                {
                    Name = "Guild",
                    Value = GuildCollections,
                    IsInline = true
                });
            }
            if (PrivateCollections != null)
            {
                embed.Fields.Add(new EmbedFieldBuilder
                {
                    Name = "Private",
                    Value = PrivateCollections,
                    IsInline = true
                });
            }

            return embed.Build();
        }

        private static string WriteCollectionString(List<Collection> collections, CollectionType type)
        {
            string Result = null;

            foreach (Collection col in collections.Where(x => x.Type == type))
            {
                Result += $"{col.Name}\n";
            }

            return Result;
        }
    }
}
