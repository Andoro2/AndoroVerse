using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    private Transform m_Player;
    public Vector3 m_PlayerPos;
    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(m_Player.position);
        m_PlayerPos = m_Player.position;
    }
}
