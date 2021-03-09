using System;
using System.Collections.Generic;
using System.Text;

namespace Iyoku.Data
{
    public class ServerUploadInfos
    {
        public ulong Id;
        public List<string> JailCategories;
        public List<string> HellCategories;
        public List<CustomChannel> CustomChannels;
    }

    public class CustomChannel
    {
        public ulong Id;
        public bool Hell;
    }
}
