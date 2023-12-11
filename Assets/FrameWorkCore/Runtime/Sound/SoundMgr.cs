using ConfigPB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    public static SoundMgr Instance;

    [SerializeField]
    private AudioSource bgmSource;

    private ResLoader resLoader;

    public void Awake()
    {
        Instance = this;
        resLoader = new ResLoader();
    }

    public void PlaySound(string soundId)
    {
        var cfg = DTSound.Instance.GetSoundById(soundId);
        var audioClip = resLoader.LoadAsset<AudioClip>(cfg.AssetPath);
        AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position);
    }

    public void PlayBgm(string bgmId)
    {
        StartCoroutine(switchBgm(bgmId));
    }

    private IEnumerator switchBgm(string bgmId)
    {
        float interval = 0.5f;
        float startTime = Time.time;
        while(Time.time - startTime < interval)
        {
            bgmSource.volume = (0.5f - (Time.time - startTime)) / interval;
            yield return null;
        }
        startTime = Time.time;
        var cfg = DTSound.Instance.GetSoundById(bgmId);
        bgmSource.clip = resLoader.LoadAsset<AudioClip>(cfg.AssetPath);
        while(Time.time - startTime < interval)
        {
            bgmSource.volume = (Time.time - startTime) / interval;
            yield return null;
        }
        bgmSource.volume = 1;
    }
}
