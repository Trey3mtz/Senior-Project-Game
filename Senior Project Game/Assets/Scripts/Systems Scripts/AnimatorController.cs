using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject spriteObject;
    private float originalScaleX;
    private bool originalFlipX;

    public bool isFacingRight = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        originalFlipX = spriteRenderer.flipX;
        originalScaleX = Mathf.Sign(spriteObject.transform.localScale.x);
    }

    #region Animator parameters
    public void PlayIntegerAnimation(string IntegerAnimation, int num)
    {
        animator.SetInteger(IntegerAnimation, num);
    }

    public void PlayFloatAnimation(string FloatAnimation, float num)
    {
        animator.SetFloat(FloatAnimation, num);
    }

    public void PlayTriggerAnimation(string triggerAnimation)
    {
        animator.SetTrigger(triggerAnimation);  
    }
    public void PlayBoolAnimation(string boolAnimation, bool tf)
    {
        animator.SetBool(boolAnimation, tf);
    }

    #endregion

    #region Direct Script Animations
    // If I wanted to just do a crossfade directly from script, these are the options
    public void CrossFade(string stateName, float transitionTime)
    {   
        animator.CrossFade(stateName, transitionTime);
    }

            public void CrossFade(string stateName)
            {   
                
                animator.CrossFade(stateName, 0,0);
            }

                // Here you pass the hash if you had used stringtohash for performance
            public void CrossFade(int stateHash, float transitionTime)
            {   
                animator.CrossFade(stateHash, transitionTime);
            }

            public void CrossFade(int stateHash)
            {   

                animator.CrossFade(stateHash, 0,0);
            }
            
            
            public void CrossFade(string stateName, float transitionTime, int layer, float offSet)
            {
                animator.CrossFade(stateName, transitionTime, layer, offSet);
            }
    #endregion

    // Sets the layer of the Animator
    public void SetLayer(int layer, float w)
    {
        animator.SetLayerWeight(layer, w);
    }

    // changes the order of the spriterenderer
    public void SetSpriteOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }

        // only flips the sprite's direction, useful for Enemys attack which become inaccurate if you flip more than just the sprite
        public void CheckDirection(float X_Diff)
    {

        if(X_Diff >= 0){
            spriteRenderer.flipX = originalFlipX;
        }
     
        else{
            spriteRenderer.flipX = !originalFlipX;
        }
    }        

        // orientates entire body rather than just the sprite, useful for when child gameObjects need to flip on X axis as well
        public void OrientateBody(float X_Diff)
    {

       if(X_Diff > 0 ){
          // spriteObject.transform.localScale = new Vector3(originalScaleX*Mathf.Abs(spriteObject.transform.localScale.x),spriteObject.transform.localScale.y,1) ;

          if(!isFacingRight)
            spriteObject.transform.Rotate(0,180,0);

            isFacingRight = true;
        }
        else if(X_Diff < 0 ){
          // spriteObject.transform.localScale = new Vector3(-1*originalScaleX*Mathf.Abs(spriteObject.transform.localScale.x),spriteObject.transform.localScale.y,1) ;
          if(isFacingRight)
            spriteObject.transform.Rotate(0,180,0);

            isFacingRight = false;
        }
    }

    
        // if the animation in question orientates around a target this flips the Y based on the direction of the x axis between the target and animation
        // made originally for the impact VFX for Player attacks connecting with enemy
        public void FlipOrientation(float X_Diff)
    {

        if(X_Diff >= 0){
                spriteObject.transform.localScale = new Vector3(Mathf.Abs(spriteObject.transform.localScale.x),Mathf.Abs(spriteObject.transform.localScale.y),1) ;
                spriteRenderer.flipX = originalFlipX;
        }
        else{
            spriteObject.transform.localScale = new Vector3(-1*Mathf.Abs(spriteObject.transform.localScale.x),-1*Mathf.Abs(spriteObject.transform.localScale.y),1) ;
      
                spriteRenderer.flipX = !originalFlipX;
        }
            
    }

}