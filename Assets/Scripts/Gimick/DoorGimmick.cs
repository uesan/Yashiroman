using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Threading;
using Cysharp.Threading.Tasks;

public class DoorGimmick : Gimick
{
    public TileBase doorTile;
    public Tilemap doorTileMap;
    public Transform upperLeftTile;
    public bool isIntoBoss;
    public StageView stageView;

    private Vector3 upperLeftTile_position;

    private const int DoorTileNum = 4;
    private const int DoorOpenDuration = 400;

    private bool isOpenedOnce = false;

    void Start()
    {
        Init();
        upperLeftTile_position = upperLeftTile.position;
        if (isIntoBoss)
        {
            stageView.bossDoor = this;
        }
    }

    public override void Restart()
    {
        if (isIntoBoss)
        {
            gameObject.GetComponent<TilemapCollider2D>().isTrigger = true;
            isOpenedOnce = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int layerNumber = collision.gameObject.layer;

        if (layerNumber == (int)Layer.Player && !isOpenedOnce)
        {
            isOpenedOnce = true;
            EnterDoor().Forget();
        }
    }

    private async UniTask EnterDoor()
    {
        Player player = PlayerLifeBarView.GetInstance().player;
        OpenDoor(upperLeftTile_position).Forget();
        await player.OnDoorEntered();
        await CloseDoor(upperLeftTile_position);
        if (isIntoBoss)
        {
            await stageView.stageEnvioronment.stageBoss.OnEnteredBossArea();
        }
        gameObject.GetComponent<TilemapCollider2D>().isTrigger = false;
        player.ForcePlayable(true);
    }

    private async UniTask OpenDoor(Vector3 upperLeftTile_pos)
    {
        Vector3Int upperLeft = doorTileMap.WorldToCell(upperLeftTile_pos);
        Vector3Int grid1, grid2;

        GeneralEnvioronment.singleton.PlayDoorSE();

        for (int i = 0; i < DoorTileNum; i++)
        {
            grid1 = upperLeft + new Vector3Int(0, -(DoorTileNum - 1) + i, 0);
            grid2 = upperLeft + new Vector3Int(1, -(DoorTileNum - 1) + i, 0);
            DeleteTile(doorTileMap, grid1);
            DeleteTile(doorTileMap, grid2);
            await UniTask.Delay(DoorOpenDuration / DoorTileNum);
        }
    }

    private async UniTask CloseDoor(Vector3 upperLeftTile_pos)
    {
        Vector3Int upperLeft = doorTileMap.WorldToCell(upperLeftTile_pos);
        Vector3Int grid1, grid2;

        GeneralEnvioronment.singleton.PlayDoorSE();

        for (int i = 0; i < DoorTileNum; i++)
        {
            grid1 = upperLeft + new Vector3Int(0, -i, 0);
            grid2 = upperLeft + new Vector3Int(1, -i, 0);
            doorTileMap.SetTile(grid1, doorTile);
            doorTileMap.SetTile(grid2, doorTile);
            await UniTask.Delay(DoorOpenDuration / DoorTileNum);
        }
    }

    private void DeleteTile(Tilemap tilemap, Vector3Int grid)
    {
        if (tilemap.HasTile(grid))
        {
            tilemap.SetTile(grid, null);
        }
    }
}
