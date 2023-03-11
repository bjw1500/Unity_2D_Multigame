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


    //플레이어 입장.
    //슬롯 변경
    //만약 플레이어가 나가면, 슬롯 비우고 플레이어 위치 변경?
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
            //NULL이면 플레이어 슬롯이 비어있다는 소리니 스킵.
            if (_player[i].text != "NULL")
                continue;
            else
            {
                //슬롯이 비어있다면 한칸씩 당겨준다.

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

        //플레이어 이름 찾아서 제거하기?

        for(int i= 0; i< playerCount; i++)
        {
            //순회하고, 이름 비교
            if (_player[i].text == leaveGamePacket.Player.Name)
            {
                //슬롯에 채워진 이름이 방을 나가는 플레이어의 이름과 같다면,

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

        //Start 패킷을 보낸다.
        //슬롯값을 같이 딸려보낸다.

    }

    
    public void LoadScene()
    {
        Managers.SceneManager.LoadScene(Define.Scene.Game);

    }

}
