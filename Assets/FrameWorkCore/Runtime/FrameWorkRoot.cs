using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameWorkRoot : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
