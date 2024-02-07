using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[System.Serializable]
public class Speaker
{
    // Clase para incluir la imagen del personaje que habla, el nombre, el diálogo en si y audio por si lo metemos en algun momento
    public string SpeakerName;
    public GameObject SpeakerImg;
    [TextArea(3, 10)]
    public string Line;
    public AudioClip Audio;
}

public class Dialogue : MonoBehaviour
{
    public bool m_Once = true, // Para que solo se pueda interactuar una vez
        m_CanMove = false, // Puede o no moverse mientras el diálogo esté en funcionamiento
        m_Follow = false, // Para que el diálogo siga funcionando aunque el personaje se mueva, si no cuando se aleja el diálogo se cierra
        m_AutomaticDialogue = false; // Determina si el diálogo salta automático o el jugador debe darle a la tecla de interacción

    public TextMeshProUGUI m_TextDisplay, m_NameDisplay;

    public GameObject m_DialogueMenu, m_Player;

    public Speaker[] m_Speakers;

    private int m_Index = 0;
    private float m_TypeSpeed = 0.02f;

    private AudioSource m_AudioS;

    public GameObject ItemShineVFX; // Efecto visual de brillo para que se vea que hay posibilidad de interactuar

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
        ItemShineVFX.SetActive(false);

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
        m_AutomaticDialogue = false;
    }
    IEnumerator Type()
    {
        // Corrutina para que escriba el texto en el cuadro caracter por caracter
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

                m_AudioS.Stop();

                if (m_Once)
                {
                    m_DialogueMenu.SetActive(false);
                    GetComponent<InteractionType>().enabled = false;
                    GetComponent<BoxCollider>().enabled = false;
                    GetComponent<Dialogue>().enabled = false;

                    //CutConversation();
                    //Destroy(gameObject,0.5f);
                }
                else ItemShineVFX.SetActive(true);

                if (!m_CanMove) m_CanMove = true;
            }
        }
    }
    public void CutConversation()
    {
        // Para cuando se aleje de la zona en la que el diálogo debe estar activo
        m_TextDisplay.text = "";
        m_AudioS.Stop();
        m_DialogueMenu.SetActive(false);
    }
}
