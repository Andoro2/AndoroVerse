using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionOnContact : MonoBehaviour
{
    GameProgress GP;
    void Start()
    {
        GP = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GP.AdvanceCheckpoint();
        }
    }
}
