using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject m_HitVFX;
    private AudioSource m_AS;
    public AudioClip m_AttackAudio;

    private void Start()
    {
        m_AS = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(m_HitVFX, transform.position, Quaternion.identity);

            Vector3 AttackVector = other.transform.position - transform.parent.parent.transform.position;

            Vector3 KnockBackDirection = new Vector3(AttackVector.x, 0f, AttackVector.z);

            other.GetComponent<EnemyLifeController>().TakeDamage(MainScript.PlayerMeleDamage, KnockBackDirection, MainScript.PlayerKBForce);

            if(m_AS != null)
            {
                m_AS.clip = m_AttackAudio;

                m_AS.Play();
            }
        }
    }
}