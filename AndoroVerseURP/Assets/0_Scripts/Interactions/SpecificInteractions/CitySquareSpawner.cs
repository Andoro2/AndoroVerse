using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySquareSpawner : MonoBehaviour
{
    public int m_EnemiesLimit = 40;
    public float m_SpawnTime = 2;
    private float m_SpawnTimer;

    public Transform[] m_SpawnPoints;
    public GameObject[] m_EnemiesToSpawn;

    GameProgress GM;
    void Start()
    {
        GM = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();
        m_SpawnTimer = m_SpawnTime;
    }

    void Update()
    {
        if(m_SpawnTimer <= 0)
        {
            Spawn();
            m_SpawnTimer = m_SpawnTime;
        }
        else
        {
            m_SpawnTimer -= Time.deltaTime;
        }

        GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

        int EnemiesCount = 0;

        foreach (GameObject Enemy in Enemies)
        {
            if (Enemy.activeSelf)
            {
                Enemy.GetComponent<EnemyAndroSphere>().DetectionRange = 500;
                EnemiesCount++;
            }
        }
        if (EnemiesCount == m_EnemiesLimit)
        {
            GM.AdvanceCheckpoint();
        }
    }
    void Spawn()
    {
        int SpawnPointIndex = Random.Range(0,m_SpawnPoints.Length - 1);

        int EnemySpawnIndex = Random.Range(0, m_EnemiesToSpawn.Length - 1);

        if(m_SpawnTime < 0.2f)
        {
            m_SpawnTime -= 0.15f;
        }

        GameObject Enemy = Instantiate(m_EnemiesToSpawn[EnemySpawnIndex], m_SpawnPoints[SpawnPointIndex].transform.position, Quaternion.identity);

        Enemy.transform.parent = gameObject.transform;

        Enemy.GetComponent<EnemySpawn>().SpawnEnemy();
    }
}
