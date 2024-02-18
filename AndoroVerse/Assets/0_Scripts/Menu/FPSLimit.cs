using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimit : MonoBehaviour
{
    public int m_FPSLimit = 60;//, CheckPointOnEnter, MissionIndexOnEnter;
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = m_FPSLimit;
    }
}
