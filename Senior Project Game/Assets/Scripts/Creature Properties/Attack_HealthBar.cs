using UnityEngine;

namespace Cyrcadian
{
    public class Attack_HealthBar : MonoBehaviour
    {
        [SerializeField] public AudioClip SFX;
        public int damage = 1;

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
                        AudioManager.Instance.PlaySoundFX(SFX);
                }
                    break;

                default:
                    break;
            }
        }
    }    
}

