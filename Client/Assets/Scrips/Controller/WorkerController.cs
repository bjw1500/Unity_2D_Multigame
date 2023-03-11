using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorkerController : CreatureController
{
    Vector3 targetPoint = Vector3.zero;
    bool canAttack = true;
    bool canMining = true;
    int miningCount = 0;

    override public CreatureState State
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

            //if (_coSearch != null)
            //{
            //    StopCoroutine(_coSearch);
            //    _coSearch = null;
            //}

            CheckUpdatePosition();

        }
    }

    protected override void Init()
    {
        base.Init();

    }

    // Update is called once per frame

    private void LateUpdate()
    {
        //Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -20);
    }

    protected override void UpdateController()
    {

        base.UpdateController();

    }


    protected override void UpdateMoving()
    {
        if (State == CreatureState.Dead)
            return;

        Vector3 dest = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = dest - transform.position;

        //여기다가 또 안해주면 다른 플레이어의 방향 전환이 안된다.
        RotationToDir();

        if (moveDir.magnitude < Speed * Time.deltaTime)
        {
            //목표 지점에 도착
            transform.position = dest;
            Debug.Log(ObjectInfo.UnitId + " " + transform.position.x + " " + transform.position.y);
            MoveToNextPos();
            return;
        }
        else
        {
            transform.position += moveDir.normalized * Speed * Time.deltaTime;
        }
    }

    protected override void MoveToNextPos()
    {

        Vector3Int destPos = Destination;
        if (_target != null && canMining)
        {

            if (_coTargetTime == null)
            {
                Debug.Log(ObjectInfo.Name + "_coTargetTime Start");
                _coTargetTime = StartCoroutine("CoTargetTime", 3.0f);
            }

            if (_coSearch != null)
            {
                StopCoroutine(_coSearch);
                _coSearch = null;
            }

            Attack();
      

        }

        List<Vector3Int> path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        if (path.Count < 2 || (_target != null && path.Count > 40))
        {
            _target = null;
            State = CreatureState.Idle;
            canMining = true;
            CheckUpdatePosition();
            return;
        }

        Vector3Int nextPos = path[1];
        Vector3Int moveCellDir = nextPos - CellPos;
        Dir = GetDirFromVec(moveCellDir);

        if (Managers.Map.CanGo(nextPos) && Managers.Object.Find(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else
        {
            if (nextPos == Managers.Map.CastlePoint[ObjectInfo.Player.Slot])
            {
                //자원을 가지고 기지로 돌아온 상태.
                canMining = true;
                Managers.Object.MasterPlayer.Mineral += 5;

            }

            State = CreatureState.Idle;
        }

        CheckUpdatePosition();
    }


    protected override void UpdateIdle()
    {
        if (_target == null && _coSearch == null)
        {
            _coSearch = StartCoroutine("CoSearch");

        }
        else if (_target != null && State != CreatureState.Dead && canMining == true)
        {

            Attack();
            //State = CreatureState.Moving;
        }
  


        //타깃이 존재하고, 공격 중인 상태가 아닐때.

    }

    protected override void UpdateAttack()
    {

        if (_target != null)
        {
            if (_coSearch != null)
                StopCoroutine(_coSearch);
            _coSearch = null;

        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public void Attack()
    {

        CreatureController cc = _target.GetComponent<CreatureController>();
        if (cc.State == CreatureState.Dead)
        {
            _target = null;
            //StopCoroutine(_coSearch);
            //_coSearch = null;
            State = CreatureState.Idle;
            CheckUpdatePosition();
            return;
        }

        Destination = cc.CellPos;
        Vector3Int dir = Destination - CellPos;
        Dir = GetDirFromVec(dir);
        if (dir.magnitude <= StatInfo.SkillRange)
        {

            //타겟을 공격할 수 없다고 판단되면 새로운 대상을 찾음.
            if(_coSearch != null)
            StopCoroutine(_coSearch);

            StopCoroutine("CoTargetTime");
            _coTargetTime = null;

            if (_coSkill == null)
            {
                canAttack = false;
                CheckUpdatePosition();
                CheckUpdateAttack((int)ObjectInfo.StatInfo.AttackType);
            }
            return;
        }
        else
        {

            State = CreatureState.Moving;

        }
    }


    public override void OnDead(ObjectInfo attacker)
    {
        base.OnDead(attacker);


        //죽었으니 디스폰하기.

        C_Despawn DespawnPacket = new C_Despawn();
        DespawnPacket.Info.Add(this.ObjectInfo);
        Managers.NetWork.Send(DespawnPacket);


    }

    public override void UseSkill(int skillId)
    {
        RotationToDir();

        if (canMining == false)
            return;

        Debug.Log($"{miningCount}");


        if (miningCount > 3)
        {
            miningCount = 0;
            canMining = false;
            _target = null;

 
            //자원을 가지고 기지로 돌아간다.

            Destination = Managers.Map.CastlePoint[ObjectInfo.Player.Slot];
            State = CreatureState.Moving;
            return;

            //자원을 가지고 기지에 도착하면?
            //어차피 알아서  idle이 되어 자원을 다시 탐색할 것.
        }


        if (_coSkill != null)
            return;

        switch (skillId)
        {
            case 0:
                _coSkill = StartCoroutine("CoStartNormalAttack", StatInfo.AttackSpeed);
                break;
            case 1:
                _coSkill = StartCoroutine("CoStartBowAttack", StatInfo.AttackSpeed);
                break;
            case 2:
                _coSkill = StartCoroutine("CoStartMagicAttack", StatInfo.AttackSpeed);
                break;
            default:
                break;
        }

        return;
    }

    IEnumerator CoStartNormalAttack(float time)
    {

        CreatureController cc = _target.GetComponent<CreatureController>();

        if (cc != null)
        {
            cc.OnDamaged(this.ObjectInfo, new SkillInfo() { SkillId = 0 });
        }

        yield return new WaitForSeconds(time);
        miningCount++;
        State = CreatureState.Idle;
        _coSkill = null;
        //canAttack = true;
    }

    IEnumerator CoStartBowAttack(float time)
    {

        //원거리 투사체 생성
        GameObject go = Managers.Resource.Instantiate("Weapon/Arrow");
        ArrowController cc = go.GetComponent<ArrowController>();


        cc.transform.position = transform.position;
        cc._target = _target;
        cc._master = this.ObjectInfo;
        cc.skillinfo = new SkillInfo() { SkillId = 1 };

        //원거리 투사체는 어떻게 이동시킬건가?
        yield return new WaitForSeconds(time);
        if (canMining == true)
            State = CreatureState.Idle;
        else
            State = CreatureState.Moving;

        _coSkill = null;
        // canAttack = true;
    }

    IEnumerator CoStartMagicAttack(float time)
    {
        GameObject go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
            {
                cc.OnDamaged(this.ObjectInfo, new SkillInfo() { SkillId = 2 });
            }
        }

        yield return new WaitForSeconds(time);
        //Idle로 바꿔줘야지 공격 대상이 죽었을 때, 다른 대상을 찾음.
        State = CreatureState.Idle;
        _coSkill = null;
    }

    IEnumerator CoSearch()
    {
    

        while (true)
        {

            yield return new WaitForSeconds(0.5f);

            if (_target != null)
            {
                State = CreatureState.Moving;
                continue;
            }

            _target = Managers.Object.Find(WorldType, gameObject, (go) =>
            {
                ResourceController cc = go.GetComponent<ResourceController>();
                if (cc == null)
                    return false;

                //같은 팀이라면.
                if (cc.ObjectInfo.TeamId == this.ObjectInfo.TeamId)
                    return false;

                //유닛 타입이 자원이라면
                if (cc.ObjectInfo.StatInfo.UnitType != UnitType.Resource)
                    return false;


                Vector3Int dir = (cc.CellPos - CellPos);
                if (dir.magnitude > StatInfo.SearchRange)
                    return false;

                return true;
            });

            if (_target != null)
            {
                ObjectInfo.TargetId = _target.GetComponent<CreatureController>().ObjectInfo.UnitId;
            }

        }

    }
}
