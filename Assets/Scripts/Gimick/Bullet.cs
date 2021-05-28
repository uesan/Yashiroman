using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 弾オブジェクトのRigidbody2Dの入れ物
    protected Rigidbody2D rb2d;
    protected Renderer bulletRenderer;
    // 弾オブジェクトの移動係数（速度調整用）
    public Vector2 bulletDirection { get; set; } = new Vector2(1.0f, 1.0f);
    public float bulletSpeed { get; set; } = 20.0f;
    public int damage { get; set; } = 1;

    void Start()
    {
        Init();
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }

    protected void Init()
    {
        // オブジェクトのRigidbody2Dを取得
        rb2d = GetComponent<Rigidbody2D>();
        bulletRenderer = GetComponent<Renderer>();
        BulletMove();
    }
    // 弾オブジェクトの移動関数
    protected void BulletMove()
    {
        // 弾オブジェクトの移動量ベクトルを作成（数値情報）
        Vector2 bulletMovement = new Vector2(1, 0).normalized;
        // Rigidbody2D に移動量を加算する
        rb2d.velocity = bulletMovement * bulletDirection * bulletSpeed;

    }

    public void Refrected()
    {
        // 弾オブジェクトの移動量ベクトルを作成（数値情報）
        Vector2 bulletMovement;
        if(bulletDirection.x > 0)
            bulletMovement = new Vector2(-1, 1).normalized;
        else
            bulletMovement = new Vector2(1, 1).normalized;
        // Rigidbody2D に移動量を加算する
        rb2d.velocity = bulletMovement * bulletSpeed;
        gameObject.GetComponent<Collider2D>().enabled = false;
    }

    public void Landing()
    {
        // なにかに弾が接触したら弾は消滅する
        Destroy(gameObject);
    }

    // ENEMYと接触したときの関数
    protected void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // なにかに弾が接触したら弾は消滅する
        ContactOtherObject(collision);
    }

    void ContactOtherObject(Collider2D collision)
    {
        int layerNumber = collision.gameObject.layer;
        switch (layerNumber)
        {
            case (int)Layer.Player:
                PlayerLifeBarView.GetInstance().Damaged(damage);
                break;
            case (int)Layer.PlayerCollider:
                PlayerLifeBarView.GetInstance().Damaged(damage);
                break;
            default:
                break;
        }
    }
}