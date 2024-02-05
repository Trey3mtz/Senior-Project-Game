using System.Collections;
using System.Collections.Generic;
using UnityEditor.Media;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeScript : MonoBehaviour
{   
    [SerializeField] private Volume hurt_volume;
    [SerializeField] private Volume night_volume;

    [SerializeField] Animator animator;
    [SerializeField] private AnimatorHandler animController;
    [SerializeField] private SpriteRenderer placeholderSprite;

    public float DayNightTransition = 5f;


    public void DayBreak()
    {   StartCoroutine(NightTimeLeaves());      }

    public void NightFall()
    {   StartCoroutine(NightTimeArrives());     }
 
    public void TakeDamage()
    {   animController.CrossFade("Volume Hurt");}
        
    // This coroutine is for lowering specific volumes slowly to zero over a period of time
    IEnumerator LowerVolumeWeightAfterHit(Volume volumeBeingLowered, float transitionTime)
    {   
        yield return new WaitForSeconds(0.15f);
        float percentageComplete = 0;
        while(volumeBeingLowered.weight > 0)
        {
            volumeBeingLowered.weight = Mathf.Lerp(volumeBeingLowered.weight, 0, percentageComplete);
            percentageComplete += Time.deltaTime / (transitionTime);
            yield return null;
        } 
    }
    
        //Under construction
        IEnumerator NightTimeArrives()
    {
       
        float percentageComplete = 0;
        while(night_volume.weight < 1)
        {
            night_volume.weight = Mathf.Lerp(night_volume.weight, 1, percentageComplete);
            percentageComplete += Time.deltaTime / (DayNightTransition);
            yield return null;
        }
        
    }
        //Under construction
        IEnumerator NightTimeLeaves()
    {

        float percentageComplete = 0;
        while(night_volume.weight > 0)
        {
            night_volume.weight = Mathf.Lerp(night_volume.weight, 0, percentageComplete);
            percentageComplete += Time.deltaTime / (DayNightTransition);
            yield return null;
        }
        
    }

}
