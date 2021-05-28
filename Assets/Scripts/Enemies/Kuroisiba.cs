using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kuroisiba : MobileEnemy
{
    // オブジェクトの移動係数（速度調整用）
    public Vector2 force { get; set; } = new Vector2(-250, 0);
    public float speed { get; set; } = 6.0f;
    private bool isMoving;
    private const int MAX_HP = 3;

    private void Start()
    {
        hp = MAX_HP;
        isMoving = false;
        Init();
    }
    private void FixedUpdate()
    {
        // 弾オブジェクトの移動関数
        EnemyMove();
    }

    // 弾オブジェクトの移動関数
    void EnemyMove()
    {
        if (!isMoving && isActive)
        {
            rb2d.AddForce(force);
            // 弾オブジェクトの移動量ベクトルを作成（数値情報）
            //Vector2 enemyMovement = new Vector2(-1, 0).normalized;
            // Rigidbody2D に移動量を加算する
            //rb2d.velocity = enemyMovement * speed;
            isMoving = true;
        }

    }

    public override void Restart()
    {
        hp = MAX_HP;
        isMoving = false;
        MobileEnemyRestartImp();
    }
}
