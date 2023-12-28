using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    // Start is called before the first frame update
   public Animator transition;
   public float transitionTime = 1;


    // starts a coroutine to play a transition into the very next scene in the build order
   public void LoadNextScene(){
        StartCoroutine(LevelLoad(SceneManager.GetActiveScene().buildIndex + 1));
   }

   public void StartGame_TitleScreen(){
        StartCoroutine(StartGame(SceneManager.GetActiveScene().buildIndex + 1));
   }


    IEnumerator LevelLoad(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    IEnumerator StartGame(int levelIndex)
    {
        transition.SetTrigger("TitleStart");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
}
