using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Cyrcadian;

/****************************************************************************

            [ WARNING ] [ DEPRICATED ]

        This script is purely for reference purposes for writing new scripts. This is from a previous project.
        Once We are certain it is no longer useful as a refernce to building the game,
        Delete this script from the project.

        DO NOT try using this script for the game.
*/


public class PauseScript : MonoBehaviour
{
    [SerializeField] GameObject gamePauseUI;
    [SerializeField] GameObject gameInventoryUI;

    private float originalTimescale;
    public bool isPaused;

    public void gamePause()
    {
        gamePauseUI.SetActive(true);
      
        originalTimescale = Time.timeScale;
        isPaused = true;
        Time.timeScale = 0;

        GameObject.Find("Focal Point").GetComponent<camera_logic>().enabled = false;
    }

    public void gameUnpause()
    {
        gamePauseUI.SetActive(false);
        Time.timeScale = originalTimescale;
        isPaused = false;

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
