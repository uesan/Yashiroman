using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gimick : MonoBehaviour
{
    public bool isActive { get; set; }

    protected void Init()
    {
        GameMaster.GetInstance().stageEnv.stageGimicks.Add(this);
    }

    protected void RestartImp()
    {
        isActive = false;
        gameObject.SetActive(true);
    }

    abstract public void Restart();
}
