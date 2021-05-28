using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sebasupiyo : MobileEnemy
{
    // オブジェクトの移動係数（速度調整用）
    public float speed { get; set; } = 7.0f;

    private bool isMoving;

    private void Start()
    {
        hp = 1;
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
            // 弾オブジェクトの移動量ベクトルを作成（数値情報）
            Vector2 enemyMovement = new Vector2(-3, -1).normalized;
            // Rigidbody2D に移動量を加算する
            rb2d.velocity = enemyMovement * speed;
            isMoving = true;
        }

    }

    public override void Restart()
    {
        hp = 1;
        isMoving = false;
        MobileEnemyRestartImp();
    }
}
