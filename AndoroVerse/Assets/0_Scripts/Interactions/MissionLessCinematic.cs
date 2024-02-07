using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.Linq;

public class MissionLessCinematic : MonoBehaviour
{
    public bool m_OneTimeOnly = false;
    public float m_VidDuration;
    private GameObject m_InGameUI;
    public GameObject m_CowdoroCinematicController;

    public Transform m_NewPosition;
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
            GameObject[] m_Dialogues = GameObject.FindGameObjectsWithTag("TextBox");
            if (m_Dialogues.Any(TextBox => TextBox.activeSelf))
            {
                GameObject ActiveTextBox = m_Dialogues.FirstOrDefault(TextBox => TextBox.activeSelf);
            }

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

        if(m_Player == null) m_Player = GameObject.FindWithTag("Player").gameObject;
        m_Player.GetComponent<CharacterController>().enabled = false;

        yield return new WaitForSeconds(m_VidDuration / 2);

        if (m_NewPosition != null)
        {
            m_Player.transform.position = m_NewPosition.position;
        }

        yield return new WaitForSeconds(m_VidDuration / 2);

        m_Player.GetComponent<CharacterController>().enabled = true;

        m_VidPlayer.Stop();
        m_VidPlayer.enabled = false;
        if(m_InGameUI == null) m_InGameUI = GameObject.FindWithTag("UI").gameObject.transform.Find("InGameUI").gameObject;
        m_InGameUI.SetActive(true);

        if (m_OneTimeOnly) Destroy(this.gameObject);

        if (m_CowdoroCinematicController != null) m_CowdoroCinematicController.GetComponent<CowDoroInteractionController>().m_CinematicInteraction = true;
    }
}
