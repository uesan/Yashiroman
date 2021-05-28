using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Right,
    Left,
    Down,
    Stop
}

public enum Wepon
{
    Takoyaki,
    Normal,
    Unkown
}

public class Player : MonoBehaviour
{
    public float speed { get; set; } = 5f;
    public float speedInDamaged { get; set; } = 2f;
    public float flap { get; set; } = 1010f;

    public Rigidbody2D rb;
    public BoxCollider2D m_boxcollier2D;
    public LayerMask whatIsGround;

    public StageEnvioronment stageEnvioronment { get; set; }

    public int hp { get; set; }
    public const int max_hp = 26;
    public Collider2D playerCollider { get; set; }
    public Wepon currentWepon { get; set; } = Wepon.Normal;
    private float prebHoriKey = 0;
    public bool m_isGround { get; set; } = false;
    private float lastJumpTime = 0f;
    private Direction playerDirection = Direction.Right;
    private const float m_centerY = 1f;
    private PlayerLifeBarView lifebarView = PlayerLifeBarView.GetInstance();
    private float startDamageTime = 0f;
    private const float DamageEffectDuration = 0.8f;
    private const int OneBlinkingEffectDuration = 360;
    private bool isPausing = false;
    public bool isInputable { get; set; }
    public bool isInvisible { get; set; }
    private CancellationToken cancellationToken;
    private PlayerEnvioronment playerEnv = PlayerEnvioronment.singleton;
    private float lastShotTime;
    private const float FIRST_JUMP_FORCE_RATIO = 0.33f;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject bullet;

    void Update()
    {
        if (isInputable)
        {
            Pause();
            if (!isPausing)
            {

                MoveAction();
                JumpAction();
                ShotAction();
            }
        }
    }

    // 物理演算をしたい場合はFixedUpdateを使うのが一般的
    void FixedUpdate()
    {
        /*
        if (isInputable)
        {
            MoveAction();
            JumpAction();
            ShotAction();
        }*/

        JudgeEndingShootAnimation();
        CheckGrounded();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layerNumber = collision.gameObject.layer;
        switch (layerNumber)
        {
            case (int)Layer.CameraRange:
                stageEnvioronment.stageView.ChangeCameraRange(collision.gameObject, gameObject).Forget();
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        cancellationToken = this.GetCancellationTokenOnDestroy();
        lifebarView.player = this;
        transform.position = GameMaster.GetInstance().stageEnv.SavePoints[stageEnvioronment.currentSavepoint];
        InitPlayer();
    }

    private void Restart()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        try
        {
            transform.position = GameMaster.GetInstance().stageEnv.SavePoints[stageEnvioronment.currentSavepoint];
        }
        catch
        {
            Debug.Log("指定されたインデックスのセーブポイントが存在しません。");
        }
        sprite.color = new Color(1f, 1f, 1f, 0f);
        gameObject.SetActive(true);
        RestartPlayer();
        GameMaster.GetInstance().stageView.Restart();
        sprite.color = new Color(1f, 1f, 1f, 1f);
    }

    private void Pause()
    {
        if(Input.GetKeyDown("p"))
        {
            if (!isPausing)
            {
                Time.timeScale = 0f;
                isPausing = true;
            }
            else
            {
                Time.timeScale = 1f;
                isPausing = false;
            }
        }
    }

    private void InitPlayer()
    {
        hp = max_hp;
        isInputable = true;
        isInvisible = false;
        lastShotTime = 0;
        lifebarView.InitPlayerLifeBarView();
    }

    private void RestartPlayer()
    {
        hp = max_hp;
        isInputable = true;
        isInvisible = false;
        lifebarView.RestartPlayerLifeBarView();
    }

    private void JudgeEndingShootAnimation()
    {
        const float shotAnimeReverb = 1.0f;
        if(Time.time - lastShotTime > shotAnimeReverb)
        {
            animator.SetBool("isShooting", false);
        }
    }

    private void CheckGrounded()
    {
        Vector2 pos = transform.position;
        Vector2 groundCheck = new Vector2(pos.x, pos.y - (m_centerY * transform.localScale.y));
        Vector2 groundArea = new Vector2(m_boxcollier2D.size.x * 0.49f, 0.05f);

        m_isGround = Physics2D.OverlapArea(groundCheck - groundArea, groundCheck + groundArea, whatIsGround);
        animator.SetBool("isGround", m_isGround);
    }

    public async UniTaskVoid StartDamaged(int damage)
    {
        animator.SetBool("isDamaged", true);
        hp -= damage;
        if(hp <= 0)
        {
            isInputable = false;
            gameObject.SetActive(false);
            await UniTask.Delay(1000);
            Restart();
            return;
        }
        startDamageTime = Time.time;
        playerEnv.PlayDamageSE();
        DoringDamaged().Forget();
    }

