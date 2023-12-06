using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ExplosionScript : MonoBehaviour
{
    Animator animator;
    private CinemachineImpulseSource boom;

    public float lifespan = 0.42f;
    public float damageWindow = 0.1f;
    
    public int Damage = 15;
    private bool canHurt = true;

    private List<Collider2D> _hitList = new List<Collider2D>();

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<AudioSource>().Play();
        animator = this.gameObject.GetComponent<Animator>();
        boom = GetComponent<CinemachineImpulseSource>();

        animator.CrossFade("Explosion",0);
        boom.GenerateImpulse();
        StartCoroutine(LifeSpan());
        StartCoroutine(DamageWindow(damageWindow));
    
    }   

    void OnTriggerEnter2D(Collider2D collider)
    {
            // Do they have a healthbar? Is it within the damage window? Are they on the list of colliders you already hit? 
            if(collider.gameObject.GetComponentInChildren<HealthBar>() != null && canHurt && !_hitList.Contains(collider))
            {
                // Damage their healthbar and add them to the list of colliders already hit
                collider.gameObject.GetComponentInChildren<HealthBar>().ChangeHealth(-Damage);
                _hitList.Add(collider);
            }
                
                    
    }

    IEnumerator DamageWindow(float timer)
    {
        yield return new WaitForSeconds(timer);
        canHurt = false;
    }

    IEnumerator LifeSpan()
    {
        yield return null;
        Destroy(this.gameObject, lifespan);
        
    }


}
