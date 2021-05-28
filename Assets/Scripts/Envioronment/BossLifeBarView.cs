using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using Cysharp.Threading.Tasks;

public class BossLifeBarView
{

    private int total_Damage = 0;

    public Slider life_Bar;
    public Boss boss { get; set; }

    public void InitBossLifeBarView()
    {
        total_Damage = 25;
        DisplayLifeBar();
        life_Bar.gameObject.SetActive(false);
    }

    public void RestartBossLifeBarView()
    {
        total_Damage = 25;
        DisplayLifeBar();
        life_Bar.gameObject.SetActive(false);
    }

    public void Damaged(int damage)
    {
        if (boss.isInvisible)
            return;
        total_Damage += damage;
        if (total_Damage > Boss.max_hp)
        {
            total_Damage = Boss.max_hp;
        }
        DisplayLifeBar();
        boss.StartDamaged(damage).Forget();
    }

    public void Healing(int heal_amount)
    {
        total_Damage -= heal_amount;
        if (total_Damage < 0)
        {
            total_Damage = 0;
        }
        GeneralEnvioronment.singleton.PlayHealingSE();
        DisplayLifeBar();
    }

    private void DisplayLifeBar()
    {
        life_Bar.value = total_Damage;
    }

    public async UniTask OnEnteredBossArea()
    {
        life_Bar.gameObject.SetActive(true);
        while(total_Damage > 0)
        {
            await UniTask.WaitForFixedUpdate();
            await UniTask.WaitForFixedUpdate();
            Healing(1);
        }
    }
}
