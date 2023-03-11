using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Server
{
    public class GameRoom : JobSerializer
    {
        
        public int RoomId { get; set; }
        public int TeamdId { get; set; } = 1;
        public int UnitId { get; set; } = 1;

        public Dictionary<int, Player> _players = new Dictionary<int, Player>();
        //public Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
        public Player _monsterPlayer;
  
      

        public Map _map = new Map();

        public void Init(int mapId)
        {
           _map.LoadMap(mapId);

            ClientSession monsterSession = SessionManager.Instance.Generate();
           _monsterPlayer = PlayerManager.Instance.MonsterPlayerAdd();
           _monsterPlayer.Info.TeamId = 9;
           _monsterPlayer.Room = this;
           _monsterPlayer.Session = monsterSession;
            monsterSession.MyPlayer = _monsterPlayer;
            _players.Add(_monsterPlayer.Info.PlayerId, _monsterPlayer);

            if (_monsterPlayer != null)
                SpawnMineral();

        }

        public void Update()
        {

            //유닛 생성 및 몬스터 업데이트



            if (_monsterPlayer != null)
            {
                foreach (GameObject obj in _monsterPlayer.PlayerUnits.Values)
                {
                    Monster monster = obj as Monster;

                    if (monster == null)
                        continue;
                    monster.Update();
                }
            }

            Flush();
        }

        //TODo
        //중립 몬스터를 생성해보자.

        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;


                //플레이어 기본 인적사항 작성
                _players.Add(newPlayer.Info.PlayerId, newPlayer);
                newPlayer.Room = this;
                newPlayer.Info.Name = $"Player_{newPlayer.Info.PlayerId}";
                newPlayer.Info.TeamId = TeamdId++;

   
                //본인한테 정보 전송
                {
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);

                    S_Spawn spawnPacket = new S_Spawn();
                    //플레이어의 유닛 목록도 가지고 있어야 한다?

                    foreach (Player p in _players.Values)
                    {
                        foreach (GameObject p_info in p.PlayerUnits.Values)
                        { 
                            spawnPacket.Info.Add(p_info.Info);
                        }
                    }
                    newPlayer.Session.Send(spawnPacket);
                }

                //타인한테 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                   // spawnPacket.Info.Add(info);
                    foreach(Player p in _players.Values)
                    {
                        if (newPlayer != p && p !=_monsterPlayer)
                            p.Session.Send(spawnPacket);
                    }
                }

        }


        public void SpawnMonster()
        {

            if (_monsterPlayer == null)
                return;

            if (_players.Count < 2)
                return;

            S_Spawn S_Packet = new S_Spawn();
            ObjectInfo obj = new ObjectInfo()
            {
                Name = $"Orc3",
                TeamId = _monsterPlayer.Info.TeamId,
                WorldType = WorldObject.Npc,
                Player = _monsterPlayer.Info,
                PosInfo = new PositionInfo() {
                    MoveDir = MoveDir.Down,
                    State = CreatureState.Idle, 
                    PosX = _map.MonsterSpawnPoint[0].x,
                    PosY = _map.MonsterSpawnPoint[0].y},
            };

            obj.UnitId = obj.Player.PlayerId * 1000 + this.UnitId++;

            //유닛 Json 파일에서 스탯 추출.
            StatInfo stat = null;
            DataManager.StatDict.TryGetValue(obj.Name, out stat);
            obj.StatInfo = new StatInfo();
            obj.StatInfo.MergeFrom(stat);

            Monster newObject = new Monster(obj, _monsterPlayer, this);
            _monsterPlayer.PlayerUnits.Add(obj.UnitId, newObject);
         

            S_Packet.Info.Add(obj);
            BroadCast(S_Packet);
            Console.WriteLine("SpawnMonster!");
            ReadObj(obj);

        }

        public void SpawnMineral()
        {


            S_Spawn S_Packet = new S_Spawn();

            for (int i = 0; i < _map.ResourcePoint.Count; i++)
            {

                ObjectInfo obj = new ObjectInfo()
                {
                    Name = $"Mineral",
                    TeamId = _monsterPlayer.Info.TeamId,
                    WorldType = WorldObject.Npc,
                    Player = _monsterPlayer.Info,
                    PosInfo = new PositionInfo()
                    {
                        MoveDir = MoveDir.Down,
                        State = CreatureState.Idle,
                        PosX = _map.ResourcePoint[i].x,
                        PosY = _map.ResourcePoint[i].y
                    },
                };
                obj.UnitId = obj.Player.PlayerId * 1000 + this.UnitId++;

                //유닛 Json 파일에서 스탯 추출.
                StatInfo stat = null;
                DataManager.StatDict.TryGetValue(obj.Name, out stat);
                obj.StatInfo = new StatInfo();
                obj.StatInfo.MergeFrom(stat);

                Resource newObject = new Resource(obj, _monsterPlayer, this);
                _monsterPlayer.PlayerUnits.Add(obj.UnitId, newObject);
                S_Packet.Info.Add(obj);

            }



            BroadCast(S_Packet);
            Console.WriteLine("SpawnResource!");
     

        }


        public void LeaveGame(int playerId)
        {
 
            Player player = null;
            if (_players.Remove(playerId, out player) == false)
                return;

            player.Room = null;

            //본인한테 정보 전송
            {
                S_LeaveGame leavePacket = new S_LeaveGame();
                player.Session.Send(leavePacket);

            }


            //타인한테 정보 전송
            {
                S_Despawn despawnPacket = new S_Despawn();
                foreach (GameObject p_info in player.PlayerUnits.Values)
                {
                    despawnPacket.Info.Add(p_info.Info);
                }

                foreach(Player p in _players.Values)
                {
                    if (player != p && _monsterPlayer != p)
                        p.Session.Send(despawnPacket);
                }

            }
            
        }


        public void HandleSpawn(Player player, C_Spawn spawnPacket)
        {

            //서버 안의 유닛 목록 저장 & 보내기

            S_Spawn S_Packet = new S_Spawn();
            foreach (ObjectInfo obj in spawnPacket.Info)
            {
                obj.UnitId = obj.Player.PlayerId * 1000 + player.Room.UnitId++;

                //유닛 Json 파일에서 스탯 추출.
                StatInfo stat = null;
                DataManager.StatDict.TryGetValue(obj.Name, out stat);
                obj.StatInfo = new StatInfo();
                obj.StatInfo.MergeFrom(stat);
                obj.PosInfo = new PositionInfo()
                {
                    MoveDir = MoveDir.Down,
                    State = CreatureState.Idle,
                    //슬롯값으로 수정하기
                    PosX = _map.CastlePoint[player.Info.Slot].x,
                    PosY = _map.CastlePoint[player.Info.Slot].y
                };

                GameObject newObject = new GameObject(obj, player, this);
                player.PlayerUnits.Add(obj.UnitId, newObject);
              //  _monsters.Add(obj.UnitId, newObject);
            
                S_Packet.Info.Add(obj);
            }

            BroadCast(S_Packet);

        }

        public void HandleMove(Player player, C_Move movePacket)
        {

        if (player == null)
            return;

            //플레이어가 지닌 유닛 정보 서버에서 수정
            GameObject moveUnit = null;
            if (player.PlayerUnits.TryGetValue(movePacket.UnitId, out moveUnit) == false)
                return;


            //다른 좌표로 갈 수 있는지 판단.
            //TODO : 검증
            if (movePacket.PosInfo.PosX != moveUnit.PosInfo.PosX || movePacket.PosInfo.PosY != moveUnit.PosInfo.PosY)
            {
                //클라측에서 Idle이 되었을 때 자신의 위치 패킷을 또 한 번 보내게 된다.
                //그런데 이때 이미 CanGo 위치에 자신이 와 있으므로 return이 됨.
                //그래서 그 동안 계속 idle 값이 제대로 전달이 안됐던 것.
                //
                if (_map.CanGo(new Vector2Int(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY)) == false)
                {
                    return;
                }
            }
            _map.ApplyMove(moveUnit, new Vector2Int(movePacket.PosInfo.PosX, movePacket.PosInfo.PosY));


            //moveUnit.PosInfo.PosX = movePacket.PosInfo.PosX;
            //moveUnit.PosInfo.PosY = movePacket.PosInfo.PosY;
            //moveUnit.PosInfo.DestinationX = movePacket.PosInfo.DestinationX;
            //moveUnit.PosInfo.DestinationY = movePacket.PosInfo.DestinationY;
            //moveUnit.PosInfo.State = movePacket.PosInfo.State;
            //moveUnit.PosInfo.MoveDir = movePacket.PosInfo.MoveDir;
             moveUnit.PosInfo = movePacket.PosInfo;



            //클라이언트한테 패킷 전달
            S_Move S_MoveUnit = new S_Move();
            S_MoveUnit.PlayerId = player.Info.PlayerId;
            S_MoveUnit.UnitId = moveUnit.UnitId;
            S_MoveUnit.PosInfo = movePacket.PosInfo;


            BroadCast(S_MoveUnit);
            

        }

        public void HandleSkill(Player player, C_Skill skillPacket)
        {
        if (player == null)
            return;

   


            //TODO 스킬 사용 여부 체크


            //서버 안의 유닛 목록에서 상태 수정
            GameObject serverInfo = null;

            //혹시 스킬 사용 도중에 죽었다면 return.
            if (player.PlayerUnits.TryGetValue(skillPacket.Info.UnitId, out serverInfo) == false)
                return;

            serverInfo.PosInfo.MergeFrom(skillPacket.Info.PosInfo);
            serverInfo.PosInfo.State = CreatureState.Attack;
            serverInfo.Info.TargetId = skillPacket.Info.TargetId;

            //패킷 배포
            S_Skill skill = new S_Skill();
            skill.Info = serverInfo.Info;
            skill.SkilId = skillPacket.Skillid;
            BroadCast(skill);

            //Data.Skill SkillData = null;
            //if (DataManager.SkillDict.TryGetValue(info.SkillInfo.SkillId, out SkillData) == false) 
            //    return;

            //switch(SkillData.attackType)
            //{
            //    case AttackType.Normal:
            //        //TODO 데미지 판정
            //        //화살 움직임 계산이 너무 복잡해서 이런 데미지 판정 부분은 굳이 서버에 전담하지 않기로 함.
            //        //
            //        //Vector2Int skillPos = serverInfo.GetFrontCellPos();
            //        //GameObject target = _map.Find(skillPos);
            //        //if (target != null)
            //        //{
            //        //    target.OnDamaged(serverInfo, SkillData);
            //        //}
            //        break;

            //    case AttackType.Bow:
            //        Console.WriteLine("Bow Unit Attack !");
            //        break;

            //    case AttackType.Magic:

            //        break;

            //}

            
        }

        public void HandleHp(Player player, C_ChangeHp changeHpPacket)
        {

            if (player == null)
                return;


            Player attackedPlayer = null;
            _players.TryGetValue(changeHpPacket.PlayerId, out attackedPlayer);
            //공격 당한 유닛 찾기
            GameObject OnDamagedUnit = null;
            if (attackedPlayer.PlayerUnits.TryGetValue(changeHpPacket.UnitId, out OnDamagedUnit) == false)
                return;

            Data.Skill SkillData = null;
            if (DataManager.SkillDict.TryGetValue(changeHpPacket.AttackSkill.SkillId, out SkillData) == false)
                return;

            Player attackerPlayer = null;
            if (_players.TryGetValue(changeHpPacket.Attacker.Player.PlayerId, out attackerPlayer) == false)
                return;

            GameObject attackUnit = null;
            if (attackerPlayer.PlayerUnits.TryGetValue(changeHpPacket.Attacker.UnitId, out attackUnit) == false)
                return;

            OnDamagedUnit.OnDamaged(attackUnit, SkillData);
            
        }


        public GameObject FindObject(Func<GameObject, bool> condition)
        {

            foreach(Player player in _players.Values)
            {

                foreach (GameObject obj in player.PlayerUnits.Values)
                {

                    if (condition.Invoke(obj))
                        return obj;
                }
            }

            return null;
        }

        public void ReadObj(ObjectInfo obj)
        {
            Console.WriteLine("unitId :" + obj.UnitId + " TeamId :" + obj.TeamId);


        }


        public void BroadCast(IMessage Packet)
        {

 
            foreach (Player p in _players.Values)
            {
                if (p == _monsterPlayer)
                    continue;

                p.Session.Send(Packet);
            }
            

        }









    }
}
