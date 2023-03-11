using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUI : UI_Base
{
    enum Buttons
    {
        NewGame,
    }

    enum Texts
    {
        ServerIPText,
    }

    public Button _newGame;

    [SerializeField]
    TMP_InputField _serverIPText;



    



    //���� ��ư�� ��� �������ٱ�?

    // Start is called before the first frame update
    void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(Texts));

        _newGame = Get<Button>((int)Buttons.NewGame);


        _serverIPText = Get<TMP_InputField>((int)Texts.ServerIPText);

        AddUIEvent(_newGame.gameObject, NewGame, Define.UIEvent.Click);


        _serverIPText.placeholder.GetComponent<TextMeshProUGUI>().text = "172.30.1.10";
        _serverIPText.text = "172.30.1.12";

    }

    public void NewGame(PointerEventData data)
    {
        Debug.Log("NewGame!");

        //SceneManager.LoadScene("Game");
        //SceneManager.LoadScene("WaitingRoom");
        //Managers.SceneManager.LoadScene(Define.Scene.WaitingRoom);
        //LoadScene�� �Բ� Managers�� NetWork�� Init �Ǹ鼭 ������ �����ϰ� ��.
        Managers.NetWork.IpAddress = _serverIPText.text;
        Managers.NetWork.Connect();
  
        //SeverSession OnConnected���� ������ LoadScene �����.


    }

    public void LoadWaitingRoom()
    {
        Managers.SceneManager.LoadScene(Define.Scene.WaitingRoom);

    }





}
