using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 启动界面是单独的特殊UI，手写动画过渡等相关的，不要使用插件，并且直接通过单例调用，因为不被UI管理器所管理
/// </summary>
public class Splash : MonoBehaviour
{
    [Header("启动界面是单独的特殊UI，手写动画过渡等相关的，不要使用插件")]
    [Header("并且直接通过单例调用，因为不被UI管理器所管理")]

    [Space]
    [Header("进度提示文本")]
    public TMPro.TextMeshProUGUI tLoading;

    public static Splash Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void RefreshProgress(int progress)
    {
        tLoading.text = $"Loading {progress}/100";
    }
}
