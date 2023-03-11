using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo();
        public GameRoom Room { get; set; }
        public WaitRoom WaitRoom { get; set; }

        public ClientSession Session { get; set; }

        public Dictionary<int, GameObject> PlayerUnits { get; set; } = new Dictionary<int, GameObject>();




    }
}
