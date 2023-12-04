using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ProcedureMgr))]
public class ProcedureMgrEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ProcedureMgr procedureMgr = (ProcedureMgr)target;
        if(!UnityEditor.EditorApplication.isPlaying)
        {
            bool bDirty = false;
            foreach(var procedureBase in procedureMgr.gameObject.GetComponents<ProcedureBase>())
            {
                if(procedureBase.enabled)
                {
                    procedureBase.enabled = false;
                    bDirty = true;
                }
            }
            if(bDirty)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }
}
