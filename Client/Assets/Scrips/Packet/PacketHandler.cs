using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PacketHandler
{

	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;


		//마스터 플레이어 생성?
		if (Managers.Object.slotNumber > Managers.Map.CastlePoint.Count)
		{
			Debug.Log("현재 플레이어수의가 맵의 인원 수보다 많습니다.");
			return;
		}

		Managers.Object.AddMasterPlayer(enterGamePacket.Player);
		//Managers.Object.MyPlayerInfo.Name = enterGamePacket.Player.Name;
		//Managers.Object.MyPlayerInfo.PlayerId = enterGamePacket.Player.PlayerId;
		//Managers.Object.MyPlayerInfo.TeamId = enterGamePacket.Player.TeamId;
		
        Debug.Log(enterGamePacket.Player);

	}

	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
		Managers.Object.RemoveMyPlayer();

		Debug.Log("S_LeaveGameHandler");
	}

	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;

		//스폰 정보를 어떻게 전달할 것인가?

		foreach(ObjectInfo spawnUnit in spawnPacket.Info)
        {
			Managers.Object.Add(spawnUnit);
		}


		Debug.Log("S_SpawnHandler");
	}

	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;

		foreach (ObjectInfo despawnUnit in despawnPacket.Info)
		{
			Debug.Log("despawn Id : " + despawnUnit.UnitId);
			Managers.Object.Remove(despawnUnit);
		}

		Debug.Log("S_DespawnHandler");

	}

	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;

		GameObject go  = Managers.Object.FindById(movePacket.UnitId);
		if (go == null)
			return;
		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc == null)
			return;
		cc.ObjectInfo.PosInfo = movePacket.PosInfo;
		//cc.ObjectInfo.PosInfo.PosX = movePacket.PosInfo.PosX;
		//cc.ObjectInfo.PosInfo.PosY = movePacket.PosInfo.PosY;
		//cc.ObjectInfo.PosInfo.DestinationX = movePacket.PosInfo.DestinationX;
		//cc.ObjectInfo.PosInfo.DestinationY = movePacket.PosInfo.DestinationY;
		//cc.State = movePacket.PosInfo.State;
		//cc.Dir = movePacket.PosInfo.MoveDir;


		//TODO 유닛을 이동시켜보자.


	}

	public static void S_SkillHandler(PacketSession session, IMessage packet)
	{
		S_Skill skillPacket = packet as S_Skill;
		ServerSession serverSession = session as ServerSession;

		//스킬 사용자
		GameObject go = Managers.Object.FindById(skillPacket.Info);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc == null)
			return;
		cc.ObjectInfo.PosInfo.State = CreatureState.Attack;

		//타겟 설정하기
		GameObject target = Managers.Object.FindById(skillPacket.Info.TargetId);
		cc._target = target;
		cc.UseSkill(skillPacket.SkilId);

	}

	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		S_ChangeHp changeHpPacket = packet as S_ChangeHp;
		ServerSession serverSession = session as ServerSession;

		//스킬 사용자
		GameObject go = Managers.Object.FindById(changeHpPacket.UnitId);
		if (go == null)
			return;

		BaseController cc = go.GetComponent<BaseController>();
		if (cc == null)
			return;

		cc.Hp = changeHpPacket.Hp;
		Debug.Log($"{changeHpPacket.UnitId}의 피가 {changeHpPacket.Hp}가 되었습니다.");


	}

	public static void S_DieHandler(PacketSession session, IMessage packet)
	{
		S_Die diePacket = packet as S_Die;
		ServerSession serverSession = session as ServerSession;

	
		GameObject go = Managers.Object.FindById(diePacket.UnitId);
		if (go == null)
			return;

		CreatureController cc = go.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.Hp = 0;
		cc.OnDead(diePacket.Attacker);

	}

	public static void S_WinGameHandler(PacketSession session, IMessage packet)
	{
		S_WinGame winPacket = packet as S_WinGame;
		ServerSession serverSession = session as ServerSession;


		if(winPacket.Win == 0)
        {
			//패배
			GameObject go = Managers.Resource.Instantiate("UI/Defeat");

        }else if(winPacket.Win == 1)
        {
			//승리
			GameObject go = Managers.Resource.Instantiate("UI/Win");
		}



	}

	public static void S_EnterWaitingRoomHandler(PacketSession session, IMessage packet)
	{
		S_EnterWaitingRoom enterGamePacket = packet as S_EnterWaitingRoom;

        //대기방에 입장시 서버에서 패킷이 날아온다.

        //날아온 패킷을 WaitingRoomUI에 전달해줘야 하는데...


        if (enterGamePacket.Player == null)
        {
            GameObject startUI = GameObject.Find("StartUI");
            StartUI su = startUI.GetComponent<StartUI>();
            su.LoadWaitingRoom();
            return;
        }

        //if (Managers.Object.MasterPlayer.WaitingRoom == null)
        //	return;

        GameObject go = GameObject.Find("WaitingRoom");
		if (go == null)
			return;

		WaitingRoomUI ui = go.GetComponent<WaitingRoomUI>();
		if (ui == null)
			return;

		ui.UpdateRoom(enterGamePacket);
	
	

		


		Debug.Log(enterGamePacket.Player);

	}

	public static void S_LeaveWaitingRoomHandler(PacketSession session, IMessage packet)
	{
		S_LeaveWaitingRoom leaveGamePacket = packet as S_LeaveWaitingRoom;

		//대기방에 입장시 서버에서 패킷이 날아온다.

		//날아온 패킷을 WaitingRoomUI에 전달해줘야 하는데...




		GameObject go = GameObject.Find("WaitingRoom");
		if (go == null)
			return;

		WaitingRoomUI ui = go.GetComponent<WaitingRoomUI>();

		if (ui == null)
			return;

		ui.LeaveGame(leaveGamePacket);


		Debug.Log(leaveGamePacket.Player);

	}

	public static void S_StartGameHandler(PacketSession session, IMessage packet)
	{
		S_StartGame startGamePacket = packet as S_StartGame;

		//대기방에 입장시 서버에서 패킷이 날아온다.

		//날아온 패킷을 WaitingRoomUI에 전달해줘야 하는데...

		GameObject go = GameObject.Find("WaitingRoom");

		if (go == null)
			return;

		WaitingRoomUI ui = go.GetComponent<WaitingRoomUI>();

		if (ui == null)
			return;

		ui.LoadScene();
		Managers.Object.slotNumber = startGamePacket.Slot;

		Debug.Log("게임을 시작합니다!");

	}




}
