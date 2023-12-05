using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    public float m_DeathTime = 3f, m_ExtraTime = 0.25f;

    public AudioClip m_SoundFX;

    void Start()
    {
        if(m_SoundFX != null)
        {
            AudioSource AS = GetComponent<AudioSource>();
            AS.clip = m_SoundFX;
            AS.Play();

            m_DeathTime = m_SoundFX.length + m_ExtraTime;
        }

        Destroy(gameObject, m_DeathTime);
    }
}
