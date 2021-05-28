using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeBarView
{
    private static PlayerLifeBarView singleton = new PlayerLifeBarView();

    private int total_Damage = 0;

    public Slider life_Bar;
    public Player player { get; set; }

    public static PlayerLifeBarView GetInstance()
    {
        return singleton;
    }

    private PlayerLifeBarView()
    {

    }

    public void InitPlayerLifeBarView()
    {
        total_Damage = 0;
        DisplayLifeBar();
    }

    public void RestartPlayerLifeBarView()
    {
        total_Damage = 0;
        DisplayLifeBar();
    }

    public void Damaged(int damage)
    {
        if (player.isInvisible)
            return;
        total_Damage += damage;
        if(total_Damage > Player.max_hp)
        {
            total_Damage = Player.max_hp;
        }
        DisplayLifeBar();
        _= player.StartDamaged(damage);
    }

    public void Healing(int heal_amount)
    {
        total_Damage -= heal_amount;
        if(total_Damage < 0)
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
}
