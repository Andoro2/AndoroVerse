using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameProgress : MonoBehaviour
{
    private TextMeshProUGUI m_MissionName, m_MissionDescription, m_MissionObjective;

    public int m_CheckPointIndex;

    public Mission m_ActiveMission;

    public int m_MissionIndex;
    public Mission[] m_Missions;

    void Start()
    {
        m_ActiveMission = m_Missions[m_MissionIndex];
    }

    void Update()
    {
        FindUI();

        m_ActiveMission = m_Missions[m_MissionIndex];

        m_MissionName.text = m_ActiveMission.m_MissionName;
        m_MissionObjective.text = m_ActiveMission.m_MissionObjective;
        m_MissionDescription.text = m_ActiveMission.m_MissionDescription;
    }
    public void FindUI()
    {
        GameObject GameUI = GameObject.FindWithTag("UI");

        m_MissionObjective = GameUI.transform.Find("InGameUI").Find("MissionObjective").GetComponent<TextMeshProUGUI>();

        GameObject MissionInfo = GameUI.transform.Find("PauseMenu").Find("MissionInfo").gameObject;
        m_MissionName = MissionInfo.transform.Find("MissionTitle").GetComponent<TextMeshProUGUI>();
        m_MissionDescription = MissionInfo.transform.Find("MissionInfo").GetComponent<TextMeshProUGUI>();
    }
    public void AdvanceCheckpoint()
    {
        m_CheckPointIndex++;
    }
    public void AdvanceMission()
    {
        m_MissionIndex++;
    }
}
[System.Serializable]
public class Mission
{
    public string m_MissionName;
    public string m_MissionObjective;
    [TextArea(3, 10)]
    public string m_MissionDescription;
}