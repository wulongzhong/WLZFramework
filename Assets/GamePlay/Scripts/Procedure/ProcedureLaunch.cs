using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureLaunch : ProcedureBase
{
    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void Update()
    {
        this.ProcedureMgr.ChangeProcedure<ProcedureLoadDataTable>();
    }
}
