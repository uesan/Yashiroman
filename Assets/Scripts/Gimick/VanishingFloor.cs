using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
using Cysharp.Threading.Tasks;

public class VanishingFloor : Gimick
{
    [SerializeField]
    private List<Vector3Int> floorsPosition;
    [SerializeField]
    private TileBase floatFloorTile;
    [SerializeField]
    private Tilemap fieldTileMap;

    private bool isActivated;
    private bool isStop;

    private const int APPEAR_DURATION = 1500;

    private void Start()
    {
        Init();
        isStop = false;
    }

    public override void Restart()
    {
        RestartImp();
        isStop = false;
    }

    private void OnBecameVisible()
    {
        if (!isActive)
        {
            isActive = true;
            Activate().Forget();
        }
    }

    private void OnBecameInvisible()
    {
        if(transform.position.x < PlayerLifeBarView.GetInstance().player.transform.position.x)
        {
            isStop = true;
        }
    }

    public async UniTask Activate()
    {
        while (!isStop)
        {
            foreach(Vector3Int position in floorsPosition)
            {
                SetTile(position, 2).Forget();
                await UniTask.Delay(APPEAR_DURATION);
                if (isStop)
                {
                    break;
                }
            }
        }

    }

    private async UniTask SetTile(Vector3Int position, int appearTimes)
    {
        GeneralEnvioronment.singleton.PlayVasnishFloorSE();
        fieldTileMap.SetTile(position, floatFloorTile);
        for (int i = 0; i < appearTimes; i++)
        {
            await UniTask.Delay(APPEAR_DURATION);
        }
        DeleteTile(position);
    }

    private void DeleteTile(Vector3Int position)
    {
        if (fieldTileMap.HasTile(position))
        {
            fieldTileMap.SetTile(position, null);
        }
    }
}
