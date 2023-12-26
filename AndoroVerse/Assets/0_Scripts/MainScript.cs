using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
    private enum GameStates { Exploring, Combating, Cinematic };
    [SerializeField] private GameStates GameState = GameStates.Exploring;

    private GameObject m_Player;

    public static float MaxPlayerLifePoints = 100f,
        PlayerLifePoints,
        PlayerMeleDamage,
        PlayerFireDamage,
        PlayerShockDamage,
        PlayerKBForce;

    public float m_PlayerMeleDamage,
        m_PlayerFireDamage,
        m_PlayerShockDamage,
        m_PlayerKBForce;

    private CollectibleInventory CI;

    public GameObject UI;
    public Slider m_HealthSlider;
    public Gradient m_HeathBarGradient;
    public Image m_FillLifeBar;

    public Transform m_ControlPoint;

    void Start()
    {
        //CI = GameObject.FindWithTag("GameController").gameObject.GetComponent<CollectibleInventory>();
        
        if(UI == null) FindUI();

        CI = GetComponent<CollectibleInventory>();
        m_Player = GameObject.FindGameObjectWithTag("Player");

        PlayerLifePoints = MaxPlayerLifePoints;

        m_HealthSlider.maxValue = MaxPlayerLifePoints;
        m_HealthSlider.value = PlayerLifePoints;
    }
    void FindUI()
    {
        UI = GameObject.FindWithTag("UI").gameObject.transform.Find("InGameUI").transform.Find("HealthBar").gameObject;
        m_HealthSlider = UI.transform.GetComponent<Slider>();
        m_FillLifeBar = UI.transform.Find("FillMask").transform.Find("Fill").transform.GetComponent<Image>();
    }
    void Update()
    {
        if (UI == null) FindUI();

        PlayerMeleDamage = m_PlayerMeleDamage;
        PlayerFireDamage = m_PlayerFireDamage;
        PlayerShockDamage = m_PlayerShockDamage;
        PlayerKBForce = m_PlayerKBForce;

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (PlayerLifePoints <= 0f)
        {
           Death();
        }

        m_HealthSlider.value = PlayerLifePoints;
        m_FillLifeBar.color = m_HeathBarGradient.Evaluate(m_HealthSlider.normalizedValue);
    }
    public void TakeDamage(float DamageValue)
    {
        PlayerLifePoints -= DamageValue;
        Debug.Log(PlayerLifePoints);
    }
    public void Heal(float HealthValue)
    {
        PlayerLifePoints += HealthValue;
    }
    public void Death()
    {
        if(m_Player == null) m_Player = GameObject.FindGameObjectWithTag("Player");
        m_Player.transform.GetComponent<CharacterController>().Death();

        GameObject[] m_Enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject Enemy in m_Enemies)
        {
            if(Enemy.gameObject.GetComponent<EnemyAndroSphere>() != null)
            {
                Enemy.gameObject.GetComponent<EnemyAndroSphere>().enabled = false;
            }
            else if (Enemy.gameObject.GetComponent<EnemyCannonFly>() != null)
            {
                Enemy.gameObject.GetComponent<EnemyCannonFly>().enabled = false;
            }
            else if (Enemy.gameObject.GetComponent<EnemyLeviTank>() != null)
            {
                Enemy.gameObject.GetComponent<EnemyLeviTank>().enabled = false;
            }
        }

        GameObject GameUI = GameObject.FindWithTag("UI");
        GameUI.transform.Find("InGameUI").gameObject.SetActive(false);
        GameUI.transform.Find("DeathScreen").gameObject.SetActive(true);
        PlayerLifePoints = MaxPlayerLifePoints;
    }
    public void RestoreLife()
    {
        PlayerLifePoints = MaxPlayerLifePoints;
    }
    /*private void OnGUI()
    {
        //GUI.skin = mySkin;

        GUI.Label(new Rect(Screen.width/2, 100, 150, 80), "Vida: " + PlayerLifePoints);

        //GUI.DrawTexture(new Rect(Screen.width / 4, Screen.height / 4, 80, 80), coinImg);
    }*/
    public void LoadGame()
    {
        PlayerData dataP = SaveLoadSystem.LoadGame();

        PlayerLifePoints = dataP.PlayerHealth;
        /*if(data.ControlPoint != null)
        {
            m_Player.transform.position = data.ControlPoint.transform.position;
        }*/
        //SceneManager.LoadScene(data.SceneIndex);
        LoadCollect();
    }
    public void LoadCollect()
    {
        CollectibleData dataC = SaveLoadSystem.LoadCollect();

        for (int i = 0; i < dataC.CollectibleList.Length; i++)
        {
            CI.m_Collectibles[i] = dataC.CollectibleList[i];
        }
    }
}
