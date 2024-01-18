using System.Collections;
using Cyrcadian;
using Cyrcadian.PlayerSystems;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameStateManager : MonoBehaviour
{

    [SerializeField] GameObject HUD_UI;
    [SerializeField] GameObject Inventory_UI;
    [SerializeField] GameObject PausedGame_UI;
    [SerializeField] GameObject GameOver_UI;

    public bool isPaused;
    public bool isInventory;

    // Used for having a switch case in 1 coroutine.
    private enum buttonPress
    {
        Load,
        Title,
        Quit
    };
    private buttonPress myButton;

    private float originalTimescale = 1;

    void Start()
    {
        Time.timeScale = originalTimescale;
        isPaused = false;
    }

    public void PauseGame()
    {
        PausedGame_UI.SetActive(true);
        HUD_UI.SetActive(false);
        Inventory_UI.SetActive(false);
        Time.timeScale = 0;
        isPaused = true;
        isInventory = false;
    }

    public void UnpauseGame()
    {
        PausedGame_UI.SetActive(false);
        HUD_UI.SetActive(true);
        Time.timeScale = originalTimescale;  
        isPaused = false;
    }

    public void OpenInventory()
    {
        Inventory_UI.SetActive(true);
        isInventory = true;
    }

    public void CloseInventory()
    {
        Inventory_UI.SetActive(false);
        isInventory = false;
    }

    public void SaveGame()
    {
        SaveSystem.Save();
    }

    public void LoadGame()
    {
        Time.timeScale = 1;
        myButton = buttonPress.Load;   
        StartCoroutine(buttonWait(myButton));
    }
    
    // GameOver will need refactoring once that system is being worked on
    // This is just a place holder for now
    public void GAMEOVER()
    {
        GameOver_UI.SetActive(true);
        Time.timeScale = 0;
        //Debug.Log("Game Over!");
    }

    // 0 will always be the very start of a game session, aka, title/main menu 
    public void ReturnToMainMenu()
    {
        myButton = buttonPress.Title;   
        StartCoroutine(buttonWait(myButton));
    }    

    // Simply for utility. Will reset whatever scene you are in.
    public void RestartScene()
    {
        Time.timeScale = originalTimescale;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    // Stops the application or leaves play mode
    public void QuitGame()
    {   
        myButton = buttonPress.Quit;   
        StartCoroutine(buttonWait(myButton));
    }

    // Here so jumping into a new scene doesn't feel like whiplash
    private IEnumerator buttonWait(buttonPress button)
    {
        yield return new WaitForSeconds(.25f);

        switch(button)
        {
            case buttonPress.Load:
                    SaveSystem.Load();
                break;
            case buttonPress.Title:
                    Time.timeScale = originalTimescale;
                    SceneManager.LoadSceneAsync(0);
                break;
            case buttonPress.Quit:
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif      
                break;
            default:
                break;
        }
    }
}
