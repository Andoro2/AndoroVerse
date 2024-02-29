using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannonFly : MonoBehaviour
{
    private enum States { Steady, Patrol, Attack };
    [SerializeField] private States m_EnemyState = States.Steady;

    [SerializeField] private float m_DetectionRange = 25f;
    public GameObject m_Bullet, m_Cannon;

    public Animator m_AnimBody, m_AnimCannon;

    private Transform m_Player;
    public Transform m_ShootPoint;

    [SerializeField] private float m_MinDistanceToPlayer = 10f, m_MaxDistanceToPlayer = 15f;
    [SerializeField] private float m_RetreatSpeed = 1.25f, m_OffenseSpeed = 1.5f, m_VerticalSpeed = 2.5f;
    [SerializeField] private float m_ShootRange = 17.5f, m_AttackCD = 2.5f, m_AttackTimer;
    public float m_Heigth = 7.5f;

    void Start()
    {
        m_Player = GameObject.Find("Player").transform;
        m_AttackTimer = m_AttackCD;
    }

    void Update()
    {
        float DistanceToPlayer = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(m_Player.position.x, m_Player.position.z));

        if (DistanceToPlayer < m_DetectionRange) m_EnemyState = States.Attack;
        else
        {
            if(gameObject.transform.GetComponent<EnemyPatrol>().m_PatrolPoints.Length > 0)
            {
                m_EnemyState = States.Patrol;
            }
            else
            {
                m_EnemyState = States.Steady;
            }
        }

        if(m_EnemyState == States.Attack)
        {
            Attack(DistanceToPlayer);
        }
        else if(m_EnemyState == States.Patrol)
        {
            gameObject.transform.GetComponent<EnemyPatrol>().Patrol();
        }

        Shoot(DistanceToPlayer);
        CheckHeigth();
    }
    
    void Attack(float Distance2Player)
    {
        Vector3 LookDirection = new(m_Player.position.x, transform.position.y, m_Player.position.z);
        transform.LookAt(LookDirection);

        m_Cannon.transform.LookAt(m_Player.position);

        if (Distance2Player >= m_MaxDistanceToPlayer)
        {
            Vector3 OffensiveDir = m_Player.position - transform.position;
            transform.position += m_OffenseSpeed * Time.deltaTime * new Vector3(OffensiveDir.x, 0f, OffensiveDir.z).normalized;
            m_AnimBody.SetBool("Moving", true);
        }
        /*
        if (Distance2Player <= m_MinDistanceToPlayer)
        {
            Vector3 RetreatDir = transform.position - m_Player.position;
            transform.position += m_RetreatSpeed * Time.deltaTime * new Vector3(RetreatDir.x, 0f, RetreatDir.z).normalized;
            m_AnimBody.SetBool("Moving", true);
        }
        else if (Distance2Player >= m_MaxDistanceToPlayer)
        {
            Vector3 OffensiveDir = m_Player.position - transform.position;
            transform.position += m_OffenseSpeed * Time.deltaTime * new Vector3(OffensiveDir.x, 0f, OffensiveDir.z).normalized;
            m_AnimBody.SetBool("Moving", true);
        }
        */
    }
    void Shoot(float Distance2Player)
    {
        m_AttackTimer -= Time.deltaTime;
        if (Distance2Player <= m_ShootRange && m_AttackTimer <= 0f)
        {
            Instantiate(m_Bullet, m_ShootPoint.position, m_ShootPoint.rotation);
            m_AttackTimer = m_AttackCD;

            m_AnimCannon.Play("Shoot");
        }
    }
    void CheckHeigth()
    {
        int LayerGround = LayerMask.GetMask("Terrain");

        Debug.DrawRay(transform.position, -Vector3.up * m_Heigth, Color.red);
        if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, m_Heigth * 1.5f, LayerGround))
        {
            if (hit.distance > m_Heigth)
                transform.position += m_VerticalSpeed * Time.deltaTime * -Vector3.up;
            else if (hit.distance < (m_Heigth - 0.5f))
                transform.position += m_VerticalSpeed * Time.deltaTime * Vector3.up;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, m_Heigth - 0.5f, transform.position.z);
        }
    }
}