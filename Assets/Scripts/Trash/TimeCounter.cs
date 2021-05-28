using System.Collections;
using UniRx;
using UnityEngine;
using System; // .NET 4.xモードで動かす場合は必須

//参考にしたサイト
// https://qiita.com/toRisouP/items/2f1643e344c741dd94f8

/// <summary>
/// 100からカウントダウンし、Debug.Logにその値を表示するサンプル
/// </summary>
public class TimeCounter : MonoBehaviour
{
    //イベントを発行する核となるインスタンス
    private Subject<int> timerSubject = new Subject<int>();

    //イベントの購読側だけを公開
    public IObservable<int> OnTimeChanged
    {
        get { return timerSubject; }
    }

    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        //100からカウントダウン
        var time = 100;
        while (time > 0)
        {
            time--;

            //イベントを発行
            timerSubject.OnNext(time);

            //1秒待つ
            yield return new WaitForSeconds(1);
        }
    }
}