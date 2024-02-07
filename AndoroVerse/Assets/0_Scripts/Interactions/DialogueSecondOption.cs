using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[System.Serializable]
public class TextLine
{
    // Clase para incluir la imagen del personaje que habla, el nombre, el diálogo en si y audio por si lo metemos en algun momento
    public string SpeakerName;
    public GameObject SpeakerImg;
    [TextArea(3, 10)]
    public string Line;
}

public class DialogueSecondOption : MonoBehaviour
{
    public bool m_Once = true, // Para que solo se pueda interactuar una vez
          m_CanMove = false, // Puede o no moverse mientras el diálogo esté en funcionamiento
          m_Follow = false, // Para que el diálogo siga funcionando aunque el personaje se mueva, si no cuando se aleja el diálogo se cierra
          m_AutomaticDialogue = false; // Determina si el diálogo salta automático o el jugador debe darle a la tecla de interacción

    public TextMeshProUGUI m_TextDisplay, m_NameDisplay;

    public GameObject m_DialogueFrame, m_Player;

    public int m_Index = 0;

    public TextLine[] m_TextLines;

    GameProgress GP;

    private GameObject ItemShineVFX; // Efecto visual de brillo para que se vea que hay posibilidad de interactuar

    public void Start()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").gameObject;
        GP = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();
        ItemShineVFX = transform.Find("ItemShineVFX").gameObject;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            InteractionManager();
        }
    }
    public void InteractionManager()
    {
        if (ItemShineVFX.activeSelf) ItemShineVFX.SetActive(false);
        if (m_Index == 0) // Primera interacción
        {
            Transform CharPics = transform.Find("DialogueCanvas").transform.Find("CharPic").transform;
            for (int i = 0; i < CharPics.childCount; i++)
            {
                Transform CharPic = CharPics.GetChild(i);
                CharPic.gameObject.SetActive(false);
            }
            

            m_DialogueFrame.SetActive(true);
            m_NameDisplay.text = m_TextLines[0].SpeakerName;
            m_TextDisplay.text = m_TextLines[0].Line;
            m_TextLines[0].SpeakerImg.SetActive(true);

            m_Index++;
        }
        else
        {
            if (m_Index < m_TextLines.Length)
            {
                m_TextLines[m_Index - 1].SpeakerImg.SetActive(false);
                m_NameDisplay.text = m_TextLines[m_Index].SpeakerName;
                m_TextDisplay.text = m_TextLines[m_Index].Line;
                m_TextLines[m_Index].SpeakerImg.SetActive(true);
                m_Index++;
            }
            else  // final de las lineas de texto
            {
                m_DialogueFrame.SetActive(false);

                if (m_Once)
                {
                    GetComponent<DialogueSecondOption>().enabled = false;
                    GetComponent<Collider>().enabled = false;
                    if (m_Follow) Destroy(gameObject, 0.2f);
                }

                else if (ItemShineVFX.activeSelf) ItemShineVFX.SetActive(true);
            }
        }
    }
    public void CutConversation()
    {
        m_Index = 0;
        m_DialogueFrame.SetActive(false);
        ItemShineVFX.SetActive(true);
    }
}
