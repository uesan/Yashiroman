using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

enum Action
{
    Intro,
    BigJumpAndShot,
    Action2,
    None,
}

public class Ponpiman : Boss
{
    bool isJumping;
    bool isDesent;
    bool isGrounding;
    private BoxCollider2D m_boxcollier2D;
    private Vector2 jumpVelocity = new Vector2(7f, 18.0f);
    private Vector2 reverseX = new Vector2(-1.0f, 1.0f);
    private Action actionName;
    private int shotTimes = 0;

    private void Start()
    {
        base.InitBoss();
        standLocate.Add(new Vector2(-4.5f, 0f));
        m_boxcollier2D = gameObject.GetComponent<BoxCollider2D>();
        bossName = BossName.Ponpiman;
        actionName = Action.None;
    }

    protected override void RestartBoss()
    {
        rb.velocity = Vector2.zero;
        isGround = true;
        isJumping = false;
        isDesent = false;
        isGrounding = false;
        actionName = Action.None;
    }

    private async UniTask Jump()
    {
        while (isJumping)
        {
            if (stopFlag)
                return;
            if (isGround)
            {
                if(bossDirection == Direction.Left)
                {
                    rb.velocity = jumpVelocity * reverseX;
                }
                else
                {
                    rb.velocity = jumpVelocity;
                }
                isGround = false;
                isDesent = false;
                isGrounding = false;
            }
            else
            {
                rb.velocity -= gravity;
                if(rb.velocity.y < 0f)
                {
                    isDesent = true;
                }
                if (offset.y > gameObject.transform.position.y && isDesent)
                {
                    rb.velocity = Vector2.zero;
                    isGround = true;
                    isJumping = false;
                    isDesent = false;
                    isGrounding = true;
                    gameObject.transform.position = new Vector2(gameObject.transform.position.x, offset.y);
                }
            }
            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
    }

    private async UniTask ActionChanger()
    {
        while (hp > 0)
        {
            if (stopFlag)
                return;
            switch (actionName)
            {
                case Action.None:
                    actionName = Action.BigJumpAndShot;
                    break;
                case Action.BigJumpAndShot:
                    await BigJumpAndShot();
                    break;
                case Action.Intro:
                    break;
                default:
                    break;
            }
            await UniTask.WaitForFixedUpdate(cancellationToken);
        }
    }

    private async UniTask BigJumpAndShot()
    {
        isJumping = true;
        await Jump();
        if (stopFlag)
            return;
        ChangeDirection();
        shotTimes = 0;
        await ShotAction();
    }

    private void ChangeDirection()
    {
        Vector3 scale = gameObject.transform.localScale;

        switch (bossDirection)
        {
            case Direction.Left:
                bossDirection = Direction.Right;
                scale.x = -1; // 反転（右向き）
                break;
            case Direction.Right:
                bossDirection = Direction.Left;
                scale.x = 1; //　そのまま（左向き）
                break;
            default:
                break;
        }

        transform.localScale = scale;
    }

    protected override void StartBattle()
    {
        offset = gameObject.transform.position;
        stopFlag = false;
        ActionChanger().Forget();
    }

    protected override async UniTask ShotAction()
    {
        while (shotTimes < 3)
        {
            if (stopFlag)
                return;
            const int shotCoolDown = 300;
            await UniTask.Delay(shotCoolDown);

            Vector3 vector = transform.position;
            Vector2 bulletDirection = new Vector2(1.0f, 1.0f);

            if (bossDirection == Direction.Left)
            {
                vector.x = transform.position.x - m_boxcollier2D.size.x * 0.6f;
                bulletDirection.x = -1.0f;
            }
            else
            {
                vector.x = transform.position.x + m_boxcollier2D.size.x * 0.6f;
                bulletDirection.x = 1.0f;
            }

            GameObject bulletObject = Instantiate(bossBullet, vector, transform.rotation);
            bulletObject.GetComponent<Bullet>().bulletDirection = bulletDirection;

            shotTimes++;
        }
        actionName = Action.None;
        shotTimes = 0;
    }
}
