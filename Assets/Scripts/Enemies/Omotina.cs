using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

enum Size
{
    normal,
    midium,
    big,
    bomb,
}

public class Omotina : MobileEnemy
{
    public Sprite normal;
    public Sprite midium;
    public Sprite big;
    public Sprite bomb;
    [SerializeField]
    private Direction direction;

    private BoxCollider2D boxCollider2D;
    private SpriteRenderer spriteRenderer;
    private Vector2 normalOffset = new Vector2(0, -0.8f);
    private Vector2 normalSize = new Vector2(1.1f, 1.1f);
    private Vector2 midiumOffset = new Vector2(0, -0.6f);
    private Vector2 midiumSize = new Vector2(1.5f, 1.5f);
    private Vector2 bigOffset = new Vector2(0, -0.05f);
    private Vector2 bigSize = new Vector2(2.2f, 2.6f);
    private Vector2 jumpPowerVector = new Vector2(20, 250);
    private const int BOMB_DURATION = 200;//mili second
    private bool isJumping;

    private const int MAX_HP = 6;
    // Start is called before the first frame update
    void Start()
    {
        hp = MAX_HP;
        Init();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        ChangeSize(Size.normal).Forget();
        isJumping = false;
    }

    public override void Restart()
    {
        hp = MAX_HP;
        ChangeSize(Size.normal).Forget();
        MobileEnemyRestartImp();
        isJumping = false;
    }

    private void Update()
    {
        if(isGround && isActive)
        {
            Jump();
        }
    }

    private void Jump()
    {
        if (!isJumping)
        {
            if(direction == Direction.Left)
            {
                rb2d.AddForce(jumpPowerVector * new Vector2(-1, 1));
            }
            else
            {
                rb2d.AddForce(jumpPowerVector);
            }
            isJumping = true;
        }
    }

    private async UniTask ChangeSize(Size size)
    {
        switch (size)
        {
            case Size.normal:
                spriteRenderer.sprite = normal;
                boxCollider2D.offset = normalOffset;
                boxCollider2D.size = normalSize;
                break;
            case Size.midium:
                spriteRenderer.sprite = midium;
                boxCollider2D.offset = midiumOffset;
                boxCollider2D.size = midiumSize;
                break;
            case Size.big:
                spriteRenderer.sprite = big;
                boxCollider2D.offset = bigOffset;
                boxCollider2D.size = bigSize;
                break;
            case Size.bomb:
                spriteRenderer.sprite = bomb;
                await UniTask.Delay(BOMB_DURATION);
                break;
            default:
                break;
        }
    }

    protected override async UniTask AfterDamaged()
    {
        if (hp <= 0)
        {
            await ChangeSize(Size.bomb);
        }
        else if(hp <= 2)
        {
            ChangeSize(Size.big).Forget();
        }
        else if(hp <= 4)
        {
            ChangeSize(Size.midium).Forget();
        }
    }

    protected override void OnGrounding()
    {
        rb2d.velocity = Vector2.zero;
        isJumping = false;
    }
}
