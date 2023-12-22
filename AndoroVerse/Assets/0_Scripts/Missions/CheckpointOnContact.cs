using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointOnContact : MonoBehaviour
{
    GameProgress GP;
    public bool m_MissionAdvance;
    void Start()
    {
        GP = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GP.AdvanceCheckpoint();

            if (m_MissionAdvance) GP.AdvanceMission();
        }
    }
}
