using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string ObjectName;
    public int Quantity;
    public bool UniqueObject;
}
public class Inventory : MonoBehaviour
{
    public List<Item> m_Items = new List<Item>();

    public void AddItem(Item Object)
    {
        m_Items.Add(Object);
    }
    public void AddQuantity(Item Object)
    {
        for (int i = 0; i < m_Items.Count; i++)
        {
            if (m_Items[i].ObjectName == Object.ObjectName)
            {
                m_Items[i].Quantity += Object.Quantity;
            }
        }
    }
    public void DropItem(Item Object)
    {
        for (int i = 0; i < m_Items.Count; i++)
        {
            if (m_Items[i].ObjectName == Object.ObjectName)
            {
                m_Items.RemoveAt(i);
            }
        }
    }
    public void DropQuantity(Item Object)
    {
        for (int i = 0; i < m_Items.Count; i++)
        {
            if (m_Items[i].ObjectName == Object.ObjectName && m_Items[i].Quantity >= Object.Quantity)
            {
                m_Items[i].Quantity -= Object.Quantity;
                if (m_Items[i].Quantity == 0) DropItem(m_Items[i]);
            }
        }
    }
}
