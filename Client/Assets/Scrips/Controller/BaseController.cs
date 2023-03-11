using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class BaseController : MonoBehaviour
{

    public int Id { 
        get { return ObjectInfo.UnitId; }
        set { ObjectInfo.UnitId = value; } }

    public ObjectInfo ObjectInfo { get; set; }
    public WorldObject WorldType { 
        get { return ObjectInfo.WorldType; } 
        set { ObjectInfo.WorldType = value; } 
    }

    public AttackType AttackType { 
        get { return ObjectInfo.StatInfo.AttackType; }
        set { ObjectInfo.StatInfo.AttackType = value; }
    }

    public StatInfo StatInfo
    {
        get { return ObjectInfo.StatInfo; }
        set {
            ObjectInfo.StatInfo.Hp = value.Hp;
            ObjectInfo.StatInfo.MaxHp = value.MaxHp;
            ObjectInfo.StatInfo.Name = value.Name;
            ObjectInfo.StatInfo.Speed = value.Speed;
            ObjectInfo.StatInfo.Armor = value.Armor;
            ObjectInfo.StatInfo.Damage = value.Damage;

            UpdateHpBar();
        }
    }

    [SerializeField]
    public int Hp { get { return StatInfo.Hp; }
        set { StatInfo.Hp = value;
            UpdateHpBar();
        }
    }


    //TODO Speed 정보도 나중에 따로 빼주자.
    public float Speed
    {
        get { return StatInfo.Speed; }
        set { StatInfo.Speed = value; }
    }

    [SerializeField]
    public GameObject _target = null;

    [SerializeField]
    public int Teamdid { get { return ObjectInfo.TeamId; } }


    HpBar _hpBar;

    protected Coroutine _coSkill;
    protected Coroutine _coSearch;
    protected Coroutine _coAttack;
    protected Coroutine _coCoolTime;
    protected Coroutine _coTargetTime;

    public Vector3Int Destination
    {
        get { return new Vector3Int(ObjectInfo.PosInfo.DestinationX, ObjectInfo.PosInfo.DestinationY, 0); }
        set { ObjectInfo.PosInfo.DestinationX= value.x;
            ObjectInfo.PosInfo.DestinationY = value.y;
        }
    }

    public Vector3Int CellPos {
        get { return new Vector3Int(ObjectInfo.PosInfo.PosX, ObjectInfo.PosInfo.PosY, 0); }
        set {

            ObjectInfo.PosInfo.PosX = value.x;
            ObjectInfo.PosInfo.PosY = value.y;
            UpdateAnimation();
        } }

    virtual public MoveDir Dir
    {
        get
        {
            return ObjectInfo.PosInfo.MoveDir;
        }

        set
        {

            if (ObjectInfo.PosInfo.MoveDir == value)
                return;

            ObjectInfo.PosInfo.MoveDir = value;

           RotationToDir();
           UpdateAnimation();
        }

    }

    [SerializeField]
    virtual public CreatureState State
    {
        get
        {
            return ObjectInfo.PosInfo.State;
        }

        set
        {
            if (ObjectInfo.PosInfo.State == value)
                return;

            if (ObjectInfo.PosInfo.State == CreatureState.Dead)
                return;

            ObjectInfo.PosInfo.State = value;

            if(_coSearch != null)
            {
                StopCoroutine(_coSearch);
                _coSearch = null;
            }

            UpdateAnimation();
        }
    }



    void Start()
    {
        Init();

    }

    protected virtual void Init()
    {

    }


    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 1.0f, 0);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();

    }

    void UpdateHpBar()
    {
        if (_hpBar == null)
            return;

        float ratio = 0.0f;
        if (StatInfo.MaxHp > 0)
        {
            ratio = ((float)Hp / StatInfo.MaxHp);

        }

        _hpBar.SetHpBar(ratio);

    }

    // Update is called once per frame


    protected virtual void Update()
    {
        UpdateController();

    }


    protected virtual void UpdateController()
    {


    }

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else if (dir.y < 0)
            return MoveDir.Down;
        else
            return MoveDir.None;
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    public void RotationToDir()
    {
        if (ObjectInfo.PosInfo.MoveDir == MoveDir.Right)
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        else if (ObjectInfo.PosInfo.MoveDir == MoveDir.Left)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }
    protected virtual void MoveToNextPos()
    {

    }

    protected virtual void UpdateAnimation()
    {

    }

    protected virtual void UpdateMoving()
    {


   

    }

    protected virtual void UpdateIdle()
    {




    }

    protected virtual void UpdateAttack()
    {




    }

    protected virtual void UpdateDead()
    {




    }
    public virtual void OnDamaged(ObjectInfo attacker, SkillInfo attackSkill)
    {

    }

    public virtual void OnDead(ObjectInfo attacker)
    {

    }

}
