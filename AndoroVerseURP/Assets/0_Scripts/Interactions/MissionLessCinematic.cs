using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MissionLessCinematic : MonoBehaviour
{
    public bool m_OneTimeOnly = false;
    public float m_VidDuration;
    private GameObject m_InGameUI;
    public GameObject m_CowdoroCinematicController;

    public VideoPlayer m_VidPlayer;
    public VideoClip m_Cinematic;
    private GameObject m_Player;

    void Start()
    {
        m_InGameUI = GameObject.FindWithTag("UI").gameObject.transform.GetChild(0).gameObject;

        m_Player = GameObject.FindWithTag("Player").gameObject;
        m_VidPlayer.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
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
        if (m_InGameUI != null) m_InGameUI.SetActive(false);

        m_Player.GetComponent<CharacterController>().enabled = false;

        yield return new WaitForSeconds(m_VidDuration);

        m_Player.GetComponent<CharacterController>().enabled = true;

        m_VidPlayer.Stop();
        m_VidPlayer.enabled = false;
        m_InGameUI.SetActive(true);

        if (m_OneTimeOnly) gameObject.SetActive(false);

        if (m_CowdoroCinematicController != null) m_CowdoroCinematicController.GetComponent<CowDoroInteractionController>().m_CinematicInteraction = true;
    }
}
