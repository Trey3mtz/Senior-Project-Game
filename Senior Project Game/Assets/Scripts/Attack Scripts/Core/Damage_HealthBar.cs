using Cyrcadian.Creatures;
using Cyrcadian.UtilityAI;
using UnityEngine;

namespace Cyrcadian.AttackSystem
{
    public class Damage_HealthBar : MonoBehaviour
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

                    if(SFX != null && collider.GetComponentInChildren<HealthBar>().WasHit())
                        AudioManager.Instance.PlaySoundFX(SFX);

                    if(collider.transform.root.GetComponentInChildren<Awareness>())
                    {
                        Awareness tempAwareness = collider.transform.root.GetComponentInChildren<Awareness>();
                        tempAwareness.AddTargetedCreature(transform.root, true, damage);
                      
                        if(!tempAwareness.VisibleCreatures.Contains(transform.root))
                            tempAwareness.VisibleCreatures.Add(transform.root);
                    }
                }
                    break;

                default:
                    break;
            }
        }
    }    
}

