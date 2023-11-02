using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMortarProjectile : MonoBehaviour
{
    public Transform m_Player;
    public LayerMask groundLayer;
    public GameObject m_ExplosionVFX;
    private Light m_ProjectileMarker;

    public float m_LaunchAngle = 45f,
        m_LaunchPower = 10f,
        m_Height,
        m_ExplosionDmg = 10f;

    private Rigidbody rb;

    private Vector3 m_StartPos,
        m_TargetPos;
    private float m_HorizontalDist,
        gravity = 9.8f;

    private void Start()
    {
        m_Player = GameObject.Find("Player").transform;
        m_ProjectileMarker = gameObject.transform.GetChild(1).GetComponent<Light>();

        rb = GetComponent<Rigidbody>();

        m_StartPos = transform.position;
        m_TargetPos = m_Player.position;

        Vector3 Displacement = m_TargetPos - m_StartPos;
        Displacement.y = 0f;
        m_HorizontalDist = Displacement.magnitude;

        float VerticalVel = m_LaunchPower * Mathf.Sin(m_LaunchAngle * Mathf.Deg2Rad);

        float FlightTime = (2f * VerticalVel) / gravity;

        float HorDist = m_HorizontalDist / FlightTime;
        Vector3 HorDir = (m_TargetPos - m_StartPos).normalized;

        Vector3 LaunchVel = HorDir * HorDist;
        LaunchVel.y = VerticalVel;

        rb.velocity = LaunchVel;
    }
    private void Update()
    {
        if(CheckHeigth() <= 1.5f)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10f);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("Player"))
                {

                    MainScript.PlayerLifePoints -= m_ExplosionDmg;
                    Vector3 AttackVector = collider.transform.position - transform.position;

                    Vector3 KnockBackDirection = new(AttackVector.x, 0f, AttackVector.z);

                    collider.GetComponent<CharacterController>().TakeDamage(KnockBackDirection);
                }
            }

            GameObject ExploVFX = Instantiate(m_ExplosionVFX, transform);
            ExploVFX.transform.parent = null;
            Destroy(gameObject);
        }

        m_ProjectileMarker.spotAngle = 50 / (gameObject.transform.position.y / 5);
        m_ProjectileMarker.innerSpotAngle = 50 / (gameObject.transform.position.y / 5);
    }
    private float CheckHeigth()
    {
        RaycastHit hit;
        int LayerGround = LayerMask.GetMask("Terrain");

        Debug.DrawRay(transform.position, Vector3.down * Mathf.Infinity, Color.red);
        Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerGround);
        m_Height = hit.distance;
        return hit.distance;
    }
}
