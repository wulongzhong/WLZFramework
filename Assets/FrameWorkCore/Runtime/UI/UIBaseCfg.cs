using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIBaseCfg
{
    public enum SortLayer
    {
        Default = 0,
        Middle = 100,
        Top = 200,
    }

    [ReadOnly]
    public string uiName;
    public SortLayer sortLayer;
    [ReadOnly]
    public string prefabPath;
}