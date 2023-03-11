using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour
{

    public PlayerInfo Info { get; set; }

    public GameObject WaitingRoom { get; set; } = null;


    [SerializeField]
    public int Mineral { get; set; } = 0;
    //플레이어의 유닛 목록
    public Dictionary<int, GameObject> PlayerUnits { get; set; } = new Dictionary<int, GameObject>();



    Coroutine _coSpawnTest = null;
    Coroutine _coStartGetMeneral = null;
    //자원

    public void StartGetMineral()
    {

        _coStartGetMeneral = StartCoroutine("CoStartGetMineral", 3.0f);

    }

    public void SpawnTest()
    {

        _coSpawnTest = StartCoroutine("CoStartSpawn", 3.0f);

    }

    IEnumerator CoStartGetMineral(float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Mineral += 1;

        }

    }

    IEnumerator CoStartSpawn(float time)
    {
        while (true)
        {
            Debug.Log("Spawn Test");
            ObjectInfo knight = new ObjectInfo()
            {
                Name = $"Knight",
                TeamId = Managers.Object.MasterPlayer.Info.TeamId,
                WorldType = WorldObject.Player,
                Player = Managers.Object.MasterPlayer.Info,
                PosInfo = new PositionInfo(),
            };

            Managers.Object.Spawn(knight);
            yield return new WaitForSeconds(time);
        }
    }



}
