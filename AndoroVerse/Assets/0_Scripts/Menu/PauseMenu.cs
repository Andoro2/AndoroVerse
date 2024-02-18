using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private UnityEngine.CharacterController m_Controller;
    private InputManager playerInputActions;

    public static bool GameIsPaused = false;
    public GameObject m_PauseMenuUI, GC;
    public MainScript MS;
    public CollectibleInventory CI;
    public GameProgress GP;
    private void Start()
    {
        
    }
    private void Update()
    {
        if (playerInputActions.UI.Pause.triggered)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    public void Resume()
    {
        m_PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause()
    {
        m_PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void SaveGame()
    {
        SaveLoadSystem.SaveGame(MS);
        SaveLoadSystem.SaveCollect(CI);
    }
    public void LoadGame()
    {
        MS.LoadGame();
    }
    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void RestartLevel()
    {
        MainScript.RestoreLife();
        GP.SetIndexOnReload();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    private void Awake()
    {
        playerInputActions = new InputManager();

        GC = GameObject.FindWithTag("GameController");
        MS = GC.GetComponent<MainScript>();
        CI = GC.GetComponent<CollectibleInventory>();
        GP = GC.GetComponent<GameProgress>();
        GP.GetIndexsOnEnter();
    }
    private void OnEnable()
    {
        playerInputActions.Enable();
    }
    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}
