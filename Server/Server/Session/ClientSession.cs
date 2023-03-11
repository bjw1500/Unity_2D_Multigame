using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using Google.Protobuf;

namespace Server
{
	public class ClientSession : PacketSession
	{
		public Player MyPlayer { get; set; }
		public int SessionId { get; set; }

		public void Send(IMessage packet)
        {
			string msgName = packet.Descriptor.Name.Replace("_", string.Empty);

			MsgId msgid = (MsgId)Enum.Parse(typeof(MsgId), msgName);

			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes((ushort)size + 4), 0, sendBuffer, 0, sizeof(ushort));
			Array.Copy(BitConverter.GetBytes((ushort)msgid), 0, sendBuffer, 2, sizeof(ushort));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

			Send(new ArraySegment<byte>(sendBuffer));

		}

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			MyPlayer = PlayerManager.Instance.Add();
            {
				MyPlayer.Info.Name = $"Player_{MyPlayer.Info.PlayerId}";
				MyPlayer.Session = this;
			}

			
			//일단 대기실에 넣는다.
			WaitRoom waitRoom = RoomManager.Instance.WaitRoomFind(1);
			waitRoom.EnterWaitRoom(MyPlayer);


			//룸에 사람이 모이고, 플레이 버튼을 누르면 게임을 시작한다.

			//패킷을 받는다. 시작패킷.


			//GameRoom gameRoom = RoomManager.Instance.Find(1);
			//gameRoom.Push(gameRoom.EnterGame, MyPlayer);

		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			if (MyPlayer.Room != null)
			{
				GameRoom gameRoom = MyPlayer.Room;
				gameRoom.Push(gameRoom.LeaveGame, MyPlayer.Info.PlayerId);
			}

			if (MyPlayer.WaitRoom != null)
			{
				WaitRoom waitRoom = MyPlayer.WaitRoom;
				waitRoom.LeaveGame(MyPlayer.Info.PlayerId);
			}

			SessionManager.Instance.Remove(this);

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
