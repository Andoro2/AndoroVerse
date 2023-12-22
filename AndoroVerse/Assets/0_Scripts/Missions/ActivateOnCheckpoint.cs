using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActivateOnCheckpoint : MonoBehaviour
{
    private GameObject[] m_Objects;

    GameObject[] NewObjects;

    public int[] m_CheckpointIndex;
    public SkinnedMeshRenderer PutoChalecoDeMierda;
    //public bool activa = false;
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
        if (gameObject.name == "logo.001")
        {
            PutoChalecoDeMierda = GetComponent<SkinnedMeshRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CheckpointIndex.Contains(GP.m_CheckPointIndex) && PutoChalecoDeMierda != null && PutoChalecoDeMierda.enabled == false)
        {
            PutoChalecoDeMierda.enabled = true;
        }
        else if (!m_CheckpointIndex.Contains(GP.m_CheckPointIndex) && PutoChalecoDeMierda != null && PutoChalecoDeMierda.enabled == true)
        {
            PutoChalecoDeMierda.enabled = false;
        }

        if (transform.childCount > 0)
        {
            RevisarHijos();
            if (m_CheckpointIndex.Contains(GP.m_CheckPointIndex))
            {
                if (PutoChalecoDeMierda != null)
                {
                    PutoChalecoDeMierda.enabled = true;
                    Debug.Log("Tuputamadre");
                }

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
                if (PutoChalecoDeMierda != null)
                {
                    PutoChalecoDeMierda.enabled = false;
                    Debug.Log("Turecontraputamadre");
                }

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
    void RevisarHijos()
    {
        NewObjects = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            NewObjects[i] = transform.GetChild(i).gameObject;
        }
        if (NewObjects != m_Objects) m_Objects = NewObjects;
        NewObjects = new GameObject[0];
    }
}
