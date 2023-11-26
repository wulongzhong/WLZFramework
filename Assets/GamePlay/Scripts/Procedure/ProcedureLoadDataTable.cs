using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureLoadDataTable : ProcedureBase
{
    public override void OnEnter()
    {
        base.OnEnter();
        DataTableMgr.Instance.InitDataTable();
    }
}