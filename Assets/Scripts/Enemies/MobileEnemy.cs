using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobileEnemy : Enemy
{

    // 敵オブジェクトのRigidbody2Dの入れ物
    public Rigidbody2D rb2d { get; set; }

    new protected void Init()
    {
        base.Init();
        rb2d = GetComponent<Rigidbody2D>();
    }

    protected void MobileEnemyRestartImp()
    {
        isActive = false;
        rb2d.velocity = Vector2.zero;
        RestartImp();
    }
}
