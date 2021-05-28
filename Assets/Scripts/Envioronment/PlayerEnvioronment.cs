using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnvioronment
{
    public static PlayerEnvioronment singleton { get; } = new PlayerEnvioronment();

    private PlayerEnvioronment()
    {

    }

    public AudioClip shotSE { get; set; }
    public AudioClip damageSE { get; set; }
    public AudioClip destroySE { get; set; }

    public void PlayShotSE()
    {
        GeneralEnvioronment.singleton.PlaySE(shotSE);
    }

    public void PlayDamageSE()
    {
        GeneralEnvioronment.singleton.PlaySE(damageSE);
    }

    public void PlayDestroySE()
    {
        GeneralEnvioronment.singleton.PlaySE(destroySE);
    }

}
