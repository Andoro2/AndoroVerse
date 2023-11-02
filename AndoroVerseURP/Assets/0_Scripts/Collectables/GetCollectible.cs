using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetCollectible : MonoBehaviour
{
    public string Collectable;
    private CollectibleInventory InventarioDeColeccionables;
    private CollectibleInventory CI;

    private List<Coleccionable> m_CollectablesChecker = new List<Coleccionable>();
    private void Start()
    {
        InventarioDeColeccionables = GameObject.FindWithTag("GameController").GetComponent<CollectibleInventory>();

        m_CollectablesChecker = InventarioDeColeccionables.m_Collectibles;

        for (int i = 0; i < m_CollectablesChecker.Count; i++)
        {
            if (m_CollectablesChecker[i].Name == Collectable && m_CollectablesChecker[i].Obtained)
            {
                Destroy(gameObject);
            }
        }

        CI = GameObject.FindWithTag("GameController").gameObject.GetComponent<CollectibleInventory>();
    }
    public void AcquireCollectible()
    {
        InventarioDeColeccionables.AcquireCollectible(Collectable);

        SaveLoadSystem.SaveCollect(CI);

        Destroy(gameObject);
    }
}
