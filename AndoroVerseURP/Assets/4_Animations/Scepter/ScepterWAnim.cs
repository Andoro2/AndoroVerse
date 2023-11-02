using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScepterWAnim : MonoBehaviour
{
    public Animator m_Animator;
    private void Awake()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        m_Animator.SetTrigger("Draw");
    }
    public void ScepterSave()
    {
        StartCoroutine("SaveScepter");
    }
    public IEnumerator SaveScepter()
    {
        m_Animator.SetTrigger("Save");
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
