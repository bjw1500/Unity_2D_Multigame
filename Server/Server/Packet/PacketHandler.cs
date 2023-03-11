using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Data;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move MovePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;


		//멀티쓰레드 오류 방지
		//어디선가 MyPlayer을 Null로 밀어줘도, player는 계속 참조하고 있기 때문에 크래쉬 문제 해결에 도움이 됨.
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;
		GameRoom gameRoom = clientSession.MyPlayer.Room;
		if (gameRoom == null)
			return;


		gameRoom.Push(gameRoom.HandleMove, player, MovePacket);


	}

	public static void C_SkillHandler(PacketSession session, IMessage packet)
	{
		C_Skill skillPacket = packet as C_Skill;
		ClientSession clientSession = session as ClientSession;

		//멀티쓰레드 오류 방지
		//어디선가 MyPlayer을 Null로 밀어줘도, player는 계속 참조하고 있기 때문에 크래쉬 문제 해결에 도움이 됨.
		Player player = clientSession.MyPlayer;
		if (player == null)
			return;
		GameRoom gameRoom = clientSession.MyPlayer.Room;
		if (gameRoom == null)
			return;

		gameRoom.Push(gameRoom.HandleSkill, player, skillPacket);
	}

	public static void C_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		C_ChangeHp changeHpPacket = packet as C_ChangeHp;
		ClientSession clientSession = session as ClientSession;

		//멀티쓰레드 오류 방지
		//어디선가 MyPlayer을 Null로 밀어줘도, player는 계속 참조하고 있기 때문에 크래쉬 문제 해결에 도움이 됨.

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		GameRoom gameRoom = clientSession.MyPlayer.Room;
		if (gameRoom == null)
			return;

		gameRoom.Push(gameRoom.HandleHp, player, changeHpPacket);
	}



	public static void C_SpawnHandler(PacketSession session, IMessage packet)
	{
		C_Spawn SpawnPacket = packet as C_Spawn;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;
		GameRoom gameRoom = clientSession.MyPlayer.Room;
		if (gameRoom == null)
			return;

		foreach (var unit in SpawnPacket.Info)
		{
			Console.WriteLine(unit.Name + "이 생성 되었습니다.");
		}


		gameRoom.Push(gameRoom.HandleSpawn, player, SpawnPacket);

	}

	public static void C_DespawnHandler(PacketSession session, IMessage packet)
	{
		C_Despawn DespawnPacket = packet as C_Despawn;
		ClientSession clientSession = session as ClientSession;


		Player player = clientSession.MyPlayer;
		if (player == null)
			return;
		GameRoom gameRoom = clientSession.MyPlayer.Room;
		if (gameRoom == null)
			return;

		foreach (ObjectInfo obj in DespawnPacket.Info)
		{
			//만약 나중에 여기에 오류가 생기면
			//유저 아디랑 유닛 아디 잘 확인해보기.

			GameObject despawnObejct = null;
			if (player.PlayerUnits.TryGetValue(obj.UnitId, out despawnObejct) == false)
				continue;

			gameRoom._map.ApplyLeave(despawnObejct);
			player.PlayerUnits.Remove(obj.UnitId);
		}

		S_Despawn S_Packet = new S_Despawn();
		foreach (ObjectInfo obj in DespawnPacket.Info)
		{
			S_Packet.Info.Add(obj);
		}


		gameRoom.Push(gameRoom.BroadCast, S_Packet);

		//RoomManager.Instance.Find(1).BroadCast(S_Packet);

	}

	public static void C_StartGameHandler(PacketSession session, IMessage packet)
	{
		C_StartGame startPacket = packet as C_StartGame;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;
		if (player == null)
			return;

		WaitRoom room = player.WaitRoom;
		if (room == null)
			return;

		player.Info.Slot = startPacket.Slot;

		room.StartGame();





	}


}
