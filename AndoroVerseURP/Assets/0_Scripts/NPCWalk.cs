using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class MissionPath
{
    public int m_MissionIndex;
    public Transform[] m_WayPoints;
    public bool m_Running;
}
public class NPCWalk : MonoBehaviour
{
    public MissionPath[] m_MissionPaths;
    private Transform m_Player;
    public int m_DestinationIndex = 0;
    public float m_WalkSpeed, m_RunSpeed;
    public NavMeshAgent m_NavMeshAgent;
    public Animator m_Anim;
    private int m_MissionActive;//, m_MissionSaved;

    private GameProgress GM;
    void Start()
    {
        GM = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();

        m_Player = GameObject.Find("Player").transform;
        m_Anim = GetComponentInChildren<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();

        m_MissionActive = GM.m_MissionIndex;
    }

    void Update()
    {
        if(m_MissionActive != GM.m_MissionIndex)
        {
            m_MissionActive = GM.m_MissionIndex;
            m_DestinationIndex = 0;
        }

        foreach (MissionPath Ruta in m_MissionPaths)
        {
            if (Ruta.m_Running) m_NavMeshAgent.speed = m_RunSpeed;
            else m_NavMeshAgent.speed = m_WalkSpeed;

            if(Ruta.m_MissionIndex == GM.m_MissionIndex)
            {
                if (m_DestinationIndex < Ruta.m_WayPoints.Length && Ruta.m_WayPoints.Length != 0)
                {
                    if(!m_NavMeshAgent.enabled) m_NavMeshAgent.enabled = true;
                    m_NavMeshAgent.SetDestination(Ruta.m_WayPoints[m_DestinationIndex].transform.position);
                    
                    //if (Ruta.m_Running) m_Anim.SetBool("Running", true);
                    //else m_Anim.SetBool("Walking", true);

                    m_Anim.SetBool("Walking", true);
                    if (Vector3.Distance(transform.position, Ruta.m_WayPoints[m_DestinationIndex].transform.position) <= 1.5f)
                    {
                        m_DestinationIndex++;
                    }
                }
                else
                {
                    m_Anim.SetBool("Walking", false);
                    m_NavMeshAgent.enabled = false;
                    transform.LookAt(m_Player);
                }
            }
        }
    }
}