    private async UniTask DoringDamaged()
    {
        ColliderVisible(false);
        isInputable = false;
        while (Time.time - startDamageTime < DamageEffectDuration)
        {
            /* アニメーションの終了時間の管理。
             */
            if (playerDirection == Direction.Right)
            {
                rb.velocity = new Vector2(-speedInDamaged, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(speedInDamaged, rb.velocity.y);
            }
            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
        isInputable = true;
        animator.SetBool("isDamaged", false);

        const int blinkTimes = 3;
        for (int i = 0; i < blinkTimes; i++)
        {
            SpriteRenderer sprite =  GetComponent<SpriteRenderer>();
            sprite.color = new Color(1f, 1f, 1f, 0f);
            await UniTask.Delay(OneBlinkingEffectDuration / 2);
            sprite.color = new Color(1f, 1f, 1f, 1f);
            await UniTask.Delay(OneBlinkingEffectDuration / 2);
        }
        ColliderVisible(true);
    }

    private void ColliderVisible(bool isVisible)
    {
        if (!isVisible)
        {
            playerCollider.enabled = false;
        }
        else
        {
            playerCollider.enabled = true;
        }
        isInvisible = !isVisible;
    }

    // 弾オブジェクトを生成して飛ばす関数
    void ShotAction()
    {
        if (Input.GetButtonDown("Fire1") && PlayerBullet.shots < PlayerBullet.MAX_SHOTS)
        {
            Vector3 vector = transform.position;
            Vector2 bulletDirection = new Vector2(1.0f, 1.0f);
            const float STAND_SHOT_HEIGHT_OFFSET = -0.3f;
            const float JUMP_SHOT_HEIGHT_OFFSET = 0f;
            float shotHeight = 0.5f;

            if (m_isGround)
            {
                shotHeight = STAND_SHOT_HEIGHT_OFFSET;
            }
            else
            {
                shotHeight = JUMP_SHOT_HEIGHT_OFFSET;
            }
            vector.y = transform.position.y + shotHeight;

            if (playerDirection == Direction.Left)
            {
                vector.x = transform.position.x - m_boxcollier2D.size.x * 0.7f;
                bulletDirection.x = -1.0f;
            }
            else
            {
                vector.x = transform.position.x + m_boxcollier2D.size.x * 0.7f;
                bulletDirection.x = 1.0f;
            }

            GameObject bulletObject = Instantiate(bullet, vector, transform.rotation);
            bulletObject.GetComponent<PlayerBullet>().bulletDirection = bulletDirection;

            lastShotTime = Time.time;
            animator.SetBool("isShooting", true);
            playerEnv.PlayShotSE();
        }
    }



    private void MoveAction()
    {
        float horizontalKey = Input.GetAxis("Horizontal");

        //右入力で右向きに動く
        if (horizontalKey > 0 && (horizontalKey > prebHoriKey || horizontalKey >= 0.99))
        {
            PlayerMove(Direction.Right, speed);
        }
        //左入力で左向きに動く
        else if (horizontalKey < 0 && (horizontalKey <= prebHoriKey || horizontalKey <= -0.99))
        {
            PlayerMove(Direction.Left, speed);
        }
        //ボタンを離すと止まる
        else
        {
            PlayerMove(Direction.Stop, speed);
        }

        prebHoriKey = horizontalKey;
    }

    private void PlayerMove(Direction direction,float movementSpeed)
    {
        Vector3 scale = transform.localScale;
        animator.SetBool("IsWalking", true);

        switch (direction)
        {
            case Direction.Left:
                rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);
                playerDirection = Direction.Left;
                scale.x = -1; // 反転する（左向き）
                break;
            case Direction.Right:
                rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
                playerDirection = Direction.Right;
                scale.x = 1; // そのまま（右向き）
                break;
            case Direction.Stop:
                animator.SetBool("IsWalking", false);
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;
            default:
                break;
        }
        transform.localScale = scale;
    }

    public async UniTask OnDoorEntered()
    {
        const float moveDistance = 3.2f;
        Vector2 enteringPosition = gameObject.transform.position;
        const int waitTimeForDoorOpen = 500;
        PlayerMove(Direction.Stop, 0);

        ForcePlayable(false);

        await UniTask.Delay(waitTimeForDoorOpen);

        while(gameObject.transform.position.x - enteringPosition.x < moveDistance)
        {
            PlayerMove(Direction.Right, speed / 2.5f);
            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
        PlayerMove(Direction.Stop, 0);
    }

    public async UniTask OnVisibleRangeEntered()
    {
        const float moveDistance = 2.2f;
        Vector2 enteringPosition = gameObject.transform.position;
        PlayerMove(Direction.Stop, 0);

        ForcePlayable(false);

        while (gameObject.transform.position.x - enteringPosition.x < moveDistance)
        {
            PlayerMove(Direction.Right, speed / 3f);
            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
        PlayerMove(Direction.Stop, 0);
    }

    public void ForcePlayable(bool isPlayable)
    {
        if (isPlayable)
        {
            isInputable = true;
            isInvisible = false;
        }
        else
        {
            isInputable = false;
            isInvisible = true;
        }

    }

    public void OnStageCleared()
    {
        ForcePlayable(false);
        PlayerMove(Direction.Stop, 0);
    }

    private void JumpAction()
    {
        const float jumpCD = 0.1f;
        if (Input.GetKeyDown("space") && m_isGround && !(Time.time - lastJumpTime < jumpCD))
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * (flap * FIRST_JUMP_FORCE_RATIO));
            lastJumpTime = Time.time;
            ExtendJump().Forget();
        }
    }

    private async UniTask ExtendJump()
    {
        const int reseptionFlame = 12;

        for (int i = 0; i < reseptionFlame; i++)
        {
            if (Input.GetKey("space"))
            {
                rb.AddForce(Vector2.up * flap / reseptionFlame  * (1.0f - FIRST_JUMP_FORCE_RATIO));
            }
            else
            {
                return;
            }
            await UniTask.DelayFrame(1);
        }
    }
}