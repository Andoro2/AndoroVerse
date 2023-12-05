using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLeviTank : MonoBehaviour
{
    private Transform m_Player, m_LaunchPoint;
    public NavMeshAgent m_NavMeshAgent;
    public GameObject m_Projectile;
    private enum States { Steady, Patrol, Attack };
    [SerializeField] private States m_EnemyState = States.Steady;

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

        if (DistanceToPlayer < m_AttackRange) m_EnemyState = States.Attack;
        else
        {
            m_NavMeshAgent.enabled = false;
            if (gameObject.transform.GetComponent<EnemyPatrol>().m_PatrolPoints.Length > 0)
            {
                m_EnemyState = States.Patrol;
            }
            else
            {
                m_EnemyState = States.Steady;
                //m_Anim.SetBool("AttackMoving", false);
                //m_Anim.SetBool("Patrol", false);
            }
        }

        if(m_EnemyState == States.Attack)
        {
            m_NavMeshAgent.enabled = true;
            transform.GetComponent<EnemyPatrol>().Patrol();
            Attack();
        }
        else if (m_EnemyState == States.Patrol)
        {
            transform.GetComponent<EnemyPatrol>().Patrol();
            //m_Anim.SetBool("Patrol", true);
            //m_Anim.SetBool("AttackMoving", false);
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
