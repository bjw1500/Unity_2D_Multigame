using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CreatureController
{


    protected override void Init()
    {


        base.Init();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f, 0);
        transform.position = pos;


    }

    // Update is called once per frame


    protected override void UpdateController()
    {
        base.UpdateController();
    }

    protected override void UpdateMoving()
    {
        //������ ���Ѵ�.
        Vector3 dest = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = dest - transform.position;

        //����ٰ� �� �����ָ� �ٸ� �÷��̾��� ���� ��ȯ�� �ȵȴ�.
        if (ObjectInfo.PosInfo.MoveDir == MoveDir.Right)
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        else if (ObjectInfo.PosInfo.MoveDir == MoveDir.Left)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

        if (moveDir.magnitude < Speed * Time.deltaTime)
        {
            //��ǥ ������ ����
            transform.position = dest;
            MoveToNextPos();
            return;
        }
        else
        {
            transform.position += moveDir.normalized * Speed * Time.deltaTime;
        }

    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

}
