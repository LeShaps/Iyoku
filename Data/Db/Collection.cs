using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Iyoku.Data;

namespace Iyoku.Db
{
    class Collection
    {
        /* Content */
        public string Name;
        public List<Category> Categories;
        public CollectionType Type;

        /* Moderation */
        public string OwnerId;
        public string OwnerStr;
        public string OriginGuild;

        /* Restrictions */
        public List<string> AllowedUsers;
        public List<string> AllowedGuilds;
        public List<string> AllowedRoles;

        public Collection() { }

        public Collection(string Name, CollectionType Type, User User, ICommandContext Context = null)
        {
            this.Name = Name;
            this.Type = Type;

            OwnerStr = User.Name;
            OwnerId = User.id;
            if (Context != null && Context.Guild != null)
                OriginGuild = Context.Guild.Id.ToString();

            AllowedGuilds = new List<string>();
            AllowedUsers = new List<string>();
            AllowedRoles = new List<string>();
        }

        private ulong Oid { get => ulong.Parse(OwnerId); }

        /* Display restrictions checks */
        public bool AllowCurrentDisplay(ICommandContext Context)
        {
            if (Type == CollectionType.Private && Context.Guild != null)
                return false;

            if (Type == CollectionType.Private && Context.Guild == null && Context.User.Id != Oid) {
                if (AllowedUsers.Contains(Context.User.Id.ToString())) {
                    return true;
                } else {
                    return false;
                }
            }

            if (Type == CollectionType.Server && Context.Guild != null) {
                if (Context.Guild.Id.ToString() == OriginGuild || AllowedGuilds.Contains(Context.Guild.Id.ToString())) {
                    return true;
                } else {
                    return false;
                }
            }

            return true;
        }

        /* Access restrictions checks */
        public async Task<bool> AllowUserAccess(ICommandContext Context)
        {
            if (Type == CollectionType.Public || Context.User.Id == Oid) {
                return true;
            }

            if (Type == CollectionType.Server && AllowedRoles.Count != 0) {
                return await CheckServerRestriction(Context);
            } else if (Type == CollectionType.Server && AllowedUsers.Count == 0) {
                return true;
            } else {
                return AllowedUsers.Contains(Context.User.Id.ToString());
            }
        }

        public async Task<bool> CheckServerRestriction(ICommandContext Context) {
            IGuildUser User = await Context.Guild.GetUserAsync(Context.User.Id);

            if (AllowedRoles.Any(r => User.RoleIds.Contains(ulong.Parse(r)))) return true;
            else
                return false;
        }
    }
}
