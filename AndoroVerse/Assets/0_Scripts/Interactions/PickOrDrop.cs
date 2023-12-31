using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PickOrDrop : MonoBehaviour
{
    //public Item[] Items;
    public List<Item> Items = new List<Item>();

    private Inventory Inventario;

    private bool NewItem = true;
    private void Start()
    {
        Inventario = GameObject.FindWithTag("GameController").GetComponent<Inventory>();
    }
    public void PickOrDropObject()
    {
        if (gameObject.GetComponent<InteractionType>().InteractionMode == InteractionType.InteractionTypes.PickUp)
        {
            PickUpObject();
        }
        else
        {
            DropDownObject();
        }

    }
    public void PickUpObject()
    {
        foreach(Item Item in Items)
        {
            for (int i = 0; i < Inventario.m_Items.Count; i++)
            {
                if (Inventario.m_Items[i].ObjectName == Item.ObjectName) NewItem = false;
            }

            if (NewItem)
            {
                if (Item.UniqueObject) Item.Quantity = 1;
                Inventario.AddItem(Item);
            }
            else Inventario.AddQuantity(Item);

            if (Item.UniqueObject)
            {
                gameObject.GetComponent<PickOrDrop>().enabled = false;
                gameObject.GetComponent<InteractionType>().enabled = false;
                gameObject.GetComponent<Collider>().enabled = false;
                gameObject.transform.GetChild(0).gameObject.SetActive(false);
            }
            //gameObject.GetComponent<PickOrDrop>().enabled = false;
        }
    }
    public void DropDownObject()
    {
        foreach (Item Item in Items)
            for (int i = 0; i < Inventario.m_Items.Count; i++)
            {
                if (Inventario.m_Items[i].ObjectName == Item.ObjectName)
                {
                    if (Item.UniqueObject)
                    {
                        Inventario.DropItem(Item);
                        gameObject.GetComponent<PickOrDrop>().enabled = false;
                    }
                    else
                    {
                        Inventario.DropQuantity(Item);
                        Item.Quantity -= Inventario.m_Items[i - 1].Quantity;
                    }
                    if (Item.Quantity == 0) gameObject.GetComponent<PickOrDrop>().enabled = false;
                }
            }
    }
}
