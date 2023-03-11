using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectManager : MonoBehaviour
{

    //플레이어 0 번은 자신의 유닛
    //플레이어 아디. 몬스터 아디. 이걸 어떻게 조합할까.

    //플레이어들의 유닛 목록 관리법.
    //플레이어 아디를 받고-> 유닛 Id를 확인해서 목록에서 삭제한다.
    //유닛 아디도 서버에서 배포해야지 않나?

    public string playerName;
    public MasterController MasterPlayer { get; set; }
    public int slotNumber; 

    static int maxMember = 10;

    Dictionary<int, GameObject> _worldUnit = new Dictionary<int, GameObject>();
    Dictionary<int, GameObject>[] _player = new Dictionary<int, GameObject>[maxMember];
    Dictionary<int, GameObject>[] _enemy = new Dictionary<int, GameObject>[maxMember];


    public void Init()
    {

    }

    public void Update()
    {


    }

    public void AddMasterPlayer(PlayerInfo info)
    {

        GameObject Master = GameObject.Find("@MasterPlayer");
        
        if(MasterPlayer == null)
        {
            Master = new GameObject();
            MasterController mc = Master.AddComponent<MasterController>();
            mc.Info = info;
            mc.Mineral = 0;
            mc.PlayerUnits = _player[info.PlayerId];
            MasterPlayer = mc;
            Master.name = "@MasterPlayer";
        }

        {

            //플레이어의 아이디가 캐슬 포인트를 넘어가면 오류 발생.
            //플레이어의 아이디값이 아닌 대기실 슬롯값으로 진행?
            ObjectInfo castle = new ObjectInfo()
            {
                Name = "Base Blue",
                TeamId = Managers.Object.MasterPlayer.Info.TeamId,
                WorldType = WorldObject.Player,
                Player = Managers.Object.MasterPlayer.Info,
                PosInfo = new PositionInfo()
                { PosX = Managers.Map.CastlePoint[slotNumber].x,
                  PosY = Managers.Map.CastlePoint[slotNumber].y
                },
            };

            Managers.Object.Spawn(castle);

        }

        //MasterPlayer.SpawnTest();
        MasterPlayer.StartGetMineral();


    }

    public void Add(ObjectInfo info)
    {

        if (info == null)
            return;

        GameObject obj = Managers.Resource.Instantiate($"Player/{info.Name}");
        if (obj == null)
        {
            Debug.Log("Obj is Null");
            return;
        }
        obj.name = info.Player.PlayerId +" "+ info.Name;
        //_myUnit = _player[MasMyPlayerInfo.PlayerId];
        //오브젝트 ID 관리
       
        Debug.Log($"Add Object : [{info.Player.PlayerId},{info.UnitId}]");

        if (info.TeamId == MasterPlayer.Info.TeamId)
        {

            //해당 플레이어 목록이 없을 경우 새로 생성
            if (_player[info.Player.PlayerId] == null)
                _player[info.Player.PlayerId] = new Dictionary<int, GameObject>();


            if (info.StatInfo.UnitType == UnitType.Building)
            {

                BuildingController cc = Util.GetOrAddComponent<BuildingController>(obj);
                cc.ObjectInfo = info;

                Vector3Int castlePoint = Managers.Map.CastlePoint[info.Player.Slot];

                _player[info.Player.PlayerId].Add(info.UnitId, obj);
                _worldUnit.Add(info.UnitId, obj);
            }
            else if(info.StatInfo.UnitType == UnitType.Resource)
            {
                ResourceController cc = Util.GetOrAddComponent<ResourceController>(obj);
                cc.ObjectInfo = info;

                _player[info.Player.PlayerId].Add(info.UnitId, obj);
                _worldUnit.Add(info.UnitId, obj);

            }
            else
            {
                CreatureController cc = null; 
                if(MasterPlayer.Info.PlayerId != info.Player.PlayerId) 
                cc = Util.GetOrAddComponent<PlayerController>(obj);
                else if (MasterPlayer.Info.PlayerId == info.Player.PlayerId)
                {
                    if(info.StatInfo.UnitType == UnitType.Worker)
                        cc = Util.GetOrAddComponent<WorkerController>(obj);
                    else if(info.StatInfo.UnitType == UnitType.Healer)
                        cc = Util.GetOrAddComponent<HealerController>(obj);
                    else
                        cc = Util.GetOrAddComponent<MyPlayerController>(obj);
                }

                cc.ObjectInfo = info;

                _player[info.Player.PlayerId].Add(info.UnitId, obj);
                _worldUnit.Add(info.UnitId, obj);
            }

        }
        else if (info.TeamId != MasterPlayer.Info.TeamId)
        {

            if (_enemy[info.Player.PlayerId] == null)
                _enemy[info.Player.PlayerId] = new Dictionary<int, GameObject>();


            //생성 타입이 캐슬, 건물이라면.
            if (info.StatInfo.UnitType == UnitType.Building)
            {
                BuildingController cc = Util.GetOrAddComponent<BuildingController>(obj);
                cc.ObjectInfo = info;

                Vector3Int castlePoint = Managers.Map.CastlePoint[info.Player.Slot];


                _enemy[info.Player.PlayerId].Add(info.UnitId, obj);
                _worldUnit.Add(info.UnitId, obj);

            }
            else if (info.StatInfo.UnitType == UnitType.Resource)
            {
                ResourceController cc = Util.GetOrAddComponent<ResourceController>(obj);
                cc.ObjectInfo = info;

                _enemy[info.Player.PlayerId].Add(info.UnitId, obj);
                _worldUnit.Add(info.UnitId, obj);

            }
            else
            {
                //enemy에 추가
                PlayerController cc = Util.GetOrAddComponent<PlayerController>(obj);

                cc.ObjectInfo = info;
                if (_enemy[info.Player.PlayerId].ContainsKey(info.UnitId))
                {
                    Destroy(cc.gameObject);
                    return;
                }

                _enemy[info.Player.PlayerId].Add(info.UnitId, obj);
                _worldUnit.Add(info.UnitId, obj);
            }


        }
     
    }


    public void Spawn(ObjectInfo info)
    {

        C_Spawn SpawnPacket = new C_Spawn();
        SpawnPacket.Info.Add(info);
        Managers.NetWork.Send(SpawnPacket);
    }
    public void Spawn(ObjectInfo[] info)
    {

        C_Spawn SpawnPacket = new C_Spawn();
        foreach(ObjectInfo obj in info)
        {
            SpawnPacket.Info.Add(obj);

        }
        Managers.NetWork.Send(SpawnPacket);
    }

    public void Remove(ObjectInfo type)
    {
        GameObject obj = FindById(type);

        if(type.TeamId == MasterPlayer.Info.TeamId)
        {
            _player[type.Player.PlayerId].Remove(type.UnitId);
            _worldUnit.Remove(type.UnitId);

        }
        else
        {
            _enemy[type.Player.PlayerId].Remove(type.UnitId);
            _worldUnit.Remove(type.UnitId);
        }

        Destroy(obj, 2.0f);
    }

    public void RemoveMyPlayer()
    {

        _player[MasterPlayer.Info.PlayerId].Clear();
        _worldUnit.Clear();
        
        foreach(GameObject obj in MasterPlayer.PlayerUnits.Values)
        {
            Managers.Resource.Destroy(obj);
        }

    }

    public GameObject FindById(ObjectInfo info)
    {
        GameObject obj = null;

        _worldUnit.TryGetValue(info.UnitId, out obj);

        return obj;
    }

    public GameObject FindById(int UnitId)
    {
        GameObject obj = null;

        _worldUnit.TryGetValue(UnitId, out obj);

        return obj;
    }



    public GameObject Find(Vector3Int cellPos)
    {

        #region 수정전
        //해당 지역에 유닛이 있는지 확인하는 함수
        //foreach(GameObject obj in _player.Values)
        //{
        //    CreatureController cc = obj.GetComponent<CreatureController>();
        //    if (cc == null)
        //        continue;

        //    if (cc.CellPos == cellPos)
        //        return obj;
        //}

        //foreach (GameObject obj in _enemy.Values)
        //{
        //    CreatureController cc = obj.GetComponent<CreatureController>();
        //    if (cc == null)
        //        continue;

        //    if (cc.CellPos == cellPos)
        //        return obj;
        //}
        #endregion

        foreach (GameObject obj in _worldUnit.Values)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (cc.CellPos == cellPos)
                return obj;
        }

        return null;
    }

    public GameObject Find(WorldObject type , GameObject start, Func<GameObject, bool> condition)
    {
        //해당 지역에 유닛이 있는지 확인하는 함수
        GameObject nearObj = null;
        float _nearDis = 99999.0f;

        #region 수정전
        //if (type == WorldObject.Player)
        //{
        //    foreach (GameObject obj in _enemy.Values)
        //    {
        //        if (condition.Invoke(obj))
        //        {
        //            float dis = (obj.transform.position - start.transform.position).sqrMagnitude;

        //            if (_nearDis > dis)
        //            {
        //                _nearDis = dis;
        //                nearObj = obj;
        //            }
        //           // return obj;
        //        }
        //    }
        //}else if(type == WorldObject.Enemy)
        //{
        //    foreach (GameObject obj in _player.Values)
        //    {
        //        if (condition.Invoke(obj))
        //        {
        //            float dis = (obj.transform.position - start.transform.position).sqrMagnitude;

        //            if (_nearDis > dis)
        //            {
        //                _nearDis = dis;
        //                nearObj = obj;
        //            }
        //            // return obj;
        //        }
        //    }

        //}
        #endregion

        foreach (GameObject obj in _worldUnit.Values)
        {
            if (condition.Invoke(obj))
            {
                float dis = (obj.transform.position - start.transform.position).sqrMagnitude;

                if (_nearDis > dis)
                {
                    _nearDis = dis;
                    nearObj = obj;
                }      
            }
        }



            return nearObj;
    }

   

    public void Clear()
    {

        for (int i = 0; i < maxMember; i++)
        {
            _player[i].Clear();
            _worldUnit.Clear();
            _enemy[i].Clear();
        }

    }




}
