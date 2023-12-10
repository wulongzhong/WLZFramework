using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "UIGlobalCfg", menuName = "UiTools/UIGlobalCfg", order = 1)]
public class UIGlobalCfg : ScriptableObject
{
    public static UIGlobalCfg Instance
    {
        get
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<UIGlobalCfg>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:UIGlobalCfg")[0]));
#else
            return null;
#endif
        }
    }

    public string uiPrefabRootPath;
    public string uiCodeRootPath;
    public List<UIBaseCfg> listSortInfo;

    private Dictionary<string, UIBaseCfg> dicUIName2Cfg;

#if UNITY_EDITOR
    public void RefreshGlobalCfg()
    {
        var allAssetPath = System.IO.Directory.GetFiles(uiPrefabRootPath);
        foreach(var path in allAssetPath)
        {
            if (path.Contains(".meta"))
            {
                continue;
            }

            var cfg = listSortInfo.Find(cfg => cfg.prefabPath == path);
            if(cfg != null)
            {
                continue;
            }

            cfg = new UIBaseCfg();
            cfg.uiName = path.Split('.')[^2];
            cfg.prefabPath = path;
            cfg.sortLayer = UIBaseCfg.SortLayer.Default;
            listSortInfo.Add(cfg);
        }
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }

#endif

    public void Init()
    {
        dicUIName2Cfg = new Dictionary<string, UIBaseCfg>();
        foreach(var baseCfg in listSortInfo)
        {
            dicUIName2Cfg.Add(baseCfg.uiName, baseCfg);
        }
    }

    public UIBaseCfg GetUIBaseCfg(string uiName)
    {
        return dicUIName2Cfg[uiName];
    }
}