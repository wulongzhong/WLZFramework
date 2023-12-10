using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProcedureBase : MonoBehaviour
{
    public ProcedureMgr ProcedureMgr { get; set; }
    public virtual void OnEnter() { this.enabled = true; Debug.Log(this.GetType().FullName + ":Enter"); }

    public virtual void OnExit() {  this.enabled = false; Debug.Log(this.GetType().FullName + ":Exit"); }
}