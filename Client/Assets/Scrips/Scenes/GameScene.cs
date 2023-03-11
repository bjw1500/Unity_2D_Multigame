using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;
        //Screen.SetResolution(1920, 1080, false);
        Managers.Map.LoadMap(1);

    }



    public override void Clear()
    {
        

    }


}
