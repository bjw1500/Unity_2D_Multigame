using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomScene : BaseScene
{
    public override void Clear()
    {
        
    }

    // Start is called before the first frame update
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.WaitingRoom;


    }

}
