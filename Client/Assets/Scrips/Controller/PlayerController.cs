using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{

    //스탯 타입


    protected override void Init()
    {


        base.Init();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f, 0);
        transform.position = pos;
        

    }

    // Update is called once per frame


    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Attack:
                UpdateAttack();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
            default:
                break;
        }

        UpdateAnimation();
    }


    Coroutine _coIdleTime;
    public override void UseSkill(int skillId)
    {
        RotationToDir();

        switch (skillId)
        {
            case 0:
                StartNormalAttack();
                break;
            case 1:
                StartBowAttack();
                break;
            case 2:
                StartMagicAttack();
                break;
            case 3:
                StartCoroutine("CoStartHealAttack", StatInfo.AttackSpeed);
                break;
            default:
                break;
        }

        if(_coIdleTime == null)
            _coIdleTime = StartCoroutine("CoIdleTime", StatInfo.AttackSpeed);

        return;
    }

    IEnumerator CoIdleTime(float time)
    {
        yield return new WaitForSeconds(time);
        _coIdleTime = null;
        State = CreatureState.Idle;
    }

    public void StartNormalAttack()
    {
       

    }

    public void StartBowAttack()
    {

        //원거리 투사체 생성
        GameObject go = Managers.Resource.Instantiate("Weapon/Arrow");
        ArrowController cc = go.GetComponent<ArrowController>();

        cc.transform.position = transform.position;
        cc._target = _target;
        cc._master = this.ObjectInfo;
        cc.noDamage = true;
        cc.skillinfo = new SkillInfo() { SkillId = 1 };


    }

    public void StartMagicAttack()
    {
        //활처럼 투사체 생성해주기.
        

    }

    public void StartHealAttack()
    {
        //활처럼 투사체 생성해주기.


    }

    protected override void UpdateMoving()
    {
        if (State == CreatureState.Dead)
            return;


        //방향을 구한다.
        Vector3 dest = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = dest - transform.position;

        //여기다가 또 안해주면 다른 플레이어의 방향 전환이 안된다.
        RotationToDir();

        if (moveDir.magnitude < Speed * Time.deltaTime)
        {
            //목표 지점에 도착
            transform.position = dest;
          //  Debug.Log(ObjectInfo.UnitId + " " + transform.position.x + " " +transform.position.y);
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
        
    }


    protected override void UpdateIdle()
    {
        //base.UpdateIdle();
    }



}
