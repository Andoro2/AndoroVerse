using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    public float m_FadeTime = 2.5f, m_FadeValue = 0.5f;
    float m_OGOpacity;
    Material[] m_Materials;
    public bool m_Fade = false;

    void Start()
    {
        m_Materials = GetComponent<MeshRenderer>().materials;

        foreach (Material Mat in m_Materials)
        {
            m_OGOpacity = Mat.color.a;
        }
    }

    void Update()
    {
        if (m_Fade)
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
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                Color m_Color = material.color;
                m_Color.a = Mathf.Lerp(m_Color.a, m_FadeValue, m_FadeTime * Time.deltaTime);
                material.color = m_Color;
            }
        }

    }
    public void FadeIn()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                Color m_Color = material.color;
                m_Color.a = Mathf.Lerp(m_Color.a, m_OGOpacity, m_FadeTime * Time.deltaTime);
                material.color = m_Color;
            }
        }
    }
}
