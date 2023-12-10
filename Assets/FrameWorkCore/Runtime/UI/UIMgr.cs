using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class UIMgr : MonoBehaviour
{
    public UIGlobalCfg globalCfg;
    private Dictionary<string, UIBase> dicUI;
    private Dictionary<UIBaseCfg.SortLayer, List<UIBase>> dicLayer2UIBaseBev;

    private void Awake()
    {
        dicUI = new Dictionary<string, UIBase>();
        dicLayer2UIBaseBev = new Dictionary<UIBaseCfg.SortLayer, List<UIBase>>();

        Type enumType = typeof(UIBaseCfg.SortLayer);
        var enumValues = Enum.GetValues(enumType);
        foreach (var enumValue in enumValues)
        {
            var v = (UIBaseCfg.SortLayer)enumValue;
            dicLayer2UIBaseBev.Add(v, new List<UIBase>());
        }
    }

    public UIBase OpenUI<T>(object userData) where T : UIBase
    {
        string uiName = typeof(T).Name;
        if (dicUI.ContainsKey(uiName) && dicUI[uiName].gameObject.activeSelf)
        {
            Debug.LogError($"UI:{uiName}已开启");
            return null;
        }
        var cfg = globalCfg.GetUIBaseCfg(uiName);
        var resLoader = new ResLoader();
        var prefab = resLoader.LoadAsset<GameObject>(cfg.prefabPath);
        var go = Instantiate(prefab, this.transform);
        var uiBase = go.GetComponent<UIBase>();
        uiBase.resLoader = resLoader;
        uiBase.OnOpen(userData);
        int sortingOrder = 0;
        if (dicLayer2UIBaseBev[cfg.sortLayer].Count > 0)
        {
            sortingOrder = dicLayer2UIBaseBev[cfg.sortLayer][^1].canvas.sortingOrder + 10;
        }
        uiBase.canvas.sortingOrder = sortingOrder;
        dicUI.Add(uiName, uiBase);
        dicLayer2UIBaseBev[cfg.sortLayer].Add(uiBase);
        return uiBase;
    }

    public void CloseUI<T>(bool bRecycle) where T : UIBase
    {
        string uiName = typeof(T).Name;
        if (!dicUI.ContainsKey(uiName))
        {
            Debug.LogError($"未找到该UI:{uiName}");
        }
        var cfg = globalCfg.GetUIBaseCfg(uiName);
        dicUI[uiName].OnClose(bRecycle);
        dicLayer2UIBaseBev[cfg.sortLayer].Remove(dicUI[uiName]);

        if (!bRecycle)
        {
            dicUI.Remove(uiName);
        }
    }

    public T FindUI<T>() where T : UIBase
    {
        string uiName = typeof(T).Name;
        if (dicUI.ContainsKey(uiName))
        {
            return (T)dicUI[uiName];
        }
        return null;
    }

    public bool IsShow(string uiName)
    {
        if (dicUI.ContainsKey(uiName) && dicUI[uiName].gameObject.activeSelf)
        {
            return true;
        }
        return false;
    }
}