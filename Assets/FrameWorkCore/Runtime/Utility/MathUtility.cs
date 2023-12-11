using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility
{
    public static float Dir2Rot(Vector2 dir)
    {
        return FixRot(Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI);
    }

    public static Vector2 Rot2Dir(float rot)
    {
        rot = rot * Mathf.PI / 180;
        return new Vector2(Mathf.Cos(rot), Mathf.Sin(rot));
    }

    public static float FixRot(float rot)
    {
        while (rot < 0)
        {
            rot += 360;
        }

        while (rot > 360)
        {
            rot -= 360;
        }

        return rot;
    }
}
