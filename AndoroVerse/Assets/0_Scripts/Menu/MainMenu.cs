using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    //private CollectibleInventory CI;

    void Start()
    {
        //CI = GameObject.FindWithTag("GameController").gameObject.GetComponent<CollectibleInventory>();
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
    public string AndoroLink = "https://twitter.com/anderu2__";
    public void LinkTwitterAndoro()
    {
        // Abrir el enlace en el navegador web predeterminado.
        Application.OpenURL(AndoroLink);
    }
    public string JordiLink = "https://twitter.com/Byjcp0072";
    public void LinkTwitterJordi()
    {
        // Abrir el enlace en el navegador web predeterminado.
        Application.OpenURL(JordiLink);
    }

    /*public void LoadCollect()
    {
        CollectibleData dataC = SaveLoadSystem.LoadCollect();

        for (int i = 0; i < dataC.CollectibleList.Length; i++)
        {
            CI.m_Collectibles[i] = dataC.CollectibleList[i];
        }
    }*/
}
