using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEnvioronment : MonoBehaviour
{
    public Slider lifebar;
    public Slider weponEnergy;
    public Slider bossLifebar;
    public Collider2D playerCollider;
    public StageView stageView;
    public Player player;
    public Boss stageBoss;
    public List<Vector2> SavePoints = new List<Vector2>();


    public List<Enemy> stageEnemies { get; set; } = new List<Enemy>();
    public List<Gimick> stageGimicks { get; set; } = new List<Gimick>();
    public PlayerLifeBarView playerEnvioronment { get; set; } = PlayerLifeBarView.GetInstance();
    public BossLifeBarView bossLifeBarView { get; set; } = new BossLifeBarView();
    public int currentSavepoint;

    private void Awake()
    {
        playerEnvioronment.life_Bar = lifebar;
        player.playerCollider = playerCollider;
        stageView.stageEnvioronment = this;
        player.stageEnvioronment = this;
        stageBoss.stageEnv = this;
        bossLifeBarView.life_Bar = bossLifebar;
        bossLifeBarView.boss = stageBoss;
        bossLifeBarView.InitBossLifeBarView();
        GameMaster.GetInstance().stageEnv = this;
        currentSavepoint = 0;
    }
}
