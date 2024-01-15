using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_HealthBar : MonoBehaviour
{
    [SerializeField] AudioSource SFX;
    public int damage = 1;
        
    // Initialize the sound of the attack.
    void Start()
    {
        SFX = GetComponent<AudioSource>();
    }

    // Upon attack landing on an entity, check for it's healthbar. 
    // Lower it by damage amount, and play attack landing soundFX.
    // This script should be attached to anything Layered as an attack (case 13).
    void OnTriggerEnter2D(Collider2D collider)
    {
        // case 10 is any entity.
        switch(collider.gameObject.layer)
        {
           case 10:
           
            if(collider.gameObject.GetComponentInChildren<HealthBar>() != null)
            { 
                collider.GetComponentInChildren<HealthBar>().ChangeHealth(-damage);

                if(SFX != null)
                    SFX.Play();
            }
                break;

            default:
                break;
        }
    }
}
