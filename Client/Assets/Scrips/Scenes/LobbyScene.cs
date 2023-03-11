using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScene : BaseScene
{
    //���⼭ UI�� �ҷ�����. 
    //���� �ҷ������� ���� ������ ����� �Ѵ�.
    //


    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Lobby;

        Debug.Log("Lobby�Դϴ�.");

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
