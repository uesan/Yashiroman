using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public abstract class Enemy : MonoBehaviour
{
    public bool isActive { get; set; }
    public int hp { get; set; } = 1;
    public int contactDamage { get; set; } = 2;
    public bool isInvisible { get; set; }
    protected Vector2 startPosition = new Vector2();

    protected bool isGround { get; set; } = true;


    protected CancellationToken cancellationToken;

    protected void Init()
    {
        cancellationToken = this.GetCancellationTokenOnDestroy();
        startPosition = transform.position;
        isInvisible = false;
        isActive = false;
        isGround = true;
        GameMaster.GetInstance().stageEnv.stageEnemies.Add(this);
    }

    protected void RestartImp()
    {
        transform.position = startPosition;
        isInvisible = false;
        isActive = false;
        isGround = true;
        gameObject.SetActive(true);
    }

    void OnBecameVisible()
    {
        isActive = true;
    }

    protected async UniTask Damaged(int damage)
    {
        hp -= damage;
        await AfterDamaged();

        if (hp <= 0)
        {
            GeneralEnvioronment.singleton.PlayEnemyDestroiedSE();
            gameObject.SetActive(false);
            isActive = false;
        }
    }

    /**
     * オーバーライドしたら、ダメージ時の処理できるよっていう。
     */
    protected virtual async UniTask AfterDamaged()
    {
        return;
    }

    protected virtual void OnGrounding()
    {
        return;
    }

    abstract public void Restart();


    // ENEMYと接触したときの関数
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        ContactOtherObject(collision);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        ContactOtherObject(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        int layerNumber = collision.gameObject.layer;
        if(layerNumber == (int)Layer.Ground)
        {
            isGround = false;
        }
    }

    protected void ContactOtherObject(Collision2D collision)
    {
        int layerNumber = collision.gameObject.layer;
        switch (layerNumber)
        {
            case (int)Layer.Bullet:
                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                if (isInvisible)
                {
                    Debug.Log("リフレクト！");
                    bullet.Refrected();
                    break;
                }
                Damaged(bullet.damage).Forget();
                bullet.Landing();
                GeneralEnvioronment.singleton.PlayEnemyDamagedSE();
                break;
            case (int)Layer.Ground:
                isGround = true;
                OnGrounding();
                break;
            default:
                break;
        }
    }

    protected void ContactOtherObject(Collider2D collision)
    {
        int layerNumber = collision.gameObject.layer;
        switch (layerNumber)
        {
            case (int)Layer.Bullet:
                Bullet bullet = collision.gameObject.GetComponent<Bullet>();
                if (isInvisible)
                {
                    Debug.Log("リフレクト！");
                    bullet.Refrected();
                    break;
                }
                Damaged(bullet.damage).Forget();
                bullet.Landing();
                GeneralEnvioronment.singleton.PlayEnemyDamagedSE();
                break;
            case (int)Layer.PlayerCollider:
                PlayerLifeBarView.GetInstance().Damaged(contactDamage);
                break;
            default:
                break;
        }
    }
}
