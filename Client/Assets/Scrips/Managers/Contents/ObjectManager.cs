using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectManager : MonoBehaviour
{

    //�÷��̾� 0 ���� �ڽ��� ����
    //�÷��̾� �Ƶ�. ���� �Ƶ�. �̰� ��� �����ұ�.

    //�÷��̾���� ���� ��� ������.
    //�÷��̾� �Ƶ� �ް�-> ���� Id�� Ȯ���ؼ� ��Ͽ��� �����Ѵ�.
    //���� �Ƶ� �������� �����ؾ��� �ʳ�?

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

            //�÷��̾��� ���̵� ĳ�� ����Ʈ�� �Ѿ�� ���� �߻�.
            //�÷��̾��� ���̵��� �ƴ� ���� ���԰����� ����?
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
        //������Ʈ ID ����
       
        Debug.Log($"Add Object : [{info.Player.PlayerId},{info.UnitId}]");

        if (info.TeamId == MasterPlayer.Info.TeamId)
        {

            //�ش� �÷��̾� ����� ���� ��� ���� ����
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


            //���� Ÿ���� ĳ��, �ǹ��̶��.
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
                //enemy�� �߰�
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

        #region ������
        //�ش� ������ ������ �ִ��� Ȯ���ϴ� �Լ�
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
        //�ش� ������ ������ �ִ��� Ȯ���ϴ� �Լ�
        GameObject nearObj = null;
        float _nearDis = 99999.0f;

        #region ������
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
