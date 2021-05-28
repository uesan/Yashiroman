using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PonpimaBullet : Bullet
{

    private string PlayerString = Layer.Player.ToString();
    private const int DAMAGE = 3;
    new private const float bulletSpeed = 7.0f;
    private float launchBaseAngle = 0.2f;

    private void Start()
    {
        base.Init();
        BulletMove();
    }

    new void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ContactOtherObject(collision);
    }

    void ContactOtherObject(Collider2D collision)
    {
        int layerNumber = collision.gameObject.layer;
        switch (layerNumber)
        {
            case (int)Layer.Player:
                PlayerLifeBarView.GetInstance().Damaged(DAMAGE);
                break;
            case (int)Layer.PlayerCollider:
                PlayerLifeBarView.GetInstance().Damaged(DAMAGE);
                break;
            case (int)Layer.Bullet:
                collision.gameObject.GetComponent<Bullet>().Landing();
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }

    new void BulletMove()
    {
        // 弾オブジェクトの移動量ベクトルを作成（数値情報）
        float rand = Random.value;
        Vector2 bulletMovement = new Vector2(rand, 1-rand + launchBaseAngle).normalized;
        // Rigidbody2D に移動量を加算する
        rb2d.velocity = bulletMovement * bulletDirection * bulletSpeed;
    }
}
