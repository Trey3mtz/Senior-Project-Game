using System.Collections;
using System.Collections.Generic;
using Cyrcadian.UtilityAI;
using Cyrcadian.UtilityAI.Actions;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace Cyrcadian.Creatures
{
    public class Creature_Animation : MonoBehaviour
    {
        [SerializeField] HealthBar health;
        [SerializeField] AnimatorHandler animatorControl;
        [SerializeField] MoveController mover;
        [SerializeField] CreatureController creature;

        [HideInInspector][SerializeField] Rigidbody2D rb;

        [Header("Animation Timings (seconds)")]
        [SerializeField] private float hurtTime = .25f;
        private bool isAnimationLocked = false;

        //  audio for sfx
        [Header("Audio")]
        [SerializeField] public AudioClip walkSFX;
        [SerializeField] public AudioClip hurtSFX;
        [SerializeField] public AudioClip deathSFX;
        //private bool canPlayFootstepSFX = true;



        
        void Awake()
        {
            // All creatures will have a health bar and rigidbody
            health = GetComponentInChildren<HealthBar>();
            rb = GetComponentInParent<Rigidbody2D>();
            mover = GetComponentInParent<MoveController>();
            creature = GetComponentInParent<CreatureController>();

            // Create and set a control for AnimatorController
            animatorControl = GetComponentInChildren<AnimatorHandler>();
            animatorControl.spriteObject = gameObject;
            animatorControl.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            animatorControl.animator = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            if(isAnimationLocked)
                return;

            if(health.CurrentHP() <= 0)
                Dead();
            else if(health.WasHit())
                Hurt();

            if(isAnimationLocked)
                return;

            if(mover.IsMoving())
            {  
                if(math.abs(rb.velocity.sqrMagnitude) > 0.25f )
                { 
                    animatorControl.OrientateBody(mover.moveDirection.x);
                    animatorControl.CrossFade("Move");  }
            }
            else
            {   
                if(creature.alertness == CreatureController.AlertState.Unconcious)
                    animatorControl.CrossFade("Sleep");
                else if(creature.isEating)
                    animatorControl.CrossFade("Eating");
                else
                    animatorControl.CrossFade("Idle");
            }


        }

        public void Attack()
        {
            isAnimationLocked = true;
            mover.BrieflyPauseMovement(.5f);
            animatorControl.CrossFade("Attack");
            StartCoroutine(animationLockOut(animatorControl.animator.GetCurrentAnimatorClipInfo(0)[0].clip.length));
        }

        public void Hurt()
        {   
            isAnimationLocked = true;
            mover.BrieflyPauseMovement(hurtTime);
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
