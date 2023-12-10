using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIGlobalCfg))]
public class UIGlobalCfgEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("刷新"))
        {
            ((UIGlobalCfg)target).RefreshGlobalCfg();
        }
    }
}