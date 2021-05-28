using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : GenericTitleManager
{
    public GameObject inputSeedPanel;
    public GameObject ReadTagsPanel;
    public List<Button> tenKeyButton = new List<Button>();
    public List<Text> buttonTexts = new List<Text>();
    private TenKey tenKey;
    public Text inputedNumberText, seedText;

    private void Start()
    {
        tenKey = new TenKey(tenKeyButton, buttonTexts, inputedNumberText);
    }

    public void ShowReadTagsPanel()
    {
        title.SetActive(false);
        StopBGM();
        ReadTagsPanel.SetActive(true);
    }

    new public void BackToTitle()
    {
        base.BackToTitle();
        ReadTagsPanel.SetActive(false);
    }

    new public void ShowPreGamePanel()
    {
        inputSeedPanel.SetActive(false);
        base.ShowPreGamePanel();
    }

    public void ShowInputSeedPanel()
    {
        inputSeedPanel.SetActive(true);
        preGamePanel.SetActive(false);
    }
}
