using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurt_entity : MonoBehaviour
{
    public float iFrames = 0.5f;
    [SerializeField] Collider2D hitbox;
    [SerializeField] HealthBar hp;

    public GameObject impactPrefab;

    // On awake, if no hitbox is given find one. Set the HP to be from the gameobject's child component.
    void Awake()
    {
        if(!hitbox)
            hitbox = GetComponent<Collider2D>();
        if(!hp)
        hp = GetComponentInChildren<HealthBar>();
    }

    // Upon colliding with an attack (case 13), find the direction the attack came from.
    // Create a particle effect prefab at that point of the attack connecting.
    // Disable the Entity's hitbox for iFrames seconds, and re-enable shortly after.
    void OnTriggerEnter2D(Collider2D collider)
    {
    
        switch(collider.gameObject.layer)
        {
            case 13:
                    Vector3 direction = collider.transform.position-transform.position;

                    if(!hp.WasHit())
                       break;

                    if(direction.x >= 0)
                        Instantiate(impactPrefab,collider.ClosestPoint(transform.position),Quaternion.Euler(0.0f, 0.0f, 0.0f));
                    else
                        Instantiate(impactPrefab,collider.ClosestPoint(transform.position),Quaternion.Euler(0.0f, -180f, 0.0f));

                    Debug.Log("Turning off Hit box");
                    hitbox.enabled = false;
                    StartCoroutine(Iframes());
                    break;
            default:
                break;
        }

        
    }

    IEnumerator Iframes()
    {
        yield return new WaitForSeconds(iFrames);
            hitbox.enabled = true;
    }

}
