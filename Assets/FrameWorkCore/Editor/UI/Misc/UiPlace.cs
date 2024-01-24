using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text;

public class UiPlace
{
    UiDevToolObject uiDevToolObject;
    bool bSelectNew = false;
    bool bUnpackNew = false;

    List<string> listSelectDir;
    List<Vector2> listDirScrollPos;

    private GUILayoutOption dirBtnWidth;
    private GUILayoutOption prefabBtnWidth;

    public void Init(UiDevToolObject uiDevToolObject)
    {
        this.uiDevToolObject = uiDevToolObject;
        listSelectDir = new List<string>();
        listDirScrollPos = new List<Vector2>();
        RefreshUIPrefabRootDir();
        PraseFilter();
    }

    public void Draw()
    {
        if (dirBtnWidth == null)
        {
            dirBtnWidth = GUILayout.Width(96);
            prefabBtnWidth = GUILayout.Width(160);
        }

        DrawOption();
        GUILayout.BeginHorizontal();
        DrawFilter();
        DrawGroup();
        DrawPrefab();
        GUILayout.EndHorizontal();
    }

    private void RefreshUIPrefabRootDir()
    {
        listSelectDir.Clear();
        listSelectDir.Add(uiDevToolObject.uiItemPrefabRootPath);
    }

    GUILayoutOption optDrawOption = GUILayout.Width(128);
    private void DrawOption()
    {
        GUILayout.BeginHorizontal();
        bSelectNew = GUILayout.Toggle(bSelectNew, "命中新增");
        bUnpackNew = GUILayout.Toggle(bUnpackNew, "解除新增的预制状态");
        GUILayout.EndHorizontal();
    }

    private List<string> listFilter;
    string[] strFilter = null;
    string strAddFilter = string.Empty;

    private void PraseFilter()
    {
        listFilter = new List<string>(EditorPrefs.GetString(nameof(listFilter), "").Split('|', System.StringSplitOptions.RemoveEmptyEntries));
    }

    private void SaveFilter()
    {
        StringBuilder stringBuilder = new StringBuilder();
        foreach(var filter in listFilter)
        {
            stringBuilder.Append(filter).Append('|');
        }
        EditorPrefs.SetString(nameof(listFilter), stringBuilder.ToString());
    }

    private void DrawFilter()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(140));

        if(GUILayout.Button("显示全部", GUILayout.Width(96)))
        {
            strFilter = null;
        }

        for(int i = 0; i < listFilter.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(140));
            var filter = listFilter[i];
            if (GUILayout.Button(filter, GUILayout.Width(96)))
            {
                strFilter = filter.Split(',', System.StringSplitOptions.RemoveEmptyEntries);
            }
            if(GUILayout.Button("-", GUILayout.Width(12)))
            {
                listFilter.Remove(filter);
                SaveFilter();
                EditorGUILayout.EndHorizontal();
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal(GUILayout.Width(140));
        strAddFilter = GUILayout.TextField(strAddFilter, GUILayout.Width(96));
        if(GUILayout.Button("+", GUILayout.Width(12)))
        {
            listFilter.Add(strAddFilter);
            SaveFilter();
            strAddFilter = string.Empty;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawGroup()
    {
        GUILayout.BeginHorizontal();
        int i = 0;
        while(i < listSelectDir.Count)
        {
            if (!Directory.Exists(listSelectDir[i]))
            {
                listSelectDir.RemoveRange(i, listSelectDir.Count - i);
                break;
            }
            string[] dirs = Directory.GetDirectories(listSelectDir[i]);
            if(dirs.Length == 0)
            {
                listSelectDir.RemoveRange(i + 1, listSelectDir.Count - (i + 1));
                break;
            }

            if(listDirScrollPos.Count <= i)
            {
                listDirScrollPos.Add(Vector2.zero);
            }
            listDirScrollPos[i] = GUILayout.BeginScrollView(listDirScrollPos[i], dirBtnWidth);
            foreach(var dir in dirs)
            {
                bool bSelect = false;
                if(listSelectDir.Count > (i + 1))
                {
                    bSelect = (dir == listSelectDir[i + 1]);
                }
                string[] strs = dir.Split('|','/','\\');
                GUI.color = bSelect ? Color.green : Color.white;
                if (GUILayout.Button(strs[strs.Length - 1]))
                {
                    strFilter = null;
                    if (listSelectDir.Count > (i + 1))
                    {
                        listSelectDir[i + 1] = dir;
                        listSelectDir.RemoveRange(i + 2, listSelectDir.Count - (i + 2));
                    }
                    else
                    {
                        listSelectDir.Add(dir);
                    }
                }
            }
            GUILayout.EndScrollView();
            ++i;
        }
        GUILayout.EndHorizontal();

        GUI.color = Color.white;
    }

    Vector2 prefabScrollPos = Vector2.zero;
    private void DrawPrefab()
    {
        prefabScrollPos = GUILayout.BeginScrollView(prefabScrollPos);
        var allFilePath = Directory.GetFiles(listSelectDir[listSelectDir.Count - 1]);
        foreach(var filePath in allFilePath)
        {
            var strs = filePath.Split('|', '/','\\','.');
            if (strs[strs.Length - 1] != "prefab")
            {
                continue;
            }
            var strName = strs[strs.Length - 2];

            bool bFilterSuccess = true;
            if (strFilter != null)
            {
                foreach (var str in strFilter)
                {
                    if(strName.IndexOf(str) == -1)
                    {
                        bFilterSuccess = false;
                        break;
                    }
                }
            }

            if (!bFilterSuccess)
            {
                continue;
            }

            GUILayout.BeginHorizontal(GUILayout.Width(320));
            if(GUILayout.Button(strName, prefabBtnWidth))
            {
                AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(filePath).GetInstanceID());
            }
            if (GUILayout.Button("添加", GUILayout.Width(72)))
            {
                if ((Selection.activeGameObject != null) && (Selection.activeGameObject.activeInHierarchy))
                {
                    GameObject gbNew = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(filePath), Selection.activeGameObject.transform);
                    Undo.RegisterCreatedObjectUndo(gbNew, nameof(gbNew));
                    if (bSelectNew)
                    {
                        Selection.activeGameObject = gbNew;
                    }
                    if (bUnpackNew)
                    {
                        PrefabUtility.UnpackPrefabInstance(gbNew, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }
}