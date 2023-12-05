using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenClose : MonoBehaviour
{
    [SerializeField] private Animator m_DoorAnim;

    [SerializeField] private bool Opened = false;
    //[SerializeField] private string m_OpenAnimation, m_CloseAnimation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("NPC") && !Opened)
        {
            m_DoorAnim.SetTrigger("Open");
            Opened = true;
            //m_DoorAnim.Play(m_OpenAnimation);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        m_DoorAnim.SetTrigger("Close");
        Opened = false;
        //m_DoorAnim.Play(m_CloseAnimation);
    }
}
