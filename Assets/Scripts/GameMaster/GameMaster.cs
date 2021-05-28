using System.Threading;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    Title,
    NormalStage,
    StageSelect,
    StageClear,
    StageResult
}

public enum StageNameEnum
{
    PonpimanStage,
    Other,
    Unknown,
}

public enum Layer
{
    Default,
    TransparentFX,
    IgnoreRaycast,
    //使っていないUserLayer = 3 がある。
    Whater = 4,
    UI,
    Ground,
    Enemy,
    Player,
    Bullet,
    Decoration,
    Gimick,
    EnemyBullet,
    PlayerCollider,
    CameraRange,
    EnemyBulletNoCancel
}

public class GameMaster
{
    private static GameMaster Singleton = new GameMaster();

    const string titleSceneName = "Title";
    const string stageClearSceneName = "StageClear";

    /***
     * 1block = position(1,1,0);
     */
    public const int BlocksInRow = 16;
    public const int BlocksInColumn = 14;
    public const int pixcelsInBlock = 16;

    public GameState gameState = GameState.NormalStage;
    Dictionary<StageNameEnum, string> stageSceneName = new Dictionary<StageNameEnum, string>();
    private StageNameEnum thisStage = StageNameEnum.Unknown;
    private List<StageNameEnum> clearedStage = new List<StageNameEnum>();

    public StageView stageView;
    public StageEnvioronment stageEnv;

    private GameMaster()
    {
        stageSceneName.Add(StageNameEnum.PonpimanStage, "PonpimanStage");
    }

    public static GameMaster GetInstance()
    {
        return Singleton;
    }

    public void ChangeSceneToStage(StageNameEnum stageName)
    {
        gameState = GameState.NormalStage;
        thisStage = stageName;
        SceneManager.LoadScene(stageSceneName[stageName]);
    }

    public void ChangeSceneToTitle()
    {
        gameState = GameState.Title;
        SceneManager.LoadScene(titleSceneName);
    }

    public async UniTask StageClear()
    {
        gameState = GameState.StageClear;
        clearedStage.Add(thisStage);
        Player player = PlayerLifeBarView.GetInstance().player;
        player.OnStageCleared();
        thisStage = StageNameEnum.Other;
        await stageView.StageClear();
        SceneManager.LoadScene(stageClearSceneName);
    }

    public void AddEnemy(Enemy enemy)
    {
        stageEnv.stageEnemies.Add(enemy);
    }
}
