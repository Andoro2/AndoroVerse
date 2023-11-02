using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private CollectibleInventory CI;
    // Start is called before the first frame update
    void Start()
    {
        CI = GameObject.FindWithTag("GameController").gameObject.GetComponent<CollectibleInventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadCollect()
    {
        CollectibleData dataC = SaveLoadSystem.LoadCollect();

        for (int i = 0; i < dataC.CollectibleList.Length; i++)
        {
            CI.m_Collectibles[i] = dataC.CollectibleList[i];
        }
    }
}
