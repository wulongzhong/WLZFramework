using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public partial class UIMain : UIBase, IEventHandle
{
    public int EventHandlerIndex { get; set; }
    public override void OnInit()
    {
        base.OnInit();
    }
    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void OnClose(bool bRecycle = true)
    {
        base.OnClose(bRecycle);
        if (EventHandlerIndex != 0)
        {
            this.ClearUpEventHandle();
        }
    }
}
