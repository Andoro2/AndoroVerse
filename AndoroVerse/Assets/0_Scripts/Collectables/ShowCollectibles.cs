using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowCollectibles : MonoBehaviour
{
    public string Collectible;
    private List<Coleccionable> m_CollectiblesChecker = new List<Coleccionable>();
    private CollectibleInventory InventarioDeColeccionables;
    //CollectibleInventory.m_Collectibles;

    private void Start()
    {
        InventarioDeColeccionables = GameObject.FindWithTag("GameController").GetComponent<CollectibleInventory>();
        m_CollectiblesChecker = InventarioDeColeccionables.m_Collectibles;

        for (int i = 0; i < m_CollectiblesChecker.Count; i++)
        {
            if (m_CollectiblesChecker[i].Name == Collectible)
            {
                if (m_CollectiblesChecker[i].Obtained == false) transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}
