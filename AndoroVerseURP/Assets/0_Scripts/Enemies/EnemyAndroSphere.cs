using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAndroSphere : MonoBehaviour
{
    private enum States { Steady, Patrol, Attack };
    [SerializeField] private States m_EnemyState = States.Steady;
    private GameObject m_AttackArea;

    private Transform m_Player;
    public NavMeshAgent m_NavMeshAgent;

    public float DetectionRange = 25f;

    private Animator m_Anim;

    [SerializeField] private float m_AttackDamage = 10f;
    [SerializeField] private float m_AttackRange = 5f, m_AttackCD = 2.5f, m_AttackTimer;
    public int m_AttackAnim;

    public List<Slash> m_SlashVFXs;

    void Start()
    {
        m_Player = GameObject.Find("Player").transform;
        m_AttackTimer = m_AttackCD;

        m_Anim = GetComponentInChildren<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_AttackArea = transform.GetChild(1).gameObject;
        transform.GetChild(1).gameObject.GetComponent<EnemyDamage>().HitDmg = m_AttackDamage;
        m_AttackArea.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float DistanceToPlayer = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(m_Player.position.x, m_Player.position.z));

        if (DistanceToPlayer < DetectionRange) m_EnemyState = States.Attack;
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
                m_Anim.SetBool("AttackMoving", false);
                m_Anim.SetBool("Patrol", false);
            }
        }

        if (m_EnemyState == States.Attack)
        {
            m_NavMeshAgent.enabled = true;
            m_Anim.SetBool("AttackMoving", true);
            m_Anim.SetBool("Patrol", false);
            Attack(DistanceToPlayer);
        }
        else if (m_EnemyState == States.Patrol)
        {
            transform.GetComponent<EnemyPatrol>().Patrol();
            m_Anim.SetBool("Patrol", true);
            m_Anim.SetBool("AttackMoving", false);
        }
    }
    [System.Serializable]
    public class Slash
    {
        public GameObject SlashVFX;
        public float Delay;
    }
    void Attack(float Distance2Player)
    {
        if (Distance2Player <= m_AttackRange && m_AttackTimer <= 0f)
        {
            m_AttackAnim = Random.Range(0, 2);
            
            StartCoroutine(GetPunch(m_SlashVFXs[m_AttackAnim]));
            m_AttackTimer = m_AttackCD;
        }
        else
        {
            m_NavMeshAgent.SetDestination(m_Player.transform.position);
            m_Anim.SetBool("AttackMoving", true);
            m_Anim.SetBool("Patrol", false);
            if (m_AttackTimer > 0f)
            {
                m_AttackTimer -= Time.deltaTime;
            }
        }
    }

    private IEnumerator GetPunch(Slash m_SlashVFX)
    {
        m_NavMeshAgent.enabled = false;

        if (m_SlashVFX == m_SlashVFXs[0])
        {
            m_Anim.Play("LeftHit");
        }
        else
        {
            m_Anim.Play("RightHit");
        }

        yield return new WaitForSeconds(m_SlashVFX.Delay);
        m_AttackArea.SetActive(true);
        m_SlashVFX.SlashVFX.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        m_AttackArea.SetActive(false);
        m_SlashVFX.SlashVFX.SetActive(false);
        m_NavMeshAgent.enabled = true;
        //yield return new WaitForSeconds(m_AttackCD);
        m_EnemyState = States.Attack;
    }
}
