using System;
using System.Collections.Generic;
using System.Text;

namespace Iyoku.Db
{
    class User
    {
        public string Name;
        public List<Collection> Collections;
        private ulong UserID;

        public string id { get => UserID.ToString(); set => ulong.Parse(value); }

        public User(string Name, ulong Id)
        {
            this.Name = Name;
            UserID = Id;
            Collections = new List<Collection>();
        }
    }
}
