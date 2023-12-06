using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactScript : MonoBehaviour
{
     [SerializeField] AudioSource soundFX;
 
    // Start is called before the first frame update
    void Start()
    {
        soundFX = GetComponent<AudioSource>();
        soundFX.Play();

        Destroy(this.gameObject, 4f);
    }

}
