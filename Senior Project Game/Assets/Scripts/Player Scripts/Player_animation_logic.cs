using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
using TMPro;
using UnityEditor.Build.Pipeline;
using UnityEditor.Timeline;

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
    [SerializeField] AudioClip walkSFX;
    [SerializeField] AudioClip draggingSFX;
    [SerializeField] AudioClip hurtSFX;
    [SerializeField] AudioClip deathSFX;
    private string currentSoundFX;
    private bool canPlayFootstepSFX = true;
    private bool canPlayDragSFX = true;
 
    [Header("The GameObject which will be the tool used")]
    [SerializeField] public GameObject tool;

    [Header("Animation Timings (seconds)")]
    [SerializeField] private float hurtTime;
    private bool isAnimationLocked = false;

    public UnityEvent Hurt;
    public UnityEvent Death;
    public UnityEvent PostDeath;

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
                        StartCoroutine(dragSoundFX(draggingSFX));
                    else    
                        StartCoroutine(footstepSoundFX(walkSFX));
                }
                else if(playerController.isGrabbing)
                    animatorController.CrossFade("Player Hold-Idle");
                else
                    animatorController.CrossFade("Idle");
            

     
        if(playerController.isGrabbing)
            hands.enabled = true;
        else if(!playerController.isGrabbing)
            hands.enabled = false;


        if(health.CurrentHP() <= 0)
            DeadPlayer();
        else if(health.WasHit())
            HurtPlayer();
        

    }

    public void HurtPlayer()
    {   
        isAnimationLocked = true;
        animatorController.CrossFade("Player Hurt");
        AudioManager.Instance.PlaySoundFX(hurtSFX);
        StartCoroutine(animationLockOut(hurtTime));
        StartCoroutine(controlsLockOut(hurtTime));
        Hurt.Invoke();
    }

    public void DeadPlayer()
    {
        isAnimationLocked = true;
        controls.Player.Disable();
        animatorController.CrossFade("Player Death");
        AudioManager.Instance.PlaySoundFX(hurtSFX);
        AudioManager.Instance.PlaySoundFX(deathSFX);
        StartCoroutine(postDeathEvents());
        Hurt.Invoke();
        Death.Invoke();
    }

    public void GenericToolSwing()
    {
        isAnimationLocked = true;
        animatorController.CrossFade("Generic Tool Swing");
        StartCoroutine(animationLockOut(.3f));
        StartCoroutine(controlsLockOut(.3f));
    }

    public void Nom()
    {
        isAnimationLocked = true;
        animatorController.CrossFade("Player Eat");
        StartCoroutine(animationLockOut(.25f));
        StartCoroutine(controlsLockOut(.25f));
    }

    // For audioclips that will get called in the Update() method
    private IEnumerator footstepSoundFX(AudioClip clip)
    {
        if(canPlayFootstepSFX)
        {   
            AudioManager.Instance.PlaySoundFX(clip, 0.5f);
            canPlayFootstepSFX = false;
        }
        else
        {   yield break;    }

        yield return new WaitForSeconds(clip.length * 0.75f);
        canPlayFootstepSFX = true;
    }

    private IEnumerator dragSoundFX(AudioClip clip)
    {
        if(canPlayDragSFX)
        {   
            AudioManager.Instance.PlaySoundFX(clip);
            canPlayDragSFX = false;
        }
        else
        {   yield break;    }

        yield return new WaitForSeconds(clip.length);
        canPlayDragSFX = true;
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

    private IEnumerator postDeathEvents()
    {
        // Waits for death animation to finish, then calls event(s)
        yield return new WaitForSeconds(5);
        PostDeath.Invoke();
    }
}
}