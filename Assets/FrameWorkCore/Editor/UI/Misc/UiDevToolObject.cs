using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "UiDevToolObject", menuName = "UiTools/UiDevToolObject", order = 1)]
public class UiDevToolObject : ScriptableObject
{
    public string uiScenesRootPath = "Assets/GamePlay/Scenes/UI";
    public string uiItemPrefabRootPath = "Assets/GamePlay/Prefabs/UIItem";

    public string uiTempletePath = "Assets/FrameWorkCore/Editor/UI/Templete";
    public string uiTempleteScenePath = "Assets/FrameWorkCore/Editor/UI/Templete/UiTemplete";

    private static UiDevToolObject instance;
    public static UiDevToolObject Instance()
    {
        if (instance == null)
        {
            instance = AssetDatabase.LoadAssetAtPath<UiDevToolObject>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:UiDevToolObject")[0]));
        }
        return instance;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void OnDisable()
    {
        instance = null;
    }
}