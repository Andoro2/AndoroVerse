using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] m_PatrolPoints;
    public float m_PatrolSpeed = 5f,
        m_RotationSpeed = 10f,
        m_TimeStopped = 1f;
    public AudioClip m_MovementAudio;

    private AudioSource AS;
    private int m_PatrolPointIndex = 0;
    private bool m_Stopped = false;
    private float m_IdleTimer = 0f;

    void Start()
    {
        AS = GetComponent<AudioSource>();        
    }
    public void Patrol()
    {
        Quaternion targetRotation;

        if (m_Stopped)
        {
            AS.Stop();
            m_IdleTimer += Time.deltaTime;
            if (m_IdleTimer >= m_TimeStopped)
            {
                m_Stopped = false;
                m_IdleTimer = 0f;
                ProceedToNextWaypoint();
            }
            else
            {
                Transform nextWaypoint = m_PatrolPoints[(m_PatrolPointIndex + 1) % m_PatrolPoints.Length];
                targetRotation = Quaternion.LookRotation(nextWaypoint.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);
            }
            return;
        }
        else
        {
            AS.clip = m_MovementAudio;
            AS.Play();
        }

        Transform currentWaypoint = m_PatrolPoints[m_PatrolPointIndex];

        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, m_PatrolSpeed * Time.deltaTime);

        targetRotation = Quaternion.LookRotation(currentWaypoint.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, m_RotationSpeed * Time.deltaTime);

        if (transform.position == currentWaypoint.position)
        {
            m_Stopped = true;
        }
    }

    private void ProceedToNextWaypoint()
    {
        m_PatrolPointIndex++;
        if (m_PatrolPointIndex >= m_PatrolPoints.Length)
        {
            m_PatrolPointIndex = 0;
        }
    }
}

