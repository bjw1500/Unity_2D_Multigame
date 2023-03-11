using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : CreatureController
{
    protected override void Init()
    {
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f, 0);
        transform.position = pos;
    }

    // Update is called once per frame
    protected override void Update()
    {

    }


}
