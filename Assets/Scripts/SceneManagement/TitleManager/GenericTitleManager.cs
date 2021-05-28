using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GenericTitleManager : MonoBehaviour
{
    const string titleScene = "Title";
    const string gameScene = "GameImp";
    public GameObject tutorialPanel, creditPanel, title, preGamePanel;

    public AudioClip bgm;
    public AudioSource source;
    public float bgmVolume;

    // Start is called before the first frame update
    void Start()
    {
        BackToTitle();
        StartBGM();
    }
    public void GoGameScene()
    {
        ChangeScene(gameScene);
    }

    public void GoTitleScene()
    {
        ChangeScene(titleScene);
    }

    public void StartBGM()
    {
        source.volume = bgmVolume;
        source.clip = bgm;
        source.Play();
    }

    public void StopBGM()
    {
        source.Stop();
    }

    private void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        title.SetActive(false);
    }

    public void ShowCredit()
    {
        creditPanel.SetActive(true);
        title.SetActive(false);
    }

    public void ShowPreGamePanel()
    {
        preGamePanel.SetActive(true);
        title.SetActive(false);
    }

    public void BackToTitle()
    {
        creditPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        preGamePanel.SetActive(false);
        title.SetActive(true);
    }
}