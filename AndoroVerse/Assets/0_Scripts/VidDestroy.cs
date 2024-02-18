using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VidDestroy : MonoBehaviour
{
    public VideoPlayer m_VidPlayer;
    public GameObject m_NewPosition;
    private GameObject m_VidObject,
        m_InGameUI, m_Player, GameUI;

    CharacterController CC;
    GameProgress GP;

    public bool m_CheckpointAdvance = false, m_MissionAdvance = false, m_NextScene = false;
    public int m_SceneIndex;
    void Start()
    {
        GameUI = GameObject.FindWithTag("UI").gameObject;
        m_InGameUI = GameUI.transform.Find("InGameUI").transform.gameObject;

        GP = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();
        m_Player = GameObject.FindWithTag("Player");

        m_VidPlayer.Play();

        m_VidObject = m_VidPlayer.gameObject;
        m_VidPlayer.loopPointReached += OnVideoEnd;


    }
    private void OnEnable()
    {
        CC = GameObject.FindWithTag("Player").GetComponent<CharacterController>();
        CC.enabled = false;
        CC.OutOfPlay();
    }

    void OnVideoEnd(VideoPlayer VP)
    {
        if (m_NewPosition != null)
        { 
            Vector3 NewPos = m_NewPosition.transform.position;

            m_Player.transform.position = NewPos;
        }

        m_VidPlayer.Stop();
        if (!m_InGameUI.activeSelf) m_InGameUI.SetActive(true);
        if (m_CheckpointAdvance) GP.AdvanceCheckpoint();
        if (m_MissionAdvance) GP.AdvanceMission();
        if (m_NextScene)
        {
            GameUI.transform.Find("LoadingScreen").transform.gameObject.SetActive(true);
            SceneManager.LoadScene(m_SceneIndex);
        }

        if (!m_Player.GetComponent<CharacterController>().enabled)
        {
            m_Player.GetComponent<CharacterController>().enabled = true;
        }
        if (!m_InGameUI.activeSelf) m_InGameUI.SetActive(true);

        m_VidPlayer.clip = null;
        m_VidPlayer.enabled = false;

        CC.enabled = true;
        CC.BackToPlay();

        Destroy(m_VidObject);

        VP.loopPointReached -= OnVideoEnd;
    }
}
