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
[System.Serializable]
public class Skill
{
    public string SkillName;
    public bool Available = false;
}
public class Inventory : MonoBehaviour
{
    public List<Skill> m_Skills = new List<Skill>();
    public List<Item> m_Items = new List<Item>();
    public void ActivateSkill(Skill Ability)
    {
        for (int i = 0; i < m_Skills.Count; i++)
        {
            if (m_Skills[i].SkillName == Ability.SkillName)
            {
                m_Skills[i].Available = true;
            }
        }
    }
    public void DeactivateSkill(Skill Ability)
    {
        for (int i = 0; i < m_Skills.Count; i++)
        {
            if (m_Skills[i].SkillName == Ability.SkillName)
            {
                m_Skills[i].Available = false;
            }
        }
    }
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
