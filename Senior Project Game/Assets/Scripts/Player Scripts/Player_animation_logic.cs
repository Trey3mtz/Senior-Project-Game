using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player_animation_logic : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] AnimatorController animatorController;
    [SerializeField] SpriteRenderer hands;

    //  audio for sfx
    [SerializeField] AudioSource walkSFX;
    [SerializeField] AudioSource pushpullSFX;
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
                    
                    // If grabbing do a push or drag animation, else walk normally
                    if(!playerController.isGrabbing)
                    {
                        animatorController.OrientateBody(playerController.playerControls.Player.Move.ReadValue<Vector2>().x);

                        if(playerController.playerControls.Player.Move.ReadValue<Vector2>().y > 0)
                            animatorController.CrossFade("Player Walk Up");
                        else
                            animatorController.CrossFade("Player Walk");
                    }
                    else if(playerController.isGrabbing)
                    {
                        if(animatorController.isFacingRight && playerController.playerControls.Player.Move.ReadValue<Vector2>().x > 0)
                            animatorController.CrossFade("Player Push");
                        else if(!animatorController.isFacingRight && playerController.playerControls.Player.Move.ReadValue<Vector2>().x < 0)
                            animatorController.CrossFade("Player Push");
                        else
                            animatorController.CrossFade("Player Drag");
                    }
                        
                    if(playerController.isGrabbing)
                        StartCoroutine(playSound(pushpullSFX));
                    else    
                        StartCoroutine(playSound(walkSFX));
                }
                else if(playerController.isGrabbing)
                    animatorController.CrossFade("Player Hold-Idle");
                else
                    animatorController.CrossFade("Idle");
            

     
        if(playerController.isGrabbing)
            hands.enabled = true;
        else if(!playerController.isGrabbing)
            hands.enabled = false;

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
