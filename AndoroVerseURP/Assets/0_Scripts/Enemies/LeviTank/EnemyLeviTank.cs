using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLeviTank : MonoBehaviour
{
    private Transform m_Player, m_LaunchPoint;
    public NavMeshAgent m_NavMeshAgent;
    public GameObject m_Projectile;

    [SerializeField] private float m_AttackDamage = 10f;
    [SerializeField] private float m_AttackRange = 25f, m_AttackCD = 5f, m_AttackTimer;

    void Start()
    {
        m_Player = GameObject.Find("Player").transform;
        m_LaunchPoint = transform.GetChild(0).GetChild(0).GetChild(0).transform;
        m_AttackTimer = m_AttackCD;

        //m_Anim = GetComponentInChildren<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float DistanceToPlayer = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(m_Player.position.x, m_Player.position.z));

        if (DistanceToPlayer < m_AttackRange)
        {
            Attack();
        }
    }

    void Attack()
    {
        m_AttackTimer -= Time.deltaTime;

        if(m_AttackTimer <= 0)
        {
            GameObject projectile = Instantiate(m_Projectile, transform.Find("Body/Mortero/ShootPoint").transform);
            
            projectile.transform.parent = null;
            projectile.transform.localScale = new Vector3(1, 1, 1);

            m_AttackTimer = m_AttackCD;
        }
    }
}
