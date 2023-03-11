using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Server
{
    public class WaitRoom
    {
        object _lock = new object();

        bool _startFlag = true;
        public int RoomId { get; set; }
        public Dictionary<int, Player> _players = new Dictionary<int, Player>();

        public List<bool> _checkSlot = new List<bool>();
        //플레이어의 슬롯 카운트를 센다?

        public void CheckSlotCount(Player player)
        {
            //처음에는 1로 시작한다.
            //1~4까지. 슬롯을 확인했을 때 비어 있으면 슬롯을 부여한다?
            foreach(bool emptySlot in _checkSlot)
            {
                if(emptySlot == false)
                {
                    //플레이어의 슬롯 넣어주기.




                }
            }
        }

        public void EnterWaitRoom(Player player)
        {
            lock (_lock)
            {
                //방 안에 들어오면,
                _players.Add(player.Info.PlayerId, player);
                player.WaitRoom = this;

                //처음 확인용.
                S_EnterWaitingRoom Packet = new S_EnterWaitingRoom();
                player.Session.Send(Packet);

                Thread.Sleep(200);

                //이미 누가 들어와 있는지 확인하고,

                foreach (Player enterPlayer in _players.Values)
                {
                    //패킷을 받는다.
                    //어떤 패킷을 받아야하는가?
                    //플레이어의 정보가 담긴 패킷.
                    //받고나서 클라이언트의 플레이어의 목록에 추가한다.
                    if (enterPlayer == player)
                        continue;

                    S_EnterWaitingRoom enterPacket = new S_EnterWaitingRoom();
                    enterPacket.Player = enterPlayer.Info;
                    player.Session.Send(enterPacket);

                }


                //패킷을 뿌려야지.

                //누가누가 들어와 있는가.
                foreach (Player enterPlayer in _players.Values)
                {

                    //보내진 패킷은 기존에 있던 플레이어들의 목록에 추가.

                    S_EnterWaitingRoom enterPacket = new S_EnterWaitingRoom();
                    enterPacket.Player = player.Info;
                    if (enterPlayer == player)
                        enterPacket.MyPlayer = true;
                    else
                        enterPacket.MyPlayer = false;

                    enterPlayer.Session.Send(enterPacket);


                }

                //TODO UNITY 대기방 만들기

            }


        }


        public void StartGame()
        {
            lock (_lock)
            {
                if (_startFlag == false)
                    return;

                _startFlag = true;


                //플레이어들을 게임 룸에 입장시킨다.
                GameRoom gameRoom = RoomManager.Instance.Find(1);

                foreach (Player player in _players.Values)
                {
                    gameRoom.Push(gameRoom.EnterGame, player);
                    S_StartGame start = new S_StartGame();
                    start.Slot = player.Info.Slot;
                    player.Session.Send(start);


                }

                
                //BroadCast(start);
            }
            
        }

        public void LeaveGame(int playerId)
        {

            Player player = null;
            if (_players.Remove(playerId, out player) == false)
                return;

            player.WaitRoom = null;

            //타인한테 정보 전송
            {
                S_LeaveWaitingRoom leave = new S_LeaveWaitingRoom();
                leave.Player = player.Info;
                BroadCast(leave);
            }

        }
        public void BroadCast(IMessage Packet)
        {


            foreach (Player p in _players.Values)
            {

                p.Session.Send(Packet);
            }


        }


    }
}
