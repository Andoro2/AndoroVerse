using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateSkills : MonoBehaviour
{
    public string SkillToActivate;

    public void OnEnable()
    {
        if(GameObject.FindWithTag("GameController") != null) ActivateSkill(SkillToActivate);
    }
    public void ActivateSkill(string AbilityName)
    {
        for (int i = 0; i < GameObject.FindWithTag("GameController").transform.GetComponent<Inventory>().m_Skills.Count; i++)
        {
            if (GameObject.FindWithTag("GameController").transform.GetComponent<Inventory>().m_Skills[i].SkillName == AbilityName)
            {
                GameObject.FindWithTag("GameController").transform.GetComponent<Inventory>().m_Skills[i].Available = false;
            }
        }
    }
}
