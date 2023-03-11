using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScene : BaseScene
{
    //여기서 UI를 불러오자. 
    //씬을 불러오려면 빌드 세팅을 해줘야 한다.
    //


    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Lobby;

        Debug.Log("Lobby입니다.");

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Managers.SceneManager.LoadScene(Define.Scene.Game);

        }

    }


    void Start()
    {
        Init();
        
    }

    public override void Clear()
    {
        Debug.Log("Lobby Scene Clear!");

    }

}
