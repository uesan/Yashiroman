using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class GeneralEnvioronment
{
    public static GeneralEnvioronment singleton { get; } = new GeneralEnvioronment();

    private GeneralEnvioronment()
    {

    }
    
    public AudioSource audioSource { get; set; }
    public AudioClip healingSE { get; set; }
    public AudioClip enemyDamagedSE { get; set; }
    public AudioClip enemyDestroiedSE { get; set; }
    public AudioClip doorSE { get; set; }
    public AudioClip vanishingfloorSE { get; set; }
    public AudioClip normalBossBGMIntro { get; set; }
    public AudioClip normalBossBGMLoop { get; set; }
    public AudioClip stageSelectBGMIntro { get; set; }
    public AudioClip stageSelectBGMLoop { get; set; }

    public void PlaySE(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, 1.0f);
    }

    public void PlayBGMNoLoop(AudioClip audioClip)
    {
        audioSource.loop = false;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlayBGMLoop(AudioClip audioClip)
    {
        audioSource.loop = true;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public async UniTask PlayBGMIntroLoop(AudioClip intro, AudioClip loop)
    {
        PlayBGMNoLoop(intro);
        float startTime = Time.time;

        while (Time.time - startTime < 100000)
        {
            await UniTask.WaitForFixedUpdate();
            if (!audioSource.isPlaying)
                break;
        }
        PlayBGMLoop(loop);
        Debug.Log("音がなっているはず");
    }

    public void PlayHealingSE()
    {
        PlaySE(healingSE);
    }

    public void PlayEnemyDamagedSE()
    {
        PlaySE(enemyDamagedSE);
    }

    public void PlayEnemyDestroiedSE()
    {
        PlaySE(enemyDestroiedSE);
    }

    public void PlayDoorSE()
    {
        PlaySE(doorSE);
    }

    public void PlayVasnishFloorSE()
    {
        PlaySE(vanishingfloorSE);
    }

    public void PlayNormalBossBGM()
    {
        PlayBGMIntroLoop(normalBossBGMIntro, normalBossBGMLoop).Forget();
    }
}
