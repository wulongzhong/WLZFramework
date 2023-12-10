using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    public static SoundMgr Instance;

    public void Awake()
    {
        Instance = this;
    }

    public void PlaySound(string soundId)
    {
        //var cfg = DTSound.Instance.GetSoundById(soundId);
        //AudioSource.PlayClipAtPoint()
    }
}
