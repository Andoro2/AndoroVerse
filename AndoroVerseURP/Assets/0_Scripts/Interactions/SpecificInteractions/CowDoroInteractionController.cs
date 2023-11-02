using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CowDoroInteractionController : MonoBehaviour
{
    //public float m_VidDuration;
    //private GameObject m_InGameUI;

    public GameObject m_CowDoro;

    //public VideoPlayer m_VidPlayer;
    //public VideoClip m_Cinematic;
    //private GameObject m_Player;

    public bool m_CinematicInteraction = false;

    void Start()
    {
        //m_InGameUI = GameObject.FindWithTag("UI").gameObject.transform.GetChild(0).gameObject;

        //m_Player = GameObject.FindWithTag("Player").gameObject;
        //m_VidPlayer.playOnAwake = false;
    }

    void Update()
    {
        if (m_CinematicInteraction)
        {
            //diálogo
            m_CowDoro.transform.GetChild(0).gameObject.SetActive(true);
            //cinemática
            //m_CowDoro.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            //diálogo
            m_CowDoro.transform.GetChild(0).gameObject.SetActive(false);
            //cinemática
            m_CowDoro.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}
