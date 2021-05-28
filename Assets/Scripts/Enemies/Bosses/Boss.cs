using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public enum BossName
{
    Ponpiman,
    None,
}

public abstract class Boss : Enemy
{
    public const int max_hp = 26;
    protected const int appearancePositionHeight = 14;

    protected Rigidbody2D rb;
    protected bool isGrounded = false;
    protected Vector2 gravity = new Vector2(0, 0.5f);
    protected List<Vector2> standLocate = new List<Vector2>();
    protected Vector2 offset = new Vector2();
    protected Direction bossDirection;
    protected bool stopFlag = false;

    public StageEnvioronment stageEnv { get; set; }

    /**
     * bossName
     * ボスの名前が入ります。Start関数で各ボスの名前を入力してください。
     */
    protected BossName bossName = BossName.None;

    [SerializeField]
    protected GameObject bossBullet;

    // Start is called before the first frame update
    void Start()
    {
        InitBoss();
    }

    /*
    new protected void OnCollisionEnter2D(Collision2D collision)
    {
        ContactOtherObject(collision);
    }
    */

    private void OnTriggerStay2D(Collider2D collision)
    {
        ContactOtherObject(collision);
    }

    /*
    new protected void ContactOtherObject(Collision2D collision)
    {
        int layerNumber = collision.gameObject.layer;
        switch (layerNumber)
        {
            case (int)Layer.Bullet:
                DesideDamageAmount();
                break;
            default:
                break;
        }
    }
    */

    new protected void ContactOtherObject(Collider2D collision)
    {
        int layerNumber = collision.gameObject.layer;
        switch (layerNumber)
        {
            case (int)Layer.Bullet:
                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                if (isInvisible)
                {
                    bullet.Refrected();
                    break;
                }
                DesideDamageAmount();
                bullet.Landing();
                break;
            case (int)Layer.PlayerCollider:
                PlayerLifeBarView.GetInstance().Damaged(contactDamage);
                break;
            default:
                break;
        }
    }

    protected void DesideDamageAmount()
    {
        switch (stageEnv.player.currentWepon)
        {
            case Wepon.Normal:
                stageEnv.bossLifeBarView.Damaged(1);
                break;
            default:
                break;
        }
    }

    protected bool JudgeDestroied()
    {
        if (hp <= 0)
        {
            return true;
        }
        return false;
    }

    public async UniTask StartDamaged(int damage)
    {
        hp -= damage;

        if (JudgeDestroied())
        {
            WhenDefeated();
            gameObject.SetActive(false);
            return;
        }

        const int blinkTimes = 2;
        const int BlinkEffectDuration = 360;

        isInvisible = true;

        for (int i = 0; i < blinkTimes; i++)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(1f, 1f, 1f, 0f);
            await UniTask.Delay(BlinkEffectDuration / 2);
            sprite.color = new Color(1f, 1f, 1f, 1f);
            await UniTask.Delay(BlinkEffectDuration / 2);
        }

        isInvisible = false;
    }

    protected void WhenDefeated()
    {
        GameMaster master = GameMaster.GetInstance();
        if (master.gameState == GameState.NormalStage)
        {
            _= master.StageClear();
        }
        PlayerEnvioronment.singleton.PlayDestroySE();
    }

    protected void InitBoss()
    {
        base.Init();
        rb = GetComponent<Rigidbody2D>();
        Restart();
    }

    public async UniTask OnEnteredBossArea()
    {
        GeneralEnvioronment.singleton.PlayNormalBossBGM();
        while(gameObject.transform.position.y > startPosition.y)
        {
            rb.velocity -= gravity;
            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
        gameObject.transform.position = startPosition;
        rb.velocity = Vector2.zero;
        await stageEnv.bossLifeBarView.OnEnteredBossArea();
        isInvisible = false;
        StartBattle();
    }

    public override void Restart()
    {
        hp = max_hp;
        isInvisible = true;
        rb.velocity = Vector2.zero;
        isGrounded = false;
        bossDirection = Direction.Left;
        Vector3 scale = transform.localScale;
        scale.x = 1; //　そのまま（左向き）
        transform.localScale = scale;

        gameObject.transform.position = startPosition + new Vector2(0, appearancePositionHeight);
        isActive = false;
        stopFlag = true;
        RestartBoss();
        Debug.Log("ボスのRestart");

        stageEnv.bossLifeBarView.RestartBossLifeBarView();
    }

    abstract protected void RestartBoss();
    abstract protected void StartBattle();
    abstract protected UniTask ShotAction();
}
