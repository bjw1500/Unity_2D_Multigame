using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public Action<PacketSession, IMessage, ushort> CustomHandler {get; set;}

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_EnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<S_Spawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.S_SpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<S_Despawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.S_DespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SSkill, MakePacket<S_Skill>);
		_handler.Add((ushort)MsgId.SSkill, PacketHandler.S_SkillHandler);		
		_onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<S_ChangeHp>);
		_handler.Add((ushort)MsgId.SChangeHp, PacketHandler.S_ChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<S_Die>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.S_DieHandler);		
		_onRecv.Add((ushort)MsgId.SEnterWaitingRoom, MakePacket<S_EnterWaitingRoom>);
		_handler.Add((ushort)MsgId.SEnterWaitingRoom, PacketHandler.S_EnterWaitingRoomHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveWaitingRoom, MakePacket<S_LeaveWaitingRoom>);
		_handler.Add((ushort)MsgId.SLeaveWaitingRoom, PacketHandler.S_LeaveWaitingRoomHandler);		
		_onRecv.Add((ushort)MsgId.SStartGame, MakePacket<S_StartGame>);
		_handler.Add((ushort)MsgId.SStartGame, PacketHandler.S_StartGameHandler);		
		_onRecv.Add((ushort)MsgId.SWinGame, MakePacket<S_WinGame>);
		_handler.Add((ushort)MsgId.SWinGame, PacketHandler.S_WinGameHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
		
		if(CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{

		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			action.Invoke(session, pkt);

		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}