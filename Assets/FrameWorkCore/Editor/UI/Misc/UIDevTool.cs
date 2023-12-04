using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public class UIDevTool : EditorWindow
{
    private UiDevToolObject toolObject;
    private UiScenePreview uiScenePreview;
    private UiPlace uiPlace;
    private UiPrefabCreate uiPrefabCreate;

    int currSelectFuncIndex = 0;
    string[] arrFuncTip = new string[] { "Ui场景列表", "Ui预制体列表", "预制体模板" };

    [MenuItem("FrameworkTools/Ui工具板")]
    public static void OpenUiDevTool()
    {
        UIDevTool uiDevTool = GetWindow<UIDevTool>();
        uiDevTool.titleContent = new GUIContent("Ui工具板");
        uiDevTool.Init();
        uiDevTool.Show();
    }
    private void Init()
    {
        toolObject = UiDevToolObject.Instance();

        uiScenePreview = new UiScenePreview();
        uiScenePreview.Init(toolObject);

        uiPlace = new UiPlace();
        uiPlace.Init(toolObject);

        uiPrefabCreate = new UiPrefabCreate();
        uiPrefabCreate.Init(toolObject);
    }

    private void OnEnable()
    {
        Init();
    }

    private void OnGUI()
    {
        currSelectFuncIndex = GUILayout.Toolbar(currSelectFuncIndex, arrFuncTip);
        switch (currSelectFuncIndex)
        {
            case 0:
                uiScenePreview.Draw();
                break;
            case 1:
                uiPlace.Draw();
                break;
            case 2:
                uiPrefabCreate.Draw();
                break;
        }
    }
}
