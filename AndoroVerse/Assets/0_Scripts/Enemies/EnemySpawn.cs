using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public GameObject m_EnemyToSpawn, m_Cylinder, m_Materialize;
    public float m_VFXTime = 3f;
    
    public void SpawnEnemy()
    {
        StartCoroutine("Spawn");
    }
    IEnumerator Spawn()
    {
        Instantiate(m_Cylinder, transform);
        Instantiate(m_Materialize, transform);

        yield return new WaitForSeconds(m_VFXTime);

        Instantiate(m_EnemyToSpawn, transform);
    }
}
