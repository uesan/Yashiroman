using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class StageView : MonoBehaviour
{
    public Image displayMask;
    public AudioSource audioSource;
    public AudioClip stageBGM;
    public AudioClip clearBGM;
    public CinemachineConfiner cinemachineConfiner;
    public CompositeCollider2D visibleArea;
    public CompositeCollider2D bossArea;
    public CinemachineCameraOffset virtualCameraOffset;


    public StageEnvioronment stageEnvioronment { get; set; }
    public DoorGimmick bossDoor { get; set; }

    private float fadeSpeed = 0.015f;
    private float currentFadeDarkness = 0f;
    private const float littleBitLessThanOne = 0.99f;
    private CancellationToken cancellationToken;
    private float BGMvolume = 0.15f;
    private const float CameraSpeed = 0.2f;

    private bool isFirstCameraChange;

    private bool isFading = true;

    private void Start()
    {
        //(多分)オブジェクトがDestroyされた時に非同期処理を中断するためのトークン
        cancellationToken = this.GetCancellationTokenOnDestroy();

        // 非同期メソッド実行
        //StartFadeOut().Forget();

        GameMaster.GetInstance().stageView = this;

        //GeneralEnvシングルトンにオーディオソースを連携させる。
        //ステージ生成ごとに行わないと、SEがならない
        GeneralEnvioronment.singleton.audioSource = audioSource;

        audioSource.volume = BGMvolume;
        PlayStageBGM();
        isFirstCameraChange = true;
    }

    private void Update()
    {

    }

    private void RestartAllEnemies()
    {
        foreach(Enemy enemy in stageEnvioronment.stageEnemies)
        {
            enemy.Restart();
        }
    }

    private void RestartAllGimicks()
    {
        foreach(Gimick gimick in stageEnvioronment.stageGimicks)
        {
            gimick.Restart();
        }
    }

    public void Restart()
    {
        PlayStageBGM();
        isFirstCameraChange = true;
        RestartAllEnemies();
        RestartAllGimicks();
    }


    public async UniTask CameraMove(Direction direction)
    {
        float currentOffsetX = 0;
        float currentOffsetY = 0;
        float speedX = 0;
        float speedY = 0;

        Func<bool> isEndFunc = () => false;

        switch (direction)
        {
            case Direction.Up:
                return;
                speedY = CameraSpeed;
                isEndFunc = () => currentOffsetY > GameMaster.BlocksInColumn;
                break;
            case Direction.Down:
                speedY = -CameraSpeed;
                isEndFunc = () => currentOffsetY < -GameMaster.BlocksInColumn;
                break;
            case Direction.Left:
                speedX = -CameraSpeed;
                isEndFunc = () => currentOffsetX < -GameMaster.BlocksInRow;
                break;
            case Direction.Right:
                speedX = CameraSpeed;
                isEndFunc = () => currentOffsetX > GameMaster.BlocksInRow;
                break;
            default:
                break;
        }

        while (true)
        {
            await UniTask.DelayFrame(1);
            currentOffsetX += speedX;
            currentOffsetY += speedY;
            virtualCameraOffset.m_Offset = new Vector3(currentOffsetX, currentOffsetY, 0);
            if (isEndFunc())
            {
                virtualCameraOffset.m_Offset = new Vector3(0, 0, 0);
                break;
            }
        }

    }

    private void PlayStageBGM()
    {
        GeneralEnvioronment.singleton.PlayBGMLoop(stageBGM);
    }

    private async UniTask PlayClearBGM()
    {
        GeneralEnvioronment.singleton.PlayBGMNoLoop(clearBGM);
        float startTime = Time.time;
        while (Time.time - startTime < 20000)
        {
            await UniTask.WaitForFixedUpdate(cancellationToken);
            if (!audioSource.isPlaying)
                break;
        }
    }

    public async UniTask StageClear()
    {
        await PlayClearBGM();
        await UniTask.Delay(500);
        await StartFadeOut();
    }

    public async UniTask ChangeCameraRange(GameObject visibleRangeObject, GameObject player)
    {
        Debug.Log("ChangedCamera");

        CompositeCollider2D visibleCollider;
        VisibleRange visibleRange;
        bool isBossArea = false;
        try
        {
            visibleCollider = visibleRangeObject.GetComponent<CompositeCollider2D>();
        }
        catch
        {
            Debug.Log("CompositeCollider2Dがありません");
            return;
        }

        if (isFirstCameraChange)
        {
            isFirstCameraChange = false;
            cinemachineConfiner.m_BoundingShape2D = visibleCollider;
            return;
        }

        try
        {
            visibleRange = visibleRangeObject.GetComponent<VisibleRange>();
            if (visibleRange.isUpdateSavepoint)
            {
                stageEnvioronment.currentSavepoint++;
            }
            isBossArea = visibleRange.isBossVisibleRange;
        }
        catch
        {
            Debug.Log("visibleRangeがありません");
        }

        Direction direction;
        BoxCollider2D boxCollider = visibleRangeObject.GetComponent<BoxCollider2D>();
        if(visibleRangeObject.transform.position.x +  boxCollider.size.x * 0.5f < player.transform.position.x)
        {
            direction = Direction.Left;
        }
        else if (visibleRangeObject.transform.position.x - boxCollider.size.x * 0.5f > player.transform.position.x)
        {
            direction = Direction.Right;
        }
        else if (visibleRangeObject.transform.position.y + boxCollider.size.y * 0.5f < player.transform.position.y)
        {
            direction = Direction.Down;
        }
        else
        {
            direction = Direction.Up;
            return;
        }
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        float speedX = playerRigidbody.velocity.x;
        float speedY = playerRigidbody.velocity.y;
        float gravity = playerRigidbody.gravityScale;
        Player playerScript = player.GetComponent<Player>();
        playerRigidbody.velocity = Vector2.zero;
        playerRigidbody.gravityScale = 0f;
        playerScript.ForcePlayable(false);
        await CameraMove(direction);
        playerRigidbody.velocity = new Vector2(speedX, speedY);
        playerRigidbody.gravityScale = gravity;
        if (!isBossArea)
        {
            playerScript.ForcePlayable(true);
        }

        cinemachineConfiner.m_BoundingShape2D = visibleCollider;
    }

    // 非同期メソッド
    public async UniTask StartFadeOut()
    {
        float startTime = Time.time;

        while (isFading && startTime - Time.time < 10000)
        {
            // 次のFixedUpdateを待つ
            await UniTask.WaitForFixedUpdate(cancellationToken);
            //Debug.Log("OnFixedUpdate!");
            FadeOut();
        }
    }

    private void FadeOut()
    {
        if (isFading)
        {
            currentFadeDarkness += fadeSpeed;
            displayMask.color = new Color(0, 0, 0, currentFadeDarkness);
            if(currentFadeDarkness > littleBitLessThanOne)
            {
                isFading = false;
            }
        }
    }
}
