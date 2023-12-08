using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueReact
{
    //public GameObject SpeakerImg;
    public string SpeakerName;
    [TextArea(3, 10)]
    public string Line;
    public AudioClip Audio;
}
public class React : MonoBehaviour
{
    private bool m_Reacted = false;

    public bool m_OneTimeOnly = true;

    public TextMeshProUGUI m_TextDisplay;

    public GameObject m_ReactionCanvas, m_Player;

    public DialogueReact[] m_Speakers;

    private AudioSource m_AudioS;

    public void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").gameObject;
        m_AudioS = m_Player.GetComponent<AudioSource>();

    }
    public void MainReact()
    {
        if (!m_Reacted)
        {
            StartCoroutine(Reaction());
        }
    }
    IEnumerator Reaction()
    {
        m_Reacted = true;
        for (int i = 0; i < m_Speakers.Length; i++)
        {
            m_TextDisplay.text = m_Speakers[i].SpeakerName + ": " + m_Speakers[i].Line;
            m_ReactionCanvas.SetActive(true);
            //m_Player.transform.GetComponent<CharacterController>().ToTalking();

            if (m_Speakers[i].Audio != null)
            {
                m_AudioS.clip = m_Speakers[i].Audio;
                m_AudioS.Play();

                yield return new WaitForSeconds(m_Speakers[i].Audio.length);
            }
            else
            {
                yield return new WaitForSeconds(m_Speakers[i].Line.ToCharArray().Length * 0.1f);
            }
        }
        //m_ReactionCanvas.SetActive(false);
        //m_Player.transform.GetComponent<CharacterController>().ToMoving();
        Destroy(gameObject);
    }
}
