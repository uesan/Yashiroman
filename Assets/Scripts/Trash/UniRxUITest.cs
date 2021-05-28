using System;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class UniRxUITest : MonoBehaviour
{
    // UIとの紐付けのため
    [SerializeField] private Button button;
    [SerializeField] private InputField inputField;
    [SerializeField] private Text countText;
    [SerializeField] private Text greetText;

    // カウントを記録する
    private int count = 0;

    // ここが重要！！
    // SubjectはIObservableとIObserverの2つを実装しており，
    // 「値を発行する」「値を購読できる」という2つの機能を持ったクラスである
    private Subject<string> _onClickButton = new Subject<string>();

    public IObservable<string> OnClickButton
    {
        get { return _onClickButton; }
    }

    void Start()
    {
        countText.text = count.ToString();
        Setup();
    }

    private void Setup()
    {
        // buttonのイベントが発行されたときにカウントアップmethodが走る
        // OnClickAsObservableはUniRxのプラグインのなかで定義されている
        // 自分で定義を追加することもできる(別の記事書いたらやってみます)
        button.OnClickAsObservable()
            // クリックイベントの最後に呼ばれるのがSubscribe
            // ラムダ式で記入している．_が引数で(今回はないので仮引数の_)呼ばれるのがCountUp()
            // Addto()はいつこのイベントを破棄するかを決めている．詳しくは別記事か参考リンクから
            .Subscribe(_ => CountUp())
            .AddTo(this);

        //インプットフィールドの入力が終わったらイベント発火
        inputField.OnEndEditAsObservable()
            .Subscribe(text => _onClickButton.OnNext(text))
            .AddTo(this);

        // 作成した _onClickButtonの値が変わったらイベント発火
        OnClickButton
            .Subscribe(text => Greet(text))
            .AddTo(this);
    }

    private void CountUp()
    {
        count++;
        countText.text = count.ToString();
    }

    private void Greet(string name)
    {
        greetText.text = "  おはようございます、" + name + "さん";
    }
}