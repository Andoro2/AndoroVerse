using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform m_Camera;
    private void Start()
    {
        m_Camera = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + m_Camera.forward);
    }
}
