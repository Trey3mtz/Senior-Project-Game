using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eye_blink : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public float blink_interval = 5;

    public int double_blink_counter = 3;
    public int counter;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        counter = double_blink_counter;

        StartCoroutine(WaitForBlink());
    }

     IEnumerator WaitForBlink()
    {
        // Generate new seed each blink
        Random.InitState(System.DateTime.Now.Millisecond);
        // Wait for a random time between 5 and 9 seconds
        yield return new WaitForSeconds(Random.Range(blink_interval, blink_interval + 4f));


        if(counter <= 0)
             StartCoroutine(DoubleBlink());
        else
            StartCoroutine(ActivateBlink());

        counter--;    
    }

     IEnumerator ActivateBlink()
    {
        spriteRenderer.enabled = true;
        
        // blink lasts 0.05 seconds
        yield return new WaitForSeconds(0.05f);

        spriteRenderer.enabled = false;

        StartCoroutine(WaitForBlink());
    }

    IEnumerator DoubleBlink()
    {   
        counter = Random.Range(double_blink_counter, double_blink_counter +2);

        spriteRenderer.enabled = true;
        
        yield return new WaitForSeconds(0.05f);

        spriteRenderer.enabled = false;
 
        yield return new WaitForSeconds(0.05f);

        spriteRenderer.enabled = true;

        yield return new WaitForSeconds(0.05f);

        spriteRenderer.enabled = false;

         StartCoroutine(WaitForBlink());
    }
}