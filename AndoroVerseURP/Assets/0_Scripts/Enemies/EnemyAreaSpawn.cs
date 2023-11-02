using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAreaSpawn : MonoBehaviour
{
    public GameObject[] m_EnemiesSpawn;
    private void Start()
    {
        SpawnEnemies();
    }
    public void SpawnEnemies()
    {
        foreach(GameObject Enemigo in m_EnemiesSpawn)
        {
            Enemigo.GetComponent<EnemySpawn>().SpawnEnemy();
        }
    }
}
