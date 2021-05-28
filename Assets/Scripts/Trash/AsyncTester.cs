using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;

public class AsyncTester : MonoBehaviour
{
    private void Start()
    {
        _ = Hoge();
        Debug.Log("先に実行される");
    }

    private async UniTaskVoid Hoge()
    {
        // 1秒待つ
        await UniTask.Delay(1000);

        // 1フレーム待つ
        await UniTask.DelayFrame(1);

        // FixedUpdateで1フレーム待つ
        await UniTask.DelayFrame(1, PlayerLoopTiming.FixedUpdate);

        // 1フレーム待ってUpdate()のタイミングまで待機
        await UniTask.Yield();

        // 次のFixedUpdate()のタイミングまで待機
        await UniTask.Yield(PlayerLoopTiming.FixedUpdate);

        var flag = true;

        // 条件がtrueになるまで待つ
        await UniTask.WaitUntil(() => flag);

        flag = false;

        // 条件がfalseになるまで待つ
        await UniTask.WaitWhile(() => flag);

        // Awakeが終わるまで待つ
        await gameObject.AwakeAsync();

        // Startが終わるまで待つ
        await gameObject.StartAsync();

        Debug.Log("いっぱい待っているので後で実行される");
    }
}
