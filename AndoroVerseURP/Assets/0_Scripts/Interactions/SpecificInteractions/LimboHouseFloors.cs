using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimboHouseFloors : MonoBehaviour
{
    public float m_FadeTime = 2f, m_FadeValue = 0.1f;
    public GameObject[] m_Floor, m_FloorColliders;
    //float m_OGOpacity;
    private List<Material> m_MaterialsList = new List<Material>();
    private Material[] m_Materials;
    private Renderer[] m_Renderers;

    public bool m_Inside = false;

    void Start()
    {
        foreach(GameObject Thing in m_Floor)
        {
            m_Renderers = Thing.GetComponentsInChildren<Renderer>();
        }

        foreach (Renderer renderer in m_Renderers)
        {
            foreach(Material material in renderer.materials)
            {
                m_MaterialsList.Add(material);
            }
        }

        m_Materials = m_MaterialsList.ToArray();
    }


    void Update()
    {
        /*Vector3 sizeOne = new Vector3(20f, 10f, 15f);
        Vector3 sizeTwo = new Vector3(9.75f, 10f, 8.5f);

        Collider[] collidersOne = Physics.OverlapBox(transform.position, sizeOne, Quaternion.identity, 7);
        Collider[] collidersTwo = Physics.OverlapBox(transform.position + new Vector3(-5.25f, 0f, -8.44f), sizeTwo, Quaternion.identity, 7);

        foreach (Collider collider in collidersOne)
        {
            if (collider.CompareTag("Player")) m_Inside = true;
            else
            {
                m_Inside = false;
                foreach (GameObject Thing in m_Floor)
                {
                    Thing.SetActive(true);
                }
            }
        }

        foreach (Collider collider in collidersTwo)
        {
            if (collider.CompareTag("Player")) m_Inside = true;
            else
            {
                m_Inside = false;
                foreach (GameObject Thing in m_Floor)
                {
                    Thing.SetActive(true);
                }
            }
        }*/
        int colisiones = 0;
        foreach(GameObject colaider in m_FloorColliders)
        {
            if (colaider.GetComponent<CheckInsideHouse>().m_Inside)
            {
                colisiones++;
            }
        }

        if (colisiones != 0) m_Inside = true;
        else m_Inside = false;

        if (m_Inside)
        {
            FadeOut();
        }
        else
        {
            FadeIn();
        }
    }
    public void FadeOut()
    {
        /*foreach (GameObject Thing in m_Floor)
        {
            Thing.gameObject.SetActive(false);
        }*/
        for (int i = 0; i < m_Materials.Length; i++)
        {
            
            StartCoroutine("DesactivarObj");
            Material material = m_Materials[i];
            Color m_Color = material.color;
            m_Color.a = Mathf.Lerp(m_Color.a, m_FadeValue, m_FadeTime * Time.deltaTime);
            material.color = m_Color;
        }
    }
    public void FadeIn()
    {
        foreach (GameObject Thing in m_Floor)
        {
            Thing.gameObject.SetActive(true);
        }
        for (int i = 0; i < m_Materials.Length; i++)
        {
            Material material = m_Materials[i];
            Color m_Color = material.color;
            m_Color.a = Mathf.Lerp(m_Color.a, 1f, m_FadeTime * Time.deltaTime);
            material.color = m_Color;
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_Inside = true;
            StartCoroutine("DesactivarObj");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_Inside = false;
            foreach (GameObject Thing in m_Floor)
            {
                Thing.SetActive(true);
            }
        }
    }
    */
    private IEnumerator DesactivarObj()
    {
        yield return new WaitForSeconds(0.25f);
        foreach (GameObject Thing in m_Floor)
        {
            if(Thing.activeSelf) Thing.gameObject.SetActive(false);
        }
    }
}
