using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class TenKey : Object
{
    private int inputedNumber;
    private const int maxDigits = 7,maxButtons = 12;
    private int maxNumber;
    private Text numberText;

    /// <summary>
    /// 関数と、0~9までのボタンとを関連付けている。
    /// </summary>
    /// <param name="buttons"></param>
    /// <param name="buttonTexts"></param>
    public TenKey(List<Button> buttons, List<Text> buttonTexts, Text forNumberText)
    {
        if(buttons.Count != maxButtons || buttonTexts.Count != maxButtons)
        {
            Debug.Log("引数のListの要素が合っていません");
            Debug.Log("buttons.count:" + buttons.Count + ", buttonsTexts.Count:" + buttonTexts.Count);
            return;
        }
        inputedNumber = 0;

        maxNumber = 1;
        for(int index = 0;index < maxDigits; index++)
        {
            maxNumber *= 10; 
        }
        maxNumber--;
        Debug.Log(maxNumber);

        int i;
        int zero = 0;
        for (i = 0;i < 9; i++)
        {
            int number;
            number = i + 1;
            buttons[i].onClick.AddListener(() => { InputNumber(number); });
            buttonTexts[i].text = number.ToString();
        }
        buttons[i].onClick.AddListener(() => { ClearNumber(zero); });
        buttonTexts[i++].text = "C";
        buttons[i].onClick.AddListener(() => { InputNumber(zero); });
        buttonTexts[i++].text = "0";
        buttons[i].onClick.AddListener(() => { DeleteNumber(zero); });
        buttonTexts[i++].text = "１文字消す";
        Assert.AreEqual(12, i);
        numberText = forNumberText;
        UpdateText();
    }

    public void InputNumber(int number)
    {

        inputedNumber *= 10;
        if (inputedNumber > maxNumber)
        {
            inputedNumber /= 10;
            return;
        }

        inputedNumber += number;
        UpdateText();
    }

    public void DeleteNumber(int i)
    {
        if (inputedNumber == 0)
        {
            return;
        }
        inputedNumber /= 10;
        UpdateText();
    }

    private void UpdateText()
    {
        numberText.text = inputedNumber.ToString();
    }

    public void ClearNumber(int i)
    {
        inputedNumber = 0;
        UpdateText();
        Debug.Log("クリア");
    }

    void ShowLog()
    {
        Debug.Log("Hello UnityAction");
    }

    public int Output()
    {
        return inputedNumber;
    }
}
