using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public List<GameObject> _playerUnit = new List<GameObject>();
    public List<GameObject> _enemyUnit = new List<GameObject>();

    public Action<int> OnPlayerSpawnEvent;
    public Action<int> OnEnemySpawnEvent;


    public void Init()
    {
        
        //오브젝트를 찾는다.

        //스폰 후에 집어 넣어준다.

    }

    //public Define.WorldObject GetWorldObjectType(GameObject go)
    //{
    //    BaseController bc = go.GetComponent<BaseController>();

    //    if (bc == null)
    //        return Define.WorldObject.Unknown;

    //    return bc.WorldObjectType;
    //}


    public GameObject Spawn(WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);
        
        //switch (type)
        //{
        //    case Define.WorldObject.Player:
        //        go.GetOrAddComponent<PlayerController>();
        //        _playerUnit.Add(go);
        //        if (OnPlayerSpawnEvent != null)
        //        OnPlayerSpawnEvent(1);
        
        //    break;

        //    case Define.WorldObject.Enemy:
        //        //go.transform.Find("UnitRoot").gameObject.AddComponent<MonsterController>();
        //        //_enemyUnit.Add(go.transform.Find("UnitRoot").gameObject);
        //        go.AddComponent<MonsterController>();
        //        _enemyUnit.Add(go);
        //        if (OnEnemySpawnEvent != null)
        //       OnEnemySpawnEvent(1);
        //    break;
        //}

        return go;

    }

    //public void Despawn(GameObject go)
    //{
    //    Define.WorldObject type = GetWorldObjectType(go);

    //    switch (type)
    //    {
    //        case Define.WorldObject.Player:
    //            if (_playerUnit.Contains(go))
    //            {
    //               // _playerUnit.Remove(go);

    //            }
    //            break;
    //        case Define.WorldObject.Enemy:
    //            if (_enemyUnit.Contains(go))
    //            {
    //                _enemyUnit.Remove(go);

    //            }
    //            break;
    //    }

    //    Managers.ResourceManager.Destroy(go);
    //}








}
