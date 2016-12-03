﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamFriendsWatcher
{
    class Friend
    {
        public String name;
        public String steamid;
        public Int64 friendsince;

        public override string ToString()
        {
            return $"{name} SteamID: {steamid} Friends Since: {friendsince}";
        }
    }
}
