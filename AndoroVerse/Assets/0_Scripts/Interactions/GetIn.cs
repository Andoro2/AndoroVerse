using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetIn : MonoBehaviour
{
    [SerializeField]
    private int m_SceneIndex;
    GameProgress GP;
    public void LoadScene()
    {
        GP = GameObject.FindWithTag("GameController").GetComponent<GameProgress>();
        GP.GetIndexsOnEnter();

        GameObject GameUI = GameObject.FindWithTag("UI");
        GameUI.transform.Find("InGameUI").gameObject.SetActive(false);
        GameUI.transform.Find("LoadingScreen").gameObject.SetActive(true);

        MainScript.RestoreLife();

        //SceneManager.LoadScene(m_SceneIndex);
        StartCoroutine("HoldLoad");
    }
    IEnumerator HoldLoad()
    {
        yield return new WaitForSeconds(0.25f);
        SceneManager.LoadScene(m_SceneIndex);
    }
}