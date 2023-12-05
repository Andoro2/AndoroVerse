using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float BulletDmg, BulletSpeed, LiveTime;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, LiveTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * BulletSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyLifeController>().TakeDamage(BulletDmg, Vector3.zero, 15f);
            Destroy(this.gameObject);
        }
    }
}
