using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWave : MonoBehaviour
{
    public GameObject m_DialogueMenu;

    public bool m_Flag = true;

    public Animator m_ArmAnimation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_DialogueMenu.activeSelf && m_Flag)
        {
            m_ArmAnimation.SetTrigger("Saludo");
            m_Flag = false;
        }
        else if (!m_DialogueMenu.activeSelf && !m_Flag)
        {
            m_Flag = true;
        }
    }
}
