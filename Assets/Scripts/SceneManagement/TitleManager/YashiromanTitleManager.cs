using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YashiromanTitleManager : GenericTitleManager
{
    public void GameStart()
    {
        GameMaster.GetInstance().ChangeSceneToStage(StageNameEnum.PonpimanStage);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            GameStart();
        }
    }
}
