using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashEffect : MonoBehaviour
{


    public Rigidbody2D rb;
    public float splat_speed = 4f;

    public float lifespan = 2f;
    float randomNum;
    float randomNum2;
    

    // Start is called before the first frame update
    void Start()
    {
       rb = GetComponent<Rigidbody2D>();

         randomNum = Random.Range(-0.45f, 0.45f);
         randomNum2 = Random.Range(-0.35f, 0.35f);

         rb.velocity = new Vector2(randomNum, 1f + randomNum2*randomNum*3 + (randomNum*0.5f)) * splat_speed;
         rb.angularVelocity = (-25 * randomNum2 *10 + randomNum*10) * splat_speed;

        StartCoroutine(Lifespan());
    }

    
    IEnumerator Lifespan()
    {
        Destroy(this.gameObject, (lifespan + randomNum));
        yield return null;
    }


}
