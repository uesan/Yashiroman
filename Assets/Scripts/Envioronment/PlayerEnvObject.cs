using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnvObject : MonoBehaviour
{
    public AudioClip shotSE;
    public AudioClip damageSE;
    public AudioClip destroySE;

    private PlayerEnvioronment playerEnv = PlayerEnvioronment.singleton;

    private void Start()
    {
        playerEnv.shotSE = shotSE;
        playerEnv.damageSE = damageSE;
        playerEnv.destroySE = destroySE;
    }
}
