using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class PlayerData
{
    public int SceneIndex;
    public float PlayerHealth;
    //public GameObject ControlPoint;

    public PlayerData (MainScript player)
    {
        SceneIndex = SceneManager.GetActiveScene().buildIndex;

        PlayerHealth = MainScript.PlayerLifePoints;
    }
}
