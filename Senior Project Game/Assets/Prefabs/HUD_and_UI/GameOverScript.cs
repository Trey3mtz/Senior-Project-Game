using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cyrcadian;

/****************************************************************************

            [ WARNING ] [ DEPRICATED ]

        This script is purely for reference from a previous project.
        Once We are certain it is no longer useful as a refernce to building the game,
        Delete this script from the project.

        DO NOT try using this script for the game.
*/

public class GameOverScript : MonoBehaviour
{

[SerializeField] GameObject gameOverUI;
[SerializeField] GameObject gameWonUI;
[SerializeField] GameObject gamePauseUI;
[SerializeField] AudioSource saintSeiya;

    public bool isTutorial = false;
    public float slowdownFactor = 0.5f;
    public float slowdownDuration = 2f;

private bool ded;
private int enemyCount;
public IEnumerator coroutine;
   void Awake()
   {
    enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
     Debug.Log("Defeat all " + (enemyCount) + " enemies!");
   }



    public void gameOver(){
        
        
        gameOverUI.SetActive(true);

        GameObject.Find("In-Game UI").SetActive(false);
        
        GameObject.Find("Focal Point").GetComponent<camera_logic>().enabled = false;
        
    }

    public void gameWon(){
        gameWonUI.SetActive(true);

        GameObject.Find("In-Game UI").SetActive(false);
        GameObject.Find("Player").GetComponent<PlayerController>().enabled = false;
        GameObject.Find("Focal Point").GetComponent<camera_logic>().enabled = false;

    }

    public void gamePause(){
        gamePauseUI.SetActive(true);

        Time.timeScale = 0;

      //  GameObject.Find("Player").GetComponent<PlayerController>().enabled = false;
        GameObject.Find("Focal Point").GetComponent<camera_logic>().enabled = false;
    }

    public void gameUnpause(){
        gamePauseUI.SetActive(false);
        Time.timeScale = 1;

        GameObject.Find("Player").GetComponent<PlayerController>().enabled = true;
        GameObject.Find("Focal Point").GetComponent<camera_logic>().enabled = true;
    }

    public void enemyKilled()
    {
        enemyCount = enemyCount - 1;
        if(enemyCount >= 0)
         Debug.Log("There are " + (enemyCount) + " enemies left");
        if(enemyCount == 0) 
        {
            
            StartCoroutine(wonGame());
        }
    }

    public void playerDeath()
    {   
        Time.timeScale = 0.1f;
        StartCoroutine(SlowdownTime());
        StartCoroutine(lostGame());
    }


    public void titleButton(){
        SceneManager.LoadScene("Title Screen");
    }

    public void RestartButton(){
        Time.timeScale = 1;
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitButton(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // 3 second breather before winning screen, cannot win if you trade lives on the last blow
    IEnumerator wonGame()
    {   if(!isTutorial){
        yield return new WaitForSeconds(3f);
        if(!ded)
        gameWon(); }
    }

    IEnumerator lostGame()
    {   
        ded = true;
        GameObject.Find("Player").GetComponent<PlayerController>().enabled = false;
        yield return new WaitForSeconds(1f);
        saintSeiya.Play();

        yield return new WaitForSeconds(2f);
        gameOver();
   

    }

    private IEnumerator SlowdownTime()
    {
        float currentTimeScale = Time.timeScale;
        float timeElapsed = 0f;

        while (timeElapsed < slowdownDuration)
        {
            Time.timeScale = Mathf.Lerp(currentTimeScale, slowdownFactor, timeElapsed / slowdownDuration);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        
        Time.timeScale = slowdownFactor;
        yield return new WaitForSecondsRealtime(slowdownDuration);

        Time.timeScale = 1f;
    }
}
