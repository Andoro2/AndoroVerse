using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWave : MonoBehaviour
{
    public GameObject m_DialogueMenu;

    private bool m_Flag = true;

    public Animator m_ArmAnimation;
    public AudioClip m_AudioClip;
    private AudioSource AS;

    // Start is called before the first frame update
    void Start()
    {
        AS = GetComponent<AudioSource>();
        AS.clip = m_AudioClip;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DialogueMenu.activeSelf && m_Flag)
        {
            StartCoroutine("RoboticAudio");
            m_ArmAnimation.SetTrigger("Saludo");
            m_Flag = false;
        }
        else if (!m_DialogueMenu.activeSelf && !m_Flag)
        {
            m_Flag = true;
        }
    }
    private IEnumerator RoboticAudio()
    {
        AS.Play();
        yield return new WaitForSeconds(m_AudioClip.length);
        AS.Play();
    }
}
