using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCompleter : MonoBehaviour
{
    private enum CheckpointTypes { Item, Dialogue, Enemies };
    [SerializeField] private CheckpointTypes CheckpointType;
    public bool m_MissionAdvance;

    GameProgress GP;
    void Start()
    {
        GP = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();
    }

    void Update()
    {
        if (CheckpointConditions())
        {
            GP.AdvanceCheckpoint();
            //gameObject.GetComponent<MissionCompleter>().enabled = false;
            gameObject.SetActive(false);
            if (m_MissionAdvance) GP.AdvanceMission();
            //Destroy(gameObject);
        }
    }

    bool CheckpointConditions()
    {
        switch (CheckpointType)
        {
            case CheckpointTypes.Item:
                if (gameObject.GetComponent<PickOrDrop>().enabled == false)
                {
                    return true;
                }
                break;
            case CheckpointTypes.Dialogue:
                if (gameObject.GetComponent<Dialogue>().enabled == false)
                {
                    return true;
                }
                break;
            case CheckpointTypes.Enemies:
                GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");

                int EnemiesCount = 0;

                foreach (GameObject Enemy in Enemies)
                {
                    if (Enemy.activeSelf)
                    {
                        EnemiesCount++;
                    }
                }
                if (EnemiesCount == 0)
                {
                    return true;
                }
                break;
            default:
                break;

        }
        return false;
    }
}
