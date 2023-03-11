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
        //Ŭ���ϴ� ���� ��ǥ�� ���� �Ѵ�.
        //��ǥ�� ��ǥ ���� ������, �� ��ǥ���� ���� ĳ���Ͱ� ��ã�� �˰����� ����� �̵��Ѵ�.
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

            Debug.Log("���콺 Ŭ��");

            //�ؾ� �� ����
            //Ŭ���� �ؼ� ��ǥ���� ��´�.
            //Map Finder�� ��ã�� �˰����� ���´�.
            //ĳ���͸� �̵���Ų��. 
        }
    }




    protected override void UpdateMoving()
    {
        if (State == CreatureState.Dead)
            return;

        Vector3 dest = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = dest - transform.position;

        //����ٰ� �� �����ָ� �ٸ� �÷��̾��� ���� ��ȯ�� �ȵȴ�.
        RotationToDir();

        if (moveDir.magnitude < Speed * Time.deltaTime)
        {
            //��ǥ ������ ����
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


        //Ÿ���� �����ϰ�, ���� ���� ���°� �ƴҶ�.

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

            //Ÿ���� ������ �� ���ٰ� �ǴܵǸ� ���ο� ����� ã��.
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


        //�׾����� �����ϱ�.

        C_Despawn DespawnPacket = new C_Despawn();
        DespawnPacket.Info.Add(this.ObjectInfo);
        Managers.NetWork.Send(DespawnPacket);


    }

    public override void UseSkill(int skillId)
    {
        RotationToDir();

        //attack Speed�� �־��� ������
        //�̰� ������ ������ attack animation�� ����� �ȵ�
        //attack animation ����� �ٸ� �ִϸ��̼� ������� �ʱ�ȭ ����� ��.

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
        //Idle�� �ٲ������ ���� ����� �׾��� ��, �ٸ� ����� ã��.
        State = CreatureState.Idle;
        _coSkill = null;
        //canAttack = true;
    }


    IEnumerator CoStartHealAttack(float time)
    {
        //Ÿ���� �Ʊ�.
        //�����ϸ� �Ʊ��� ü�� ȸ��.
        //�����Ÿ�?


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
        //Idle�� �ٲ������ ���� ����� �׾��� ��, �ٸ� ����� ã��.
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

            //Ÿ���� ���� ������ �����.
            _target = Managers.Object.Find(WorldType, gameObject, (go) =>
            {
                CreatureController cc = go.GetComponent<CreatureController>();
                if (cc == null)
                    return false;

                //���� Ÿ���� �ڿ��̶��
                if (cc.ObjectInfo.StatInfo.UnitType == UnitType.Resource)
                    return false;

                Vector3Int dir = (cc.CellPos - CellPos);
                if (dir.magnitude > StatInfo.SearchRange)
                    return false;

                //Ǯ�Ƕ��,
                if (cc.ObjectInfo.StatInfo.Hp == cc.ObjectInfo.StatInfo.MaxHp)
                    return false;

                //���� ���� �ƴ϶��
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
