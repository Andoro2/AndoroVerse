using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float HitDmg;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MainScript.TakeDamage(HitDmg);
            Vector3 AttackVector = other.transform.position - transform.position;

            Vector3 KnockBackDirection = new(AttackVector.x, 0f, AttackVector.z);

            other.GetComponent<CharacterController>().TakeDamage(KnockBackDirection);
        }
    }
}
