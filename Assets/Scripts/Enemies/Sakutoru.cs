using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class Sakutoru : Enemy
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Direction direction = Direction.Left;

    private Animator animator;
    private Player player;
    private bool isShooting;
    private const float BULLET_SPEED = 8f;

    const int MAX_HP = 3;
    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        hp = MAX_HP;
        isShooting = false;
        SetInvisible(true);
        Init();
    }

    public override void Restart()
    {
        isShooting = false;
        SetInvisible(true);
        hp = MAX_HP;
        RestartImp();
    }

    private async UniTask CheckActivate()
    {
        if (isShooting || !isActive)
            return;

        if (player == null)
        {
            player = PlayerLifeBarView.GetInstance().player;
        }
        while (Mathf.Abs(player.gameObject.transform.position.x - transform.position.x) < 5.0f)
        {
            await Activate();
        }

        SetInvisible(true);
        isShooting = false;
    }

    private async UniTask Activate()
    {
        const int WAIT_TIME = 1000;   //millisecond
        const int AFTER_WAIT_TIME = 1000;   //millisecond

        SetInvisible(false);
        isShooting = true;

        await UniTask.Delay(WAIT_TIME, cancellationToken: cancellationToken);
        ShotAction();
        await UniTask.Delay(AFTER_WAIT_TIME, cancellationToken: cancellationToken);
    }

    private void SetInvisible(bool isInvisible)
    {
        this.isInvisible = isInvisible;
        animator.SetBool("isVisible", !isInvisible);
    }

    private void ShotAction()
    {
        Vector2 bulletDirection = new Vector2(1.0f, 1.0f);
        Vector2 bulletPosition = transform.position;
        BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();

        if (!isActive)
            return;

        bulletPosition.y = transform.position.y - 1 + boxCollider2D.size.y * 0.5f;

        if (direction == Direction.Left)
        {
            bulletPosition.x = transform.position.x - boxCollider2D.size.x * 0.5f;
            bulletDirection.x = -1.0f;
        }
        else
        {
            bulletPosition.x = transform.position.x + boxCollider2D.size.x * 0.5f;
            bulletDirection.x = 1.0f;
        }
        GameObject bulletObject = Instantiate(bullet, bulletPosition, transform.rotation);
        Debug.Log(bulletPosition);
        Bullet bulletInstance = bulletObject.GetComponent<Bullet>();
        bulletInstance.bulletDirection = bulletDirection;
        bulletInstance.bulletSpeed = BULLET_SPEED;
        //playerEnv.PlayShotSE();

        return;
    }

    private void FixedUpdate()
    {
        CheckActivate().Forget();
    }
}
