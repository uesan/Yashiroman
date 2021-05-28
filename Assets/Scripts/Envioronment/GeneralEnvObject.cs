using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEnvObject : MonoBehaviour
{
    public AudioClip healingSE;
    public AudioClip enemyDamagedSE;
    public AudioClip enemyDestroiedSE;
    public AudioClip doorSE;
    public AudioClip vanishingfloorSE;
    public AudioClip normalBossBGMIntro;
    public AudioClip normalBossBGMLoop;
    public AudioClip stageSelectBGMIntro;
    public AudioClip stageSelectBGMLoop;

    private GeneralEnvioronment generalEnvioronment = GeneralEnvioronment.singleton;

    private void Start()
    {
        generalEnvioronment.healingSE = healingSE;
        generalEnvioronment.enemyDamagedSE = enemyDamagedSE;
        generalEnvioronment.enemyDestroiedSE = enemyDestroiedSE;
        generalEnvioronment.doorSE = doorSE;
        generalEnvioronment.vanishingfloorSE = vanishingfloorSE;
        generalEnvioronment.normalBossBGMIntro = normalBossBGMIntro;
        generalEnvioronment.normalBossBGMLoop = normalBossBGMLoop;
        generalEnvioronment.stageSelectBGMIntro = stageSelectBGMIntro;
        generalEnvioronment.stageSelectBGMLoop = stageSelectBGMLoop;
    }
}
