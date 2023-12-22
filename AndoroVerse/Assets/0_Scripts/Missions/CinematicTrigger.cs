using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CinematicTrigger : MonoBehaviour
{
    //public int m_MissionIndex;
    public float m_VidDuration;
    private GameObject m_InGameUI;

    GameProgress GP;

    public VideoPlayer m_VidPlayer;
    public VideoClip m_Cinematic;

    public Transform m_NewPosition;
    private GameObject m_Player;

    public bool m_CheckpointAdvance = false, m_MissionAdvance = false, m_NextScene = false;
    public int m_SceneIndex;

    void Start()
    {
        m_InGameUI = GameObject.FindWithTag("UI").gameObject.transform.GetChild(0).gameObject;
        GP = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();
        m_Player = GameObject.FindWithTag("Player").gameObject;
        m_VidPlayer.playOnAwake = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_VidPlayer.clip = m_Cinematic;
            m_VidPlayer.Play();
            m_VidPlayer.time = 0f;
            StartCoroutine(FinDeCinemtaica());
        }
    }
    private IEnumerator FinDeCinemtaica()
    {
        m_VidPlayer.enabled = true;
        if(m_InGameUI != null) m_InGameUI.SetActive(false);

        //CharacterController CC = m_Player.GetComponent<CharacterController>();

        if (m_Player.GetComponent<CharacterController>().enabled)
        {
            m_Player.GetComponent<CharacterController>().enabled = false;
        }

        yield return new WaitForSeconds(m_VidDuration / 2f);

        if(m_NewPosition != null)
        {
            m_Player.transform.position = m_NewPosition.position;
        }

        yield return new WaitForSeconds(m_VidDuration / 2f);

        if (!m_Player.GetComponent<CharacterController>().enabled)
        {
            m_Player.GetComponent<CharacterController>().enabled = true;
        }
        
        m_VidPlayer.Stop();
        m_VidPlayer.enabled = false;
        m_InGameUI.SetActive(true);

        if(m_CheckpointAdvance) GP.AdvanceCheckpoint();
        if (m_MissionAdvance) GP.AdvanceMission();
        if (m_NextScene)SceneManager.LoadScene(m_SceneIndex);
        Destroy(gameObject);
    }
}