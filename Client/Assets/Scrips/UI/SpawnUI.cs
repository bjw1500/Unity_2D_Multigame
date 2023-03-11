using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpawnUI : UI_Base
{
    // Start is called before the first frame update

  

    [SerializeField]
    TextMeshProUGUI _mineralText;

    Button _archerSpawnButton = null;
    Button _knightSpawnButton = null;
    Button _workerSpawnButton = null;
    Button _royalKnightSpawnButton = null;
    Button _healerSpawnButton = null;



    enum Units{
        Knight,
        Archer,
    }

    enum Buttons
    {
        ArcherSpawnButton,
        KnightSpawnButton,
        WorkerSpawnButton,
        RoyalKnightSpawnButton,
        HealerSpawnButton,
    }

    enum Texts
    {
        MineralText,
    }


    public void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<TextMeshProUGUI>(typeof(Texts));

        _archerSpawnButton = Get<Button>((int)Buttons.ArcherSpawnButton);
        _knightSpawnButton = Get<Button>((int)Buttons.KnightSpawnButton);
        _workerSpawnButton = Get<Button>((int)Buttons.WorkerSpawnButton);
        _royalKnightSpawnButton = Get<Button>((int)Buttons.RoyalKnightSpawnButton);
        _healerSpawnButton = Get<Button>((int)Buttons.HealerSpawnButton);



        UnityEngine.Object[] button = null;
        _objects.TryGetValue(typeof(Button), out button);
        foreach(UnityEngine.Object obj in button )
        {
            Button bt = obj as Button; 
            bt.gameObject.AddComponent<UI_EventHandler>();
        }

        //�Լ��� ������ ���� ���ĺ���.

        AddUIEvent(_knightSpawnButton.gameObject, KnightSpawnEvent, Define.UIEvent.Click);
        AddUIEvent(_archerSpawnButton.gameObject, ArcherSpawnEvent, Define.UIEvent.Click);
        AddUIEvent(_workerSpawnButton.gameObject, WorkerSpawnEvent, Define.UIEvent.Click);
        AddUIEvent(_royalKnightSpawnButton.gameObject, RoyalKnightSpawnEvent, Define.UIEvent.Click);
        AddUIEvent(_healerSpawnButton.gameObject, HealerSpawnEvent, Define.UIEvent.Click);


        _mineralText = Get<TextMeshProUGUI>((int)Texts.MineralText);

    }


    public void Update()
    {
        if (Managers.Object.MasterPlayer == null)
            return;

        _mineralText.text = $"Mineral:{Managers.Object.MasterPlayer.Mineral}";
    }

    //������ư ���踦 ��� �ұ�?
    //���� ������ �޴� �����̺�Ʈ�� �������̽��� ����?
    //���踦 �Ϸ��� ���� �ȿ� ���� ������ ���� UI�� �����ؾ���.
    //


    public void KnightSpawnEvent(PointerEventData data)
    {
        StatInfo spawnUnit = null;
        Managers.DataManager.StatDict.TryGetValue("Knight", out spawnUnit);
        if (spawnUnit == null)
        {

            Debug.Log("�߸��� �����Դϴ�.");
            return;

        }

        if (Managers.Object.MasterPlayer.Mineral < spawnUnit.Cost)
        {
            Debug.Log("�̳׶��� �����մϴ�.");
            return;
        }

        //�÷��̾��� �ڿ����� ���߿� �������� �����ϰ� ���ٰ�.

        Managers.Object.MasterPlayer.Mineral = Managers.Object.MasterPlayer.Mineral - spawnUnit.Cost;


        Debug.Log("Click Spawn");
        ObjectInfo knight = new ObjectInfo()
        {
            Name = $"Knight",
            TeamId = Managers.Object.MasterPlayer.Info.TeamId,
            WorldType = WorldObject.Player,
            Player = Managers.Object.MasterPlayer.Info,
            PosInfo = new PositionInfo(),
        };

        Managers.Object.Spawn(knight);

    }


    public void ArcherSpawnEvent(PointerEventData data)
    {
        StatInfo spawnUnit = null;
        Managers.DataManager.StatDict.TryGetValue("Archer", out spawnUnit);
        if (spawnUnit == null)
        {

            Debug.Log("�߸��� �����Դϴ�.");
            return;

        }

        if (Managers.Object.MasterPlayer.Mineral < spawnUnit.Cost)
        {
            Debug.Log("�̳׶��� �����մϴ�.");
            return;
        }

        //�÷��̾��� �ڿ����� ���߿� �������� �����ϰ� ���ٰ�.

        Managers.Object.MasterPlayer.Mineral = Managers.Object.MasterPlayer.Mineral - spawnUnit.Cost;



        Debug.Log("Click �ü� Spawn");
        ObjectInfo archer = new ObjectInfo()
        {
            Name = $"Archer",
            TeamId = Managers.Object.MasterPlayer.Info.TeamId,
            WorldType = WorldObject.Player,
            Player = Managers.Object.MasterPlayer.Info,
            PosInfo = new PositionInfo(),
        };

        Managers.Object.Spawn(archer);

    }

    public void WorkerSpawnEvent(PointerEventData data)
    {
        StatInfo spawnUnit = null;
        Managers.DataManager.StatDict.TryGetValue("Worker", out spawnUnit);
        if (spawnUnit == null)
        {

            Debug.Log("�߸��� �����Դϴ�.");
            return;

        }

        if (Managers.Object.MasterPlayer.Mineral < spawnUnit.Cost)
        {
            Debug.Log("�̳׶��� �����մϴ�.");
            return;
        }

        //�÷��̾��� �ڿ����� ���߿� �������� �����ϰ� ���ٰ�.

        Managers.Object.MasterPlayer.Mineral = Managers.Object.MasterPlayer.Mineral - spawnUnit.Cost;


        Debug.Log("Click Spawn");
        ObjectInfo knight = new ObjectInfo()
        {
            Name = $"Worker",
            TeamId = Managers.Object.MasterPlayer.Info.TeamId,
            WorldType = WorldObject.Player,
            Player = Managers.Object.MasterPlayer.Info,
            PosInfo = new PositionInfo(),
        };

        Managers.Object.Spawn(knight);

    }

    public void RoyalKnightSpawnEvent(PointerEventData data)
    {

        StatInfo spawnUnit = null;
        Managers.DataManager.StatDict.TryGetValue("RoyalKnight", out spawnUnit);
        if (spawnUnit == null)
        {

            Debug.Log("�߸��� �����Դϴ�.");
            return;

        }

        if (Managers.Object.MasterPlayer.Mineral < spawnUnit.Cost)
        {
            Debug.Log("�̳׶��� �����մϴ�.");
            return;
        }

        //�÷��̾��� �ڿ����� ���߿� �������� �����ϰ� ���ٰ�.

        Managers.Object.MasterPlayer.Mineral = Managers.Object.MasterPlayer.Mineral - spawnUnit.Cost;


        Debug.Log("Click Spawn");
        ObjectInfo knight = new ObjectInfo()
        {
            Name = $"RoyalKnight",
            TeamId = Managers.Object.MasterPlayer.Info.TeamId,
            WorldType = WorldObject.Player,
            Player = Managers.Object.MasterPlayer.Info,
            PosInfo = new PositionInfo(),
        };

        Managers.Object.Spawn(knight);

    }

    public void HealerSpawnEvent(PointerEventData data)
    {

        StatInfo spawnUnit = null;
        Managers.DataManager.StatDict.TryGetValue("Healer", out spawnUnit);
        if (spawnUnit == null)
        {

            Debug.Log("�߸��� �����Դϴ�.");
            return;

        }

        if (Managers.Object.MasterPlayer.Mineral < spawnUnit.Cost)
        {
            Debug.Log("�̳׶��� �����մϴ�.");
            return;
        }

        //�÷��̾��� �ڿ����� ���߿� �������� �����ϰ� ���ٰ�.

        Managers.Object.MasterPlayer.Mineral = Managers.Object.MasterPlayer.Mineral - spawnUnit.Cost;


        Debug.Log("Click Spawn");
        ObjectInfo knight = new ObjectInfo()
        {
            Name = $"Healer",
            TeamId = Managers.Object.MasterPlayer.Info.TeamId,
            WorldType = WorldObject.Player,
            Player = Managers.Object.MasterPlayer.Info,
            PosInfo = new PositionInfo(),
        };

        Managers.Object.Spawn(knight);

    }

}
