using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Speaker
{
    public string SpeakerName;
    public GameObject SpeakerImg;
    [TextArea(3, 10)]
    public string Line;
    public AudioClip Audio;
}

public class Dialogue : MonoBehaviour
{
    public bool m_OneTimeOnly = true, m_CanMove = false, m_Follow = false, m_AutomaticDialogue = false;

    public TextMeshProUGUI m_TextDisplay, m_NameDisplay;

    public GameObject m_DialogueMenu, m_Player;

    public Speaker[] m_Speakers;

    private int m_Index = 0;
    private float m_TypeSpeed = 0.02f;

    private AudioSource m_AudioS;

    public void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").gameObject;
        m_AudioS = m_Player.GetComponent<AudioSource>();
    }

    public void InteractionManager()
    {
        if (!m_DialogueMenu.activeSelf)
        {
            StartDialogue();
        }
        else
        {
            NextSentence();
        }
    }
    public void StartDialogue()
    {
        m_DialogueMenu.SetActive(true);
        //m_Player.transform.GetComponent<CharacterController>().ToTalking();
        m_Index = 0;
        m_NameDisplay.text = m_Speakers[m_Index].SpeakerName;
        m_TextDisplay.text = "";

        if (m_Speakers[0].Audio != null)
        {
            m_AudioS.clip = m_Speakers[0].Audio;
            m_AudioS.Play();
        }

        m_Speakers[m_Index].SpeakerImg.SetActive(true);

        StartCoroutine(Type());
    }
    IEnumerator Type()
    {
        foreach(char c in m_Speakers[m_Index].Line.ToCharArray())
        {
            if (m_TextDisplay.text != m_Speakers[m_Index].Line)
            {
                m_TextDisplay.text += c;
                yield return new WaitForSeconds(m_TypeSpeed);
            }
        }
    }
    public void NextSentence()
    {
        if (m_TextDisplay.text != m_Speakers[m_Index].Line)
        {
            StopCoroutine(Type());
            m_TextDisplay.text = m_Speakers[m_Index].Line;
        }
        else
        {
            if (m_Index < m_Speakers.Length - 1)
            {
                m_Speakers[m_Index].SpeakerImg.SetActive(false);
                m_Index++;
                if (m_Speakers[m_Index].Audio != null)
                {
                    m_AudioS.Stop();
                    m_AudioS.clip = m_Speakers[m_Index].Audio;
                    m_AudioS.Play();
                }
                m_Speakers[m_Index].SpeakerImg.SetActive(true);
                m_NameDisplay.text = m_Speakers[m_Index].SpeakerName;
                m_TextDisplay.text = "";
                StartCoroutine(Type());
            }
            else
            {
                m_TextDisplay.text = "";

                m_DialogueMenu.SetActive(false);
                //m_Player.transform.GetComponent<CharacterController>().ToMoving();

                m_AudioS.Stop();

                if (m_OneTimeOnly)
                {
                    m_DialogueMenu.SetActive(false);
                    GetComponent<InteractionType>().enabled = false;
                    GetComponent<BoxCollider>().enabled = false;
                    GetComponent<Dialogue>().enabled = false; //Destroy(gameObject);
                }

                if (!m_CanMove) m_CanMove = true;
            }
        }
    }
    public void CutConversation()
    {
        m_TextDisplay.text = "";
        m_AudioS.Stop();
        m_DialogueMenu.SetActive(false);
        //m_Player.transform.GetComponent<CharacterController>().ToMoving();
    }
}
