using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActivateOnCheckpoint : MonoBehaviour
{
    private GameObject[] m_Objects;

    public int m_CheckpointIndex;

    GameProgress GP;
    // Start is called before the first frame update
    void Start()
    {
        GP = GameObject.FindWithTag("GameController").gameObject.GetComponent<GameProgress>();

        m_Objects = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            m_Objects[i] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CheckpointIndex == GP.m_CheckPointIndex)
        {
            foreach (GameObject Object in m_Objects)
            {
                if (Object != null)
                {
                    Object.SetActive(true);
                }
            }
        }
        else
        {
            foreach (GameObject Object in m_Objects)
            {
                if (Object != null)
                {
                    Object.SetActive(false);
                }
            }
        }
    }
}
