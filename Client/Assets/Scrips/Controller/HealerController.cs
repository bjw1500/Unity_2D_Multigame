using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerController : MyPlayerController
{
    Vector3 targetPoint = Vector3.zero;

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
        GetDirInput();
        base.UpdateController();

    }

    void GetDirInput()
    {
        //클릭하는 곳의 좌표를 얻어야 한다.
        //목표한 좌표 값을 찍으면, 그 좌표값을 향해 캐릭터가 길찾기 알고리즘을 사용해 이동한다.
        SetDestination();


    }

    void SetDestination()
    {
        if (Input.GetMouseButtonDown(1))
        {

            Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int destination = new Vector3Int();
            destination.x = Mathf.RoundToInt(mousePoint.x);
            destination.y = Mathf.RoundToInt(mousePoint.y);
            destination.z = 0;
            Destination = destination;

            State = CreatureState.Moving;

            Debug.Log("마우스 클릭");

            //해야 할 순서
            //클릭을 해서 좌표값을 얻는다.
            //Map Finder로 길찾기 알고리즘을 따온다.
            //캐릭터를 이동시킨다. 
        }
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
        if (_target != null)
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
        else if (_target != null && State != CreatureState.Dead)
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

    public override void Attack()
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
            StopCoroutine("CoTargetTime");
            _coTargetTime = null;

            if (_coSkill == null)
            {
                //canAttack = false;
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

        //attack Speed를 넣어준 이유는
        //이걸 해주지 않으면 attack animation이 재생이 안됨
        //attack animation 재생후 다른 애니메이션 재생으로 초기화 해줘야 함.

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
            case 3:
                _coSkill = StartCoroutine("CoStartHealAttack", StatInfo.AttackSpeed);
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
        //Idle로 바꿔줘야지 공격 대상이 죽었을 때, 다른 대상을 찾음.
        State = CreatureState.Idle;
        _coSkill = null;
        //canAttack = true;
    }


    IEnumerator CoStartHealAttack(float time)
    {
        //타겟은 아군.
        //공격하면 아군의 체력 회복.
        //사정거리?


        GameObject go = _target;
        if (go != null)
        {
            CreatureController cc = go.GetComponent<CreatureController>();
            if (cc != null)
            {
                cc.OnDamaged(this.ObjectInfo, new SkillInfo() { SkillId = 3 });
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

            //타겟을 같은 팀으로 만들기.
            _target = Managers.Object.Find(WorldType, gameObject, (go) =>
            {
                CreatureController cc = go.GetComponent<CreatureController>();
                if (cc == null)
                    return false;

                //유닛 타입이 자원이라면
                if (cc.ObjectInfo.StatInfo.UnitType == UnitType.Resource)
                    return false;

                Vector3Int dir = (cc.CellPos - CellPos);
                if (dir.magnitude > StatInfo.SearchRange)
                    return false;

                //풀피라면,
                if (cc.ObjectInfo.StatInfo.Hp == cc.ObjectInfo.StatInfo.MaxHp)
                    return false;

                //같은 팀이 아니라면
                if (cc.ObjectInfo.Player.PlayerId != this.ObjectInfo.Player.PlayerId)
                    return false;
  
                if (cc.ObjectInfo.TeamId != this.ObjectInfo.TeamId)
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
