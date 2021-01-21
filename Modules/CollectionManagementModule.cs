using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iyoku.Data;
using Iyoku.Db;

namespace Iyoku.Modules
{
    class CollectionManagementModule : ModuleBase
    {
        [Command("Allow user")]
        public async Task AllowUserAccess(params string[] Args)
        {
            string AccessUser = Args[0];
            string CollectionName = String.Join(' ', Args.Skip(1));

            User Currentuser = await Globals.Db.GetUser(Context.User);
            if (Currentuser.Collections.Any(x => x.Name == CollectionName)) {
                Currentuser.Collections.Where(x => x.Name == CollectionName).FirstOrDefault().AllowedUsers.Add(AccessUser);
            } else {
                await ReplyAsync("The collection you want to allow access to doesn't exist");
                return;
            }
            await Globals.Db.UpdateUser(Currentuser);
        }

        [Command("Remove user")]
        public async Task RemoveUserAccess(params string[] Args)
        {
            string AccessUser = Args[0];
            string CollectionName = String.Join(' ', Args.Skip(1));

            User CurrentUser = await Globals.Db.GetUser(Context.User);
            if (CurrentUser.Collections.Any(x => x.Name == CollectionName)) {
                CurrentUser.Collections.Where(x => x.Name == CollectionName).FirstOrDefault().AllowedUsers.Remove(AccessUser);
            } else {
                await ReplyAsync("The collection you want to allow access to doesn't exist, or the user you want to remove doesn't have access to it");
                return;
            }
            await Globals.Db.UpdateUser(CurrentUser);
        }
    }
}
