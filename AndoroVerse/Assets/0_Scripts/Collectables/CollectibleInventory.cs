using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Coleccionable
{
    public string Name;
    public bool Obtained = false;
}
public class CollectibleInventory : MonoBehaviour
{
    public List<Coleccionable> m_Collectibles = new List<Coleccionable>();
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) ResetCollectibles();
    }
    public void ResetCollectibles()
    {
        for (int i = 0; i < m_Collectibles.Count; i++)
        {
            m_Collectibles[i].Obtained = false;
         
        }
    }
    public void AcquireCollectible(string CollectName)
    {
        for (int i = 0; i < m_Collectibles.Count; i++)
        {
            if (m_Collectibles[i].Name == CollectName)
            {
                m_Collectibles[i].Obtained = true;
            }
        }
    }
}
