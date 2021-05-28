using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Bullet
{
    public const int MAX_SHOTS = 3;

    private void Start()
    {
        base.Init();
        shots++;
        damage = 1;
    }

    private void OnDestroy()
    {
        if (shots > 0)
        {
            shots--;
        }
    }

    public static int shots { get; set; } = 0;
}
