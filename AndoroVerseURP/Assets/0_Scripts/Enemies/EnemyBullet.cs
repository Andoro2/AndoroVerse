using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Transform m_Player;
    public float BulletDmg, BulletSpeed, LiveTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, LiveTime);
        m_Player = GameObject.Find("Player").transform;
        Vector3 lookDirection = new Vector3(m_Player.position.x, m_Player.position.y, m_Player.position.z);
        transform.LookAt(lookDirection);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * BulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MainScript.PlayerLifePoints -= BulletDmg;
            Destroy(this.gameObject);
        }
        if (other.gameObject.CompareTag("Terrain"))
        {
            Destroy(this.gameObject);
        }
    }
}
