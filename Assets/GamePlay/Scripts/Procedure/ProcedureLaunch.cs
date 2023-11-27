using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureLaunch : ProcedureBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        Splash.Instance.RefreshProgress(1);
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void Update()
    {
        Splash.Instance.RefreshProgress(2);
        this.ProcedureMgr.ChangeProcedure<ProcedureLoadDataTable>();
    }
}
