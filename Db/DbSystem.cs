using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;

namespace Iyoku.Db
{
    class DbSystem
    {
        public RethinkDB R1 { get; }

        private Connection _conn;
        private const string _dbName = "IyokuDb";

        private const string _usersTableName = "Users";

        public DbSystem()
        {
            R1 = RethinkDB.R;
        }

        public async Task InitAsync()
            => await InitAsync(_dbName);

        public async Task InitAsync(string DbName)
        {
            _conn = await R1.Connection().ConnectAsync();
            if (!await R1.DbList().Contains(DbName).RunAsync<bool>(_conn))
            {
                R1.DbCreate(DbName).Run(_conn);
            }
            if (!await R1.Db(DbName).TableList().Contains(_usersTableName).RunAsync<bool>(_conn))
            {
                R1.Db(DbName).TableCreate(_usersTableName).Run(_conn);
            }
        }

        public async Task<bool> UserExists(string UserID)
        {
            return !await R1.Db(_dbName).Table(_usersTableName).GetAll(UserID).Count().Eq(0).RunAsync<bool>(_conn);
        }

        public async Task<User> InitUser(IUser User)
        {
            string UserID = User.Id.ToString();
            if (!await UserExists(UserID))
            {
                User NewUser = new User(User.Username, User.Id);

                await R1.Db(_dbName).Table(_usersTableName).Insert(NewUser).RunAsync(_conn);
            }

            return await R1.Db(_dbName).Table(_usersTableName).Get(UserID).RunAsync<User>(_conn);
        }

        public async Task AddUserCollection(IUser User, Collection collection)
        {
            if (await UserExists(User.Id.ToString()))
            {
                User UpdatedUser = await R1.Db(_dbName).Table(_usersTableName).Get(User.Id.ToString()).RunAsync<User>(_conn);
                UpdatedUser.Collections.Add(collection);
                await R1.Db(_dbName).Table(_usersTableName).Update(UpdatedUser).RunAsync(_conn);
            }
        }
        
        public async Task UpdateUser(User UpdatedUser)
        {
            await R1.Db(_dbName).Table(_usersTableName).Update(UpdatedUser).RunAsync(_conn);
        }

        public async Task<User> GetUser(IUser User)
        {
            if (!await UserExists(User.Id.ToString())) return null;

            return await R1.Db(_dbName).Table(_usersTableName).Get(User.Id.ToString()).RunAsync<User>(_conn);
        }

        public async Task<User> GetUser(string Username)
        {
            if(!await R1.Db(_dbName).Table(_usersTableName).Filter(x => x["Name"]).Count().Eq(0).RunAsync<bool>(_conn))
            {
                var Results = await R1.Db(_dbName).Table(_usersTableName).Filter(x => x["Name"]).RunResultAsync<User[]>(_conn);
                User FinalUser = Results[0];
                return FinalUser;
            }

            return null;
        }
    }
}
