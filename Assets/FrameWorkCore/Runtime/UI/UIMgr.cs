using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UIMgr : MonoBehaviour
{
    public string uiPrefabFolderPath;

    private Dictionary<string, UIBase> dicUI;

    private void Awake()
    {
        dicUI = new Dictionary<string, UIBase>();
    }

    public UIBase OpenUI<T>() where T : UIBase
    {
        string uiName = typeof(T).Name;
        if (dicUI.ContainsKey(uiName) && dicUI[uiName].gameObject.activeSelf)
        {
            Debug.LogError($"UI:{uiName}已开启");
            return null;
        }

        var resLoader = new ResLoader();
        var path = Path.Combine(uiPrefabFolderPath, $"{uiName}.prefab");
        var prefab = resLoader.LoadAsset<GameObject>(path);
        var go = Instantiate(prefab, this.transform);
        var uiBase = go.GetComponent<UIBase>();
        uiBase.resLoader = resLoader;
        uiBase.OnOpen();
        dicUI.Add(uiName, uiBase);
        return uiBase;
    }

    public void CloseUI<T>() where T : UIBase
    {
        string uiName = typeof(T).Name;
        if (!dicUI.ContainsKey(uiName))
        {
            Debug.LogError($"未找到该UI:{uiName}");
        }
        dicUI[uiName].OnClose();
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

        return false;
    }
}