using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogue : MonoBehaviour
{
    public float m_ReadTime = 2f;
    public Dialogue D;

    void Start()
    {
        //D = GetComponent<Dialogue>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            D.StartDialogue();
            StartCoroutine("Cerrar");
        }
    }
    private IEnumerator Cerrar()
    {
        yield return new WaitForSeconds(m_ReadTime);
        Destroy(gameObject);
    }
}
