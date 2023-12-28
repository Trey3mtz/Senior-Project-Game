using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject HUD_UI;
    [SerializeField] GameObject Inventory_UI;
    [SerializeField] GameObject PausedGame_UI;
    [SerializeField] GameObject GameOver_UI;

    public bool isPaused;

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
        Time.timeScale = 0;
        isPaused = true;
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
    }

    public void CloseInventory()
    {
        Inventory_UI.SetActive(false);
    }

    
    // GameOver will need refactoring once that system is being worked on
    // This is just a place holder for now
    public void GAMEOVER()
    {
        GameOver_UI.SetActive(true);
        Time.timeScale = 0;
        //Debug.Log("Game Over!");
    }

    public void RestartScene()
    {
        Time.timeScale = originalTimescale;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}