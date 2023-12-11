using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameSpeedMgr))]
public class GameSpeedMgrEditor : Editor
{
    static float[] smallSpeed = new float[4] { 0.125f, 0.25f, 0.5f, 0.75f };
    static float[] bigSpeed = new float[4] { 2, 4, 8, 16 };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        bool bDirty = false;
        GameSpeedMgr gameSpeedMgr = (GameSpeedMgr)target;
        GUILayout.BeginHorizontal();
        foreach (var speed in smallSpeed)
        {
            if (GUILayout.Button(speed.ToString()))
            {
                gameSpeedMgr.GameSpeed = speed;
                bDirty = true;
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        foreach (var speed in bigSpeed)
        {
            if (GUILayout.Button(speed.ToString()))
            {
                gameSpeedMgr.GameSpeed = speed;
                bDirty = true;
            }
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Reset"))
        {
            gameSpeedMgr.GameSpeed = 1;
            bDirty = true;
        }
        if(bDirty && !UnityEditor.EditorApplication.isPlaying)
        {
            EditorUtility.SetDirty(gameSpeedMgr);
        }
    }
}