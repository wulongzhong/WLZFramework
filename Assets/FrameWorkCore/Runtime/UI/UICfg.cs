using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UICfg", menuName = "UiTools/UiDevToolObject", order = 1)]
public class UICfg : ScriptableObject
{
    public string uiPrefabRootPath;
    public string uiCodeRootPath;
    [HideInInspector]
    public List<UISortInfo> listSortInfo;

    public static void Create()
    {

    }
}