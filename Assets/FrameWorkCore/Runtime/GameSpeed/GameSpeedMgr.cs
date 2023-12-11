using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpeedMgr : MonoBehaviour
{
    [Range(0f, 16f)]
    [SerializeField]
    private float gameSpeed = 1;

    public float GameSpeed
    {
        get { return gameSpeed; }
        set { gameSpeed = value; RefreshGameSpeed(); }
    }

    private void Awake()
    {
        Time.timeScale = gameSpeed;
    }

    private void RefreshGameSpeed()
    {
        Time.timeScale = gameSpeed;
    }
}
