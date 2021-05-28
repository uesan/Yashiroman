using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsimiPod : MobileEnemy
{
    [SerializeField]
    private float xMin;
    [SerializeField]
    private float xMax;
    [SerializeField]
    private Direction direction = Direction.Left;

    private float speed = 2f;
    private Player player;
    private float offsetY = 0.5f;
    private float floatError = 0.1f;
    const int MAX_HP = 2;

    private void Start()
    {
        Init();
        hp = MAX_HP;
        isInvisible = true;
    }

    private void FixedUpdate()
    {
        // 弾オブジェクトの移動関数
        EnemyMove();
    }

    void EnemyMove()
    {
        if (isActive)
        {
            if (player == null)
            {
                player = PlayerLifeBarView.GetInstance().player;
            }

            // 弾オブジェクトの移動量ベクトルを作成（数値情報）
            Vector2 enemyMovement = DesideMoveDirection();
            if (IsPlayerSameHeight())
            {
                rb2d.velocity = enemyMovement * speed * 3f;
            }
            else
            {
                // Rigidbody2D に移動量を加算する
                rb2d.velocity = enemyMovement * speed;
            }
        }
    }

    private bool IsPlayerSameHeight()
    {
        if(player.m_isGround && transform.position.y + offsetY - floatError < player.transform.position.y && player.transform.position.y < transform.position.y + offsetY + floatError)
        {
            return true;
        }
        return false;
    }

    private Vector2 DesideMoveDirection()
    {
        Vector2 vector = new Vector2();

        if(transform.position.x < xMin)
        {
            direction = Direction.Right;
        }
        else if(xMax < transform.position.x)
        {
            direction = Direction.Left;
        }

        if(direction == Direction.Left)
        {
            vector = new Vector2(-1, 0).normalized;
        }
        else
        {
            vector = new Vector2(1, 0).normalized;
        }
        return vector;
    }

    public override void Restart()
    {
        isInvisible = true;
        hp = MAX_HP;
        MobileEnemyRestartImp();
    }
}
