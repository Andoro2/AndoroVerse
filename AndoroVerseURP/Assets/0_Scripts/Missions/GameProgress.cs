using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameProgress : MonoBehaviour
{
    public TextMeshProUGUI m_MissionName, m_MissionDescription, m_MissionObjective;

    public int m_CheckPointIndex;

    public Mission m_ActiveMission;

    public int m_MissionIndex;
    public Mission[] m_Missions;

    //public CheckPoint[] m_CheckPoints;

    void Start()
    {
        m_ActiveMission = m_Missions[0];
        //m_ActiveMission.MissionState = Mission.MissionStates.Active;
    }

    void Update()
    {
        m_ActiveMission = m_Missions[m_MissionIndex];

        m_MissionName.text = m_ActiveMission.m_MissionName;
        m_MissionObjective.text = m_ActiveMission.m_MissionObjective;
        m_MissionDescription.text = m_ActiveMission.m_MissionDescription;
    }

    public void AdvanceCheckpoint()
    {
        m_CheckPointIndex++;
    }
    public void AdvanceMission()
    {
        //m_ActiveMission.MissionState = Mission.MissionStates.Completed;
        m_MissionIndex++;
        //m_Missions[m_MissionIndex].MissionState = Mission.MissionStates.Active;
    }
}
/*
[System.Serializable]
public class CheckPoint
{
    public GameObject m_RevivePoint;
}*/

[System.Serializable]
public class Mission
{
    public string m_MissionName;
    //public enum MissionStates { Holding, Active, Completed }
    //[SerializeField] public MissionStates MissionState = MissionStates.Holding;
    public string m_MissionObjective;
    [TextArea(3, 10)]
    public string m_MissionDescription;
}
