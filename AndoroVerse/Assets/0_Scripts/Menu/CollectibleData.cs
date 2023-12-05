using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectibleData
{
    public Coleccionable[] CollectibleList;

    public CollectibleData(CollectibleInventory coleccionables)
    {
        CollectibleList = coleccionables.m_Collectibles.ToArray();
    }
}
