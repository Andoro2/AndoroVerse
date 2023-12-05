using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSkills : MonoBehaviour
{
    //private List<Skill> m_Skills = new List<Skill>();
    public string SkillToActivate;
    // Start is called before the first frame update
    void Start()
    {
        //m_Skills = GameObject.FindWithTag("GameController").transform.GetComponent<Inventory>().m_Skills;
    }

    public void OnEnable()
    {
        ActivateSkill(SkillToActivate);
    }
    public void ActivateSkill(string AbilityName)
    {
        for (int i = 0; i < GameObject.FindWithTag("GameController").transform.GetComponent<Inventory>().m_Skills.Count; i++)
        {
            if (GameObject.FindWithTag("GameController").transform.GetComponent<Inventory>().m_Skills[i].SkillName == AbilityName)
            {
                GameObject.FindWithTag("GameController").transform.GetComponent<Inventory>().m_Skills[i].Available = true;
            }
        }
    }
}
