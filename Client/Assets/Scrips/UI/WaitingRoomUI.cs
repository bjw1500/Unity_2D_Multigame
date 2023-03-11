using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PlayerSlot
{
    public int _slot;
    public PlayerInfo _playerInfo;


    //�÷��̾� ����.
    //���� ����
    //���� �÷��̾ ������, ���� ���� �÷��̾� ��ġ ����?
}

public class WaitingRoomUI : UI_Base
{
    public int playerCount = 2;
    public Dictionary<int, PlayerSlot> _players = new Dictionary<int, PlayerSlot>();
    public PlayerSlot mySlot;

    enum Texts
    {
        PlayerText1,
        PlayerText2,
        PlayerText3,
        PlayerText4,
    }

    enum Buttons
    {
        GameStart
    }

    List<TextMeshProUGUI> _player = new List<TextMeshProUGUI> ();
    TextMeshProUGUI _player1;
    TextMeshProUGUI _player2;
    TextMeshProUGUI _player3;
    TextMeshProUGUI _player4;

    Button _gameStart;


    void Start()
    {

      
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));



        for(int i = 0; i< playerCount; i++)
        {
            _player.Add(Get<TextMeshProUGUI>(i));
            _player[i].text = "NULL";
        }


        _gameStart = Get<Button>((int)Buttons.GameStart);

        AddUIEvent(_gameStart.gameObject, GameStart, Define.UIEvent.Click);


    }

    public void UpdateRoom(S_EnterWaitingRoom enterGamePacket)
    {

        PlayerSlot newPlayer = new PlayerSlot();
        newPlayer._playerInfo = enterGamePacket.Player;
        

        for(int i = 0; i< playerCount; i++)
        {
            //NULL�̸� �÷��̾� ������ ����ִٴ� �Ҹ��� ��ŵ.
            if (_player[i].text != "NULL")
                continue;
            else
            {
                //������ ����ִٸ� ��ĭ�� ����ش�.

                newPlayer._slot = i;
                _player[i].text = enterGamePacket.Player.Name;
                if (enterGamePacket.MyPlayer == true)
                    mySlot = newPlayer;

                break;
            }

        }

        _players.Add(enterGamePacket.Player.PlayerId, newPlayer);


    }

    public void LeaveGame(S_LeaveWaitingRoom leaveGamePacket)
    {

        _players.Remove(leaveGamePacket.Player.PlayerId);

        //�÷��̾� �̸� ã�Ƽ� �����ϱ�?

        for(int i= 0; i< playerCount; i++)
        {
            //��ȸ�ϰ�, �̸� ��
            if (_player[i].text == leaveGamePacket.Player.Name)
            {
                //���Կ� ä���� �̸��� ���� ������ �÷��̾��� �̸��� ���ٸ�,

                _player[i].text = "NULL";

            }

        }

    }



    public void GameStart(PointerEventData eventData)
    {
        // Managers.SceneManager.LoadScene(Define.Scene.Game);

        C_StartGame start = new C_StartGame();
        start.Slot = mySlot._slot;
        Managers.NetWork.Send(start);

        //Start ��Ŷ�� ������.
        //���԰��� ���� ����������.

    }

    
    public void LoadScene()
    {
        Managers.SceneManager.LoadScene(Define.Scene.Game);

    }

}
