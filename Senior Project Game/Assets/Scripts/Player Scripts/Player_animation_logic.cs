using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_animation_logic : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] AnimatorController animatorController;

    //  audio for sfx
    [SerializeField] AudioSource walkSFX;
    AudioSource currentsound;

    // Start is called before the first frame update
    void Start()
    {
        if(playerController == null)
            playerController = this.gameObject.GetComponentInParent<PlayerController>();

        animatorController = this.gameObject.GetComponent<AnimatorController>();
    }

    // Update is called once per frame
    void Update()
    {
        // If game is paused, do not follow through this script's update
        if(playerController.isPaused)
            return;
        else

        // Faces the correct direction on the x-axis, and animates moving.
        if(playerController.playerControls.Player.Move.IsInProgress()){
        
        // If grabbing don't look the other way
        if(!playerController.isGrabbing)
            animatorController.OrientateBody(playerController.playerControls.Player.Move.ReadValue<Vector2>().x);

            if(playerController.playerControls.Player.Move.ReadValue<Vector2>().y > 0)
                animatorController.PlayIntegerAnimation("Movestate",2);
            else
                animatorController.PlayIntegerAnimation("Movestate",1);

            StartCoroutine(playSound(walkSFX));
        }
        else
          animatorController.PlayIntegerAnimation("Movestate",0);
            




    }


    // Controlls what soundFX is playing
    IEnumerator playSound(AudioSource sfx)
    {       
        if(!sfx.isPlaying){
            sfx.Play();
            currentsound = sfx;
        }

        yield return new WaitForSeconds(.2f);
        currentsound = sfx;
        
    }   
}
