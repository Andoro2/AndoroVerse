using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLifeController : MonoBehaviour
{
    public float m_MaxLifePoints = 75f, m_LifePoints, m_KnockBackForce, m_KnockBackDuration;
    private Rigidbody rb;

    public Slider m_HealthSlider;

    public Item[] m_Drops;
    public GameObject m_PickUpPreFab;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        m_LifePoints = m_MaxLifePoints;

        m_HealthSlider.maxValue = m_MaxLifePoints;
    }
    public void Update()
    {
        m_HealthSlider.value = m_LifePoints;
    }
    public void TakeDamage(float Dmg, Vector3 KnockBackDir, float KnockBackForce)
    {
        m_KnockBackForce = KnockBackForce;

        StartCoroutine("KnockbackImpulse", KnockBackDir);

        m_LifePoints -= Dmg;

        if (m_LifePoints <= 0f)
        {
            if(m_Drops.Length > 0f)
            {
                foreach(Item Drop in m_Drops)
                {
                    GameObject Droped = Instantiate(m_PickUpPreFab, transform.position, transform.rotation);

                    Droped.GetComponent<PickOrDrop>().Items.Add(Drop);
                }
            }
            Destroy(this.gameObject);
        }
    }
    IEnumerator KnockbackImpulse(Vector3 KnockBackDir)
    {
        Vector3 m_KnockBack = KnockBackDir.normalized * m_KnockBackForce;

        rb.AddForce(m_KnockBack, ForceMode.Impulse);

        yield return new WaitForSeconds(m_KnockBackDuration);

        // Revierte la fuerza de knockback
        rb.velocity = Vector3.zero;
    }
}
