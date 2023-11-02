using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandTwirl : MonoBehaviour
{
    public float m_Increase = 0.001f, m_FinalValue;
    private float m_OriginalValue, pctg = 0f;
    // Start is called before the first frame update
    void Start()
    {
        m_OriginalValue = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x < m_FinalValue ||
           transform.localScale.y < m_FinalValue ||
           transform.localScale.z < m_FinalValue)
        {
            pctg += m_Increase;
        }

        transform.localScale = new Vector3(Mathf.Lerp(m_OriginalValue, m_FinalValue, pctg),
            Mathf.Lerp(m_OriginalValue, m_FinalValue, pctg),
            Mathf.Lerp(m_OriginalValue, m_FinalValue, pctg));
    }
}
