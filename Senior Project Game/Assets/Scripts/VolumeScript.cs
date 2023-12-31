using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeScript : MonoBehaviour
{   
    [SerializeField] private Volume hurt_volume;
    [SerializeField] private Volume night_volume;
    [SerializeField] Vignette vignette;

    ColorParameter OriginalColor;
    private float intensity;
    public float totalHurtTime = 0.4f;
    public float DayNightTransition = 5f;
    
    void Awake()
    {
        hurt_volume.profile.TryGet(out vignette);
        
        OriginalColor =  vignette.color;
    }

    public void DayBreak()
    {
        StartCoroutine(NightTimeLeaves());
    }

    public void NightFall()
    {
        StartCoroutine(NightTimeArrives());
    }

    public void TakeDamage()
    {
       hurt_volume.weight = 1;
       StartCoroutine(LowerVolumeWeightAfterHit());
    }

    IEnumerator LowerVolumeWeightAfterHit()
    {
        yield return new WaitForSeconds(0.15f);
        float percentageComplete = 0;
        while(hurt_volume.weight > 0)
        {
            hurt_volume.weight = Mathf.Lerp(hurt_volume.weight, 0, percentageComplete);
            percentageComplete += Time.deltaTime / (totalHurtTime);
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
