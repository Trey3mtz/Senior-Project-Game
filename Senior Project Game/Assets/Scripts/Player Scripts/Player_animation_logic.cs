using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Cyrcadian.PlayerSystems
{

public class Player_animation_logic : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] AnimatorController animatorController;
    [SerializeField] SpriteRenderer hands;
    [SerializeField] private HealthBar health;
    [SerializeField] private PlayerControls controls;

    //  audio for sfx
    [Header("Audio")]
    [SerializeField] AudioSource walkSFX;
    [SerializeField] AudioSource pushpullSFX;
    [SerializeField] AudioSource hurtSFX;
    [SerializeField] AudioSource deathSFX;
    private AudioSource currentsound;

    [Header("Animation Timings (seconds)")]
    [SerializeField] private float hurtTime;
    private bool isAnimationLocked = false;

    public UnityEvent Hurt;
    public UnityEvent Death;

    // Start is called before the first frame update
    void Start()
    {
        if(playerController == null)
            playerController = gameObject.GetComponentInParent<PlayerController>();

        animatorController = gameObject.GetComponent<AnimatorController>();
        health = GetComponentInChildren<HealthBar>();
        controls = playerController.playerControls;
    }

    // Update is called once per frame
    void Update()
    {
        // If game is paused or the play is hurt, do not follow through this script's update
        if(playerController.gameStateManager.isPaused || isAnimationLocked)
            return;
        else

                // Faces the correct direction on the x-axis, and animates moving.
                if(playerController.playerControls.Player.Move.IsInProgress())
                {    
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
                    // Play sfx of the pull, pull, or walk animation    
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


        if(health._hp <= 0)
            DeadPlayer();
        else if(health.wasHit())
            HurtPlayer();
        

    }

    public void HurtPlayer()
    {
        isAnimationLocked = true;
        animatorController.CrossFade("Player Hurt");
        StartCoroutine(playSound(hurtSFX));
        StartCoroutine(animationLockOut(hurtTime));
        StartCoroutine(controlsLockOut(hurtTime));
        Hurt.Invoke();
    }

    public void DeadPlayer()
    {
        isAnimationLocked = true;
        controls.Player.Disable();
        animatorController.CrossFade("Player Death");
        StartCoroutine(playSound(hurtSFX));
        StartCoroutine(playSound(deathSFX));
        Death.Invoke();
    }


    // Controlls what soundFX is playing
    private IEnumerator playSound(AudioSource sfx)
    {       
        if(!sfx.isPlaying){
            sfx.Play();
            currentsound = sfx;
        }

        yield return new WaitForSeconds(.2f);
        currentsound = sfx;
    }   

    private IEnumerator animationLockOut(float time)
    {
        yield return new WaitForSeconds(time);
        isAnimationLocked = false;
    }

    private IEnumerator controlsLockOut(float time)
    {
        controls.Player.Disable();
        yield return new WaitForSeconds(time);
        controls.Player.Enable();
    }    
}
}