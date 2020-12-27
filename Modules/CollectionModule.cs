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
            await ReplyAsync("", false, CreateCollectionName(CustomUser));
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

        private Embed CreateCollectionName(User user)
        {
            List<Collection> UserCollections = user.Collections;

            EmbedBuilder embed = new EmbedBuilder
            {
                Title = "Your collections",
                Description = "Here's a list of your collections\n\n",
                Color = new Color(16234901)
            };

            foreach (Collection col in UserCollections)
            {
                if (col.AllowCurrentDisplay(Context)) {
                    embed.Description += $"**{col.Name}**\n";
                }
            }

            return embed.Build();
        }
    }
}
