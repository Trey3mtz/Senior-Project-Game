using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.Creatures
{
    public class Creature_Animation : MonoBehaviour
    {
        [SerializeField] HealthBar health;
        [SerializeField] AnimatorController animatorControl;

        [HideInInspector][SerializeField] Rigidbody2D rb;

        [Header("Animation Timings (seconds)")]
        [SerializeField] private float hurtTime;
        private bool isAnimationLocked = false;

        //  audio for sfx
        [Header("Audio")]
        [SerializeField] AudioClip walkSFX;
        [SerializeField] AudioClip hurtSFX;
        [SerializeField] AudioClip deathSFX;
        private bool canPlayFootstepSFX = true;



        
        void Awake()
        {
            // All creatures will have a health bar and rigidbody
            health = GetComponentInChildren<HealthBar>();
            rb = GetComponentInParent<Rigidbody2D>();

            // Create and set a control for AnimatorController
            animatorControl = GetComponentInChildren<AnimatorController>();
            animatorControl.spriteObject = gameObject;
            animatorControl.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animatorControl.animator = GetComponentInChildren<Animator>();
        }

        
        void Update()
        {
            if(isAnimationLocked)
                return;
            
                animatorControl.CrossFade("Idle");


        if(health.CurrentHP() <= 0)
            Dead();
        else if(health.WasHit())
            Hurt();
        }

        public void Hurt()
        {   
            isAnimationLocked = true;
            animatorControl.CrossFade("Hurt");
            AudioManager.Instance.PlaySoundFX(hurtSFX);
            StartCoroutine(animationLockOut(hurtTime));
        }

        public void Dead()
        {
            isAnimationLocked = true;
            animatorControl.CrossFade("Death");
            AudioManager.Instance.PlaySoundFX(hurtSFX);
            AudioManager.Instance.PlaySoundFX(deathSFX);
        }

        private IEnumerator animationLockOut(float time)
        {
            yield return new WaitForSeconds(time);
            isAnimationLocked = false;
        }  
    }
}
