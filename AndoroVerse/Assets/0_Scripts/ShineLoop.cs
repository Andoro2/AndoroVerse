using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ShineLoop : MonoBehaviour
{
    private VisualEffect m_Shine;

    void Start()
    {
        m_Shine = GetComponent<VisualEffect>();
        InvokeRepeating("ResetShine", 0f, 1f);
    }

    void ResetShine()
    {
        m_Shine.Play();
    }
}
