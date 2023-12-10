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
    [HideInInspector]
    public Canvas canvas;
    private Animator animator;
    private GraphicRaycaster raycaster;
    [HideInInspector]
    public ResLoader resLoader;

    [Header("本地化模块类型")]
    public ConfigPB.LocalizationModuleType localizationModuleType;
    [Header("默认按钮音效ID")]
    public string btnDefaultClickSoundId = "button_click";

    [Space]
    [Header("此处往下为自动生成，勿手动编辑")]
    [Space]

    public TextMeshProUGUI[] arrStaticLocalizationText;
    public Button[] arrPlaySoundBtn;
    public string[] arrBtnSoundId;

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

    protected bool bCloseRecycle = true;

    private void Awake()
    {
        OnInit();
    }

    public virtual void OnInit()
    {
        canvas = GetComponent<Canvas>();
        animator = GetComponent<Animator>();
        raycaster = GetComponent<GraphicRaycaster>();
        foreach(var text in arrStaticLocalizationText)
        {
            text.text = DTLocalization.Instance.GetString(localizationModuleType, text.text);
        }

        if (arrPlaySoundBtn != null && arrPlaySoundBtn.Length > 0)
        {
            for (int i = 0; i < arrPlaySoundBtn.Length; ++i)
            {
                int tempIndex = i;
                var soundId = arrBtnSoundId[tempIndex];
                if (soundId.Equals("default"))
                {
                    soundId = btnDefaultClickSoundId;
                }
                arrPlaySoundBtn[tempIndex].onClick.AddListener(() => { SoundMgr.Instance.PlaySound(soundId); });
            }
        }
    }

    public virtual void OnOpen(object userData)
    {
        BPlayingOpenAnimation = false;
        bCloseRecycle = true;
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
        bCloseRecycle = bRecycle;
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

    public virtual void OnCloseAnimationEnd()
    {
        BPlayingCloseAnimation = false;
        if (!bCloseRecycle)
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