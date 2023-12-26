using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    [SerializeField] GameObject gamePauseUI;
    [SerializeField] GameObject gameInventoryUI;

    private float originalTimescale;

    public void gamePause()
    {
        gamePauseUI.SetActive(true);
      
        originalTimescale = Time.timeScale;
        Time.timeScale = 0;

        GameObject.Find("Focal Point").GetComponent<camera_logic>().enabled = false;
    }

    public void gameUnpause()
    {
        gamePauseUI.SetActive(false);
        Time.timeScale = originalTimescale;

        GameObject.Find("Focal Point").GetComponent<camera_logic>().enabled = true;    
    }

    public void openInventory()
    {
        gameInventoryUI.SetActive(true);
    }
    
    public void closeInventory()
    {
        gameInventoryUI.SetActive(false);
    }

    public void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void RestartScene(){
         SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
         Time.timeScale = originalTimescale;
    }
}
