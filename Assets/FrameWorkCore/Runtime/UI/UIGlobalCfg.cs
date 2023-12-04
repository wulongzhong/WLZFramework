using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIGlobalCfg", menuName = "UiTools/UIGlobalCfg", order = 1)]
public class UIGlobalCfg : ScriptableObject
{
    public string uiPrefabRootPath;
    public string uiCodeRootPath;
    public List<UIBaseCfg> listSortInfo;

    public static void Create()
    {

    }
}