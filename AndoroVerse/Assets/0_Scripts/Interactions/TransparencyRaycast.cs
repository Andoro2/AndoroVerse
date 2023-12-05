using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyRaycast : MonoBehaviour
{
    Transform m_Player;
    private ObjectFader m_Fader;
    //public float m_AlphaValue = 0.25f;
    private void Start()
    {
        m_Player = GameObject.Find("Player").transform;
    }
    private void Update()
    {
        m_Player = GameObject.FindGameObjectWithTag("Player").gameObject.transform;

        Vector3 m_RayDir = m_Player.position - transform.position;
        Ray ray = new Ray(transform.position, m_RayDir);
        RaycastHit hit;
        Debug.DrawRay(transform.position, m_RayDir * 10, Color.green);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider == null)
                return;
            if (hit.collider.CompareTag("Wall"))
            {
                m_Fader = hit.collider.gameObject.GetComponent<ObjectFader>();
                if(m_Fader != null)
                {
                    m_Fader.m_Fade = true; ;
                }                
            }

            //if (hit.collider.CompareTag("Player"))
            else
            {
                if (m_Fader != null)
                {
                    m_Fader.m_Fade = false;
                }
            }
        }
    }
}
