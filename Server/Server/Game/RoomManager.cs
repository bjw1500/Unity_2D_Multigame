using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class RoomManager
    {
        public static RoomManager Instance {get;} = new RoomManager();

        object _lock = new object();
        Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
        Dictionary<int, WaitRoom> _waitRooms = new Dictionary<int, WaitRoom>();


        int _roomId = 1;
        int _waitRoomId = 1;

        public GameRoom Add(int mapId = 2)
        {
            GameRoom gameRoom = new GameRoom();
            //gameRoom.Init(mapId);
            gameRoom.Push(gameRoom.Init, mapId);

            lock(_lock)
            {
                gameRoom.RoomId = _roomId;
                _rooms.Add(_roomId, gameRoom);
                _roomId++;
            }

            return gameRoom;

        }

        public WaitRoom WaitRoomAdd()
        {
            WaitRoom gameRoom = new WaitRoom();
            //gameRoom.Init(mapId);
            
            lock (_lock)
            {
                gameRoom.RoomId = _waitRoomId;
                _waitRooms.Add(_waitRoomId, gameRoom);
                _waitRoomId++;
            }

            return gameRoom;

        }

        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                return _rooms.Remove(roomId);
            }


        }

        public GameRoom Find(int roomId)
        {
            lock(_lock)
            {
                GameRoom room = null;
                if (_rooms.TryGetValue(roomId, out room))
                    return room;

                return null;
            }
        }

        public WaitRoom WaitRoomFind(int roomId)
        {
            lock (_lock)
            {
                WaitRoom room = null;
                if (_waitRooms.TryGetValue(roomId, out room))
                    return room;

                return null;
            }
        }


    }
}
