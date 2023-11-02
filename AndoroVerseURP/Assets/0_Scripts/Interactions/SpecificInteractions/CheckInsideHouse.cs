using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInsideHouse : MonoBehaviour
{
    public bool m_Inside = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_Inside = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_Inside = false;
        }
    }
}
