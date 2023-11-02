using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBurn : MonoBehaviour
{
    public float m_DamageTime = 0.1f;

    private float m_Timer;
    private void OnEnable()
    {
        m_Timer = m_DamageTime;
    }

    private void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_DamageTime)
        {
            m_Timer = 0f;

            Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(1f,1f,5.5f));
            //Debug.Log(colliders.Length);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("Enemy"))
                {
                    Vector3 AttackVector = collider.transform.position - transform.parent.parent.transform.position;

                    Vector3 KnockBackDirection = new Vector3(AttackVector.x, 0f, AttackVector.z);

                    collider.GetComponent<EnemyLifeController>().TakeDamage(MainScript.PlayerFireDamage, KnockBackDirection, MainScript.PlayerKBForce);
                }
            }
        }
    }
}