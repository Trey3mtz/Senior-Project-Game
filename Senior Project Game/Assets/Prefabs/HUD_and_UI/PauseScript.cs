using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [SerializeField] GameObject gamePauseUI;

    private float originalTimescale;
    private bool isPaused = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            gamePause();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            gameUnpause();
        }
    }

    public void gamePause(){
    gamePauseUI.SetActive(true);
    isPaused = true;
    originalTimescale = Time.timeScale;
    
    Time.timeScale = 0;

        GameObject.Find("Player").GetComponent<PlayerController>().enabled = false;
        GameObject.Find("Focal Point").GetComponent<camera_logic>().enabled = false;
       
    }

    public void gameUnpause(){
        gamePauseUI.SetActive(false);
        isPaused = false;
        Time.timeScale = originalTimescale;

        GameObject.Find("Player").GetComponent<PlayerController>().enabled = true;
        GameObject.Find("Focal Point").GetComponent<camera_logic>().enabled = true;
        
    }




    public void QuitGame(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
