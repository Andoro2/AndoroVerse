using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShock : MonoBehaviour
{
    public float m_KnockBackForce = 50f;

    private void Update()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(10f, 10f, 10f));

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Enemy"))
            {
                Vector3 AttackVector = collider.transform.position - transform.parent.parent.transform.position;

                Vector3 KnockBackDirection = new Vector3(AttackVector.x, 0f, AttackVector.z);

                collider.GetComponent<EnemyLifeController>().TakeDamage(MainScript.PlayerShockDamage, KnockBackDirection, m_KnockBackForce);
            }
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(10f, 10f, 10f));
    }
}