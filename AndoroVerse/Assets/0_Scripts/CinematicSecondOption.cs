using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CinematicSecondOption : MonoBehaviour
{
    public GameObject gameChanger, m_NewPosition;

    private GameObject m_InGameUI, m_Player;

    GameProgress GP;

    public bool m_CheckpointAdvance = false, m_MissionAdvance = false, m_NextScene = false;
    public int m_SceneIndex;

    private bool m_Moved = false;
    void Start()
    {
        GameObject GameUI = GameObject.FindWithTag("UI").gameObject;
        m_InGameUI = GameUI.transform.Find("InGameUI").transform.gameObject;

        GP = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();
        m_Player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        /*if (gameChanger == null)
        {
            if(m_NewPosition != null && !m_Moved) m_Player.transform.position = m_NewPosition.transform.position; m_Moved = true;

            if (!m_InGameUI.activeSelf) m_InGameUI.SetActive(true);
            if (m_CheckpointAdvance) GP.AdvanceCheckpoint();
            if (m_MissionAdvance) GP.AdvanceMission();
            if (m_NextScene) SceneManager.LoadScene(m_SceneIndex);

            if (!m_Player.GetComponent<CharacterController>().enabled)
            {
                m_Player.GetComponent<CharacterController>().enabled = true;
            }
            if (!m_InGameUI.activeSelf) m_InGameUI.SetActive(true);
        }*/
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameChanger != null)
        {
            gameChanger.SetActive(true);

            VidDestroy VD = gameChanger.GetComponent<VidDestroy>();
            if (m_CheckpointAdvance) VD.m_CheckpointAdvance = true;
            if (m_MissionAdvance) VD.m_MissionAdvance = true;
            if (m_NextScene) VD.m_NextScene = true; VD.m_SceneIndex = m_SceneIndex;
            if (m_NewPosition != null) VD.m_NewPosition = m_NewPosition;

            GameObject GameUI = GameObject.FindWithTag("UI").gameObject;
            m_InGameUI = GameUI.transform.Find("InGameUI").transform.gameObject;

            if (m_InGameUI.activeSelf) m_InGameUI.SetActive(false);

            m_Player = GameObject.FindWithTag("Player");
            if (m_Player.GetComponent<CharacterController>().enabled)
            {
                m_Player.GetComponent<CharacterController>().enabled = false;
            }
            gameObject.SetActive(false);
        }
    }
}
