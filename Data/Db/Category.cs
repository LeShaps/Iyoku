using System;
using System.Collections.Generic;
using System.Text;

namespace Iyoku.Db
{
    class Category
    {
        public string Name;
        public List<string> Content;

        public Category(string Name)
        {
            this.Name = Name;
            Content = new List<string>();
        }
    }
}
