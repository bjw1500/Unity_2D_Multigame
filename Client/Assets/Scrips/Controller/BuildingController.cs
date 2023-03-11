using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : CreatureController
{
    // Start is called before the first frame update
    protected override void Init()
    {
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f, 0);
        transform.position = pos;
        AddHpBar();
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    protected override void UpdateAnimation()
    {
        
    }

    public override void OnDead(ObjectInfo attacker)
    {
        base.OnDead(attacker);


        //
        
        //C_WinGame wingame = new C_WinGame();
        //Managers.NetWork.Send(wingame);
    }

}
