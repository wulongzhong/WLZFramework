using TMPro;
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
    [HideInInspector]
    public ResLoader resLoader;

    [Header("本地化模块类型")]
    public ConfigPB.LocalizationModuleType localizationModuleType;
    [Header("默认按钮音效ID")]
    public int btnDefaultClickSoundId = 114;

    [Space]
    [Header("此处往下为自动生成，勿手动编辑")]
    [Space]

    public TextMeshProUGUI[] arrStaticLocalizationText;
    public Button[] arrPlaySoundBtn;
    public int[] arrBtnSoundId;

    /// <summary>
    /// 是否在播放开启动画中
    /// </summary>
    public bool BPlayingOpenAnimation {  get; set; }
    /// <summary>
    /// 是否在播放关闭动画中
    /// </summary>
    public bool BPlayingCloseAnimation { get; set; }
    /// <summary>
    /// 动画结束时间 公用
    /// </summary>
    public float AnimationEndTime { get; set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        raycaster = GetComponent<GraphicRaycaster>();
        OnInit();
    }

    public virtual void OnInit()
    {
    }

    public virtual void OnOpen(object userData)
    {
        BPlayingOpenAnimation = false;
        if (animator != null)
        {
            if (animator.HasState(0, Animator.StringToHash(UIBaseAniType.Open.ToString())))
            {
                raycaster.enabled = false;
                animator.Play(UIBaseAniType.Open.ToString());
                BPlayingOpenAnimation = true;
                AnimationEndTime = Time.time + animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }
        raycaster.enabled = true;
    }

    public virtual void OnOpenAnimationEnd()
    {
        raycaster.enabled = true;
        BPlayingOpenAnimation = false;
    }

    public virtual void OnClose(bool bRecycle = true)
    {
        BPlayingCloseAnimation = false;
        raycaster.enabled = false;
        if (animator != null)
        {
            if (animator.HasState(0, Animator.StringToHash(UIBaseAniType.Close.ToString())))
            {
                animator.Play(UIBaseAniType.Close.ToString());
                BPlayingCloseAnimation = true;
                AnimationEndTime = Time.time + animator.GetCurrentAnimatorStateInfo(0).length;
            }
        }
    }

    public virtual void OnCloseAnimationEnd(bool bRecycle = true)
    {
        BPlayingCloseAnimation = false;
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

    protected virtual void OnUpdate()
    {
        if (BPlayingOpenAnimation && Time.time > AnimationEndTime)
        {
            OnOpenAnimationEnd();
        }
        if (BPlayingCloseAnimation && Time.time > AnimationEndTime)
        {
            OnCloseAnimationEnd();
        }
    }
}