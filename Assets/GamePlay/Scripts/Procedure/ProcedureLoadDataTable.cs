using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureLoadDataTable : ProcedureBase
{
    public override void OnEnter()
    {
        Splash.Instance.RefreshProgress(10);
        base.OnEnter();
        Splash.Instance.RefreshProgress(11);
        DataTableMgr.Instance.InitDataTable();
        Splash.Instance.RefreshProgress(12);
    }
}