using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetIn : MonoBehaviour
{
    [SerializeField]
    private int m_SceneIndex;

    public void LoadScene()
    {
        SceneManager.LoadScene(m_SceneIndex);
    }
}