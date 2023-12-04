using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class UiScenePreview
{
    private UiDevToolObject toolObject;

    string strNewSceneName = string.Empty;
    Vector2 scrollPos = Vector2.zero;

    public void Init(UiDevToolObject toolObject)
    {
        this.toolObject = toolObject;
    }

    public void Draw()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Height(48));
        strNewSceneName = EditorGUILayout.TextField(strNewSceneName);
        if (GUILayout.Button("创建场景"))
        {
            var path = $"{toolObject.uiTempleteScenePath}.unity";
            var newPath = $"{toolObject.uiScenesRootPath}/{strNewSceneName}.unity";
            strNewSceneName = string.Empty;
            AssetDatabase.CopyAsset(path, newPath);
            AssetDatabase.Refresh();

            EditorSceneManager.OpenScene(newPath);
        }
        EditorGUILayout.EndHorizontal();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        var allFilePath = Directory.GetFiles(toolObject.uiScenesRootPath);
        foreach (var filePath in allFilePath)
        {
            var strs = filePath.Split('|', '/', '\\', '.');
            if (strs[strs.Length - 1] != "unity")
            {
                continue;
            }
            var strName = strs[strs.Length - 2];

            GUILayout.BeginHorizontal(GUILayout.Width(320));
            if (GUILayout.Button(strName, GUILayout.Width(160)))
            {
                //AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath).GetInstanceID());
            }
            if (GUILayout.Button("打开场景", GUILayout.Width(72)))
            {
                EditorSceneManager.OpenScene(filePath);
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}