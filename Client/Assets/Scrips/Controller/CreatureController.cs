using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{

   
    protected Animator _anim = null;

    protected override void Init()
    { 
        
        if (ObjectInfo.StatInfo.UnitType == UnitType.Horse)
        {
            _anim = gameObject.transform.Find("HorseRoot").GetComponent<Animator>();
           // _anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/HorseNewController");
        }
        else
        {
            _anim = gameObject.transform.Find("UnitRoot").GetComponent<Animator>();
            _anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animation/AnimationNewController");
        }

        State = CreatureState.Idle;
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f, 0);
        transform.position = pos;
        AddHpBar();
        //TODO
        //플레이어가 유닛이 가기를 원하는 방향? 설정하기. 지금은 임시로 캐슬 포인트로 해놓음.
        Destination = new Vector3Int(0, 0, 0);
    }



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


    protected override void MoveToNextPos()
    {
        
    }


    protected override void UpdateAnimation()
    {
        switch (State)
        {
            case CreatureState.Idle:
                _anim.Play("0_idle");
                break;
            case CreatureState.Moving:
                _anim.Play("1_Run");
                break;
            case CreatureState.Attack:
                if (AttackType == AttackType.Heal)
                {
                    _anim.Play($"2_Attack_Normal");
                    break;
                }
                string Type = AttackType.ToString();
                    _anim.Play($"2_Attack_{Type}");
                break;
            case CreatureState.Dead:
                _anim.Play("4_Death");
                break;
        }
    }

    protected override void UpdateMoving()
    {




    }

    protected override void UpdateIdle()
    {



    }

    protected override void UpdateAttack()
    {

    }

  

    protected override void UpdateDead()
    {
 

    }
    public override void OnDamaged(ObjectInfo attacker, SkillInfo attackSkill)
    {
        


        base.OnDamaged(attacker, attackSkill);

        //누가 때렸는가를 보낸다?
        //결과적으로 플레이어가 공격한 결과값을 보냄.
        C_ChangeHp hpPacket = new C_ChangeHp();
        hpPacket.PlayerId = ObjectInfo.Player.PlayerId;
        hpPacket.UnitId = ObjectInfo.UnitId;

        hpPacket.Attacker = attacker;
        hpPacket.AttackSkill = attackSkill;

        Managers.NetWork.Send(hpPacket);

        Debug.Log($"Attack!{ObjectInfo.UnitId}");
        Debug.Log($"{gameObject.name} OnDamaged!");


    }

    public override void OnDead(ObjectInfo attacker)
    {
        base.OnDead(attacker);

        State = CreatureState.Dead;
        Debug.Log($"{attacker.Name}에 의해 {ObjectInfo.Name}가 사망했습니다.");


        //죽었으니 디스폰하기.

            //C_Despawn DespawnPacket = new C_Despawn();
            //DespawnPacket.Info.Add(this.ObjectInfo);
            //Managers.NetWork.Send(DespawnPacket);
        

    }


    protected void CheckUpdatePosition()
    {
        
        //Destination, State, Dir 변화가 생겼다면 실행.
        C_Move movePacket = new C_Move();
        movePacket.UnitId = ObjectInfo.UnitId;
        movePacket.PosInfo = ObjectInfo.PosInfo;
        Managers.NetWork.Send(movePacket);

    }

    protected void CheckUpdateAttack(int skillid)
    {

        //서버에 스킬 사용 요청 패킷 보내기.            
        C_Skill skillPacket = new C_Skill();
        skillPacket.Info = ObjectInfo;
        skillPacket.Skillid = skillid;
        Managers.NetWork.Send(skillPacket);
    }

    public virtual void UseSkill(int skillId)
    {

    }


    IEnumerator CoCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        _coCoolTime = null;
    }


    IEnumerator CoTargetTime(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log(ObjectInfo.Name + "_coTargetTime End");
        _target = null;
        _coTargetTime = null;
        State = CreatureState.Idle;
    }

   


}
