using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    public enum UIBaseAniType
    {
        Open,
        Close,
    }
    private Animator animator;
    private GraphicRaycaster raycaster;

    public ResLoader resLoader;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        raycaster = GetComponent<GraphicRaycaster>();
        OnInit();
    }

    public virtual void OnInit()
    {
    }

    public virtual async void OnOpen()
    {
        if (animator != null)
        {
            if (animator.HasState(0, Animator.StringToHash(UIBaseAniType.Open.ToString())))
            {
                raycaster.enabled = false;
                animator.Play(UIBaseAniType.Open.ToString());
                await UniTask.Delay(Mathf.RoundToInt(animator.GetCurrentAnimatorStateInfo(0).length * 1000));
            }
        }
        raycaster.enabled = true;
    }

    public virtual void OnUpdate()
    {

    }

    public virtual async void OnClose(bool bRecycle = true)
    {
        raycaster.enabled = false;
        if (animator != null)
        {
            if (animator.HasState(0, Animator.StringToHash(UIBaseAniType.Close.ToString())))
            {
                animator.Play(UIBaseAniType.Close.ToString());
                await UniTask.Delay(Mathf.RoundToInt(animator.GetCurrentAnimatorStateInfo(0).length * 1000));
            }
        }
        if (!bRecycle)
        {
            OnRealDestory();
        }
    }

    public virtual void OnRealDestory()
    {
        Destroy(gameObject);
        resLoader.Dispose();
        resLoader = null;
    }
}