using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Observer
{
    public string SpeakerName;
    [TextArea(3, 10)]
    public string Line;
    public AudioClip Audio;
}
public class Observe : MonoBehaviour
{
    public bool m_OneTimeOnly = true, m_CanMove = false, m_DialogueOnce = false;

    private bool m_Interacted = false;

    public GameObject m_ObserveCanvas, m_DialogueCanvas, m_Player;

    public TextMeshProUGUI m_DescriptionDisplay, m_DialogueDisplay;

    [TextArea(2, 5)]
    public string m_Description;

    public Observer[] m_Observers;

    private AudioSource m_AudioS;

    public void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").gameObject;
        m_AudioS = m_Player.GetComponent<AudioSource>();
    }
    public void Update()
    {
        if (m_DialogueCanvas == null && m_ObserveCanvas == null) Destroy(gameObject);
    }

    public void InteractionManager()
    {
        if (!m_ObserveCanvas.activeSelf)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    public void Open()
    {
        m_ObserveCanvas.SetActive(true);
        //m_Player.transform.GetComponent<CharacterController>().ToTalking();

        m_DescriptionDisplay.text = m_Description;

        if (m_OneTimeOnly) m_Interacted = true;

        if(m_DialogueCanvas != null) StartCoroutine(Observation());

    }
    IEnumerator Observation()
    {
        m_DialogueCanvas.SetActive(true);
        for (int i = 0; i < m_Observers.Length; i++)
        {
            m_DialogueDisplay.text = m_Observers[i].SpeakerName + ": " + m_Observers[i].Line;

            if (m_Observers[i].Audio != null)
            {
                m_AudioS.clip = m_Observers[i].Audio;
                m_AudioS.Play();

                yield return new WaitForSeconds(m_Observers[i].Audio.length);
            }
            else
            {
                yield return new WaitForSeconds(m_Observers[i].Line.ToCharArray().Length * 0.1f);
            }
        }
        //m_ReactionCanvas.SetActive(false);

        if (m_DialogueOnce) Destroy(m_DialogueCanvas);
        else
        {
            m_DialogueCanvas.SetActive(false);
            //m_Player.transform.GetComponent<CharacterController>().ToMoving();
        }
        //Close();
    }
    public void Close()
    {
        //StopCoroutine(Observation());

        if (!m_CanMove) m_CanMove = true;

        if (m_OneTimeOnly) Destroy(m_ObserveCanvas);
        else m_ObserveCanvas.SetActive(false);
    }
}
