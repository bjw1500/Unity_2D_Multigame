using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();

        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        int _playerId = 0;

        public Player Add()
        {
            Player player = new Player();

            lock (_lock)
            {
                player.Info.PlayerId = _playerId;
                _players.Add(_playerId, player);
                _playerId++;
            }

            return player;

        }

        public Player MonsterPlayerAdd()
        {
            Player player = new Player();

            lock (_lock)
            {
                player.Info.PlayerId = 9;
                _players.Add(player.Info.PlayerId, player);
            }

            return player;

        }

        public bool Remove(int playerId)
        {
            lock (_lock)
            {
                return _players.Remove(playerId);
            }


        }

        public Player Find(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.TryGetValue(playerId, out player))
                    return player;

                return null;

            }
        }


    }
}
