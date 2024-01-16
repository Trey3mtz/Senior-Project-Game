using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Cyrcadian
{
    public class Mineable_Resource : MonoBehaviour
    {

        //                                IMPORTANT NOTE: If you want to animate a resource that can be "Mined" with a tool,
        //                                                animation state name "Hit" must be used in all their animatorcontrollers.
        //                                                Whether its a tree, a rock, etc they all must share this name.

        //                                                This should be applicable to any static thing that drops resources.

        [SerializeField] Item item;
        [SerializeField] Transform spawnpoint;
        [SerializeField] string correctTool;

        [SerializeField] AudioClip minedSFX;
        private Animator animator;

        void Awake()
        {
            spawnpoint = gameObject.transform;
            animator = GetComponent<Animator>();
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            var randomVariable = Random.insideUnitCircle / 3;
            if(collider.gameObject.tag == correctTool)
            {
                World_Item.SpawnWorldItem(spawnpoint.position + new Vector3(0,.75f)+  randomVariable.ConvertTo<Vector3>(), item);
                AudioManager.Instance.PlaySoundFX(minedSFX);
                animator.CrossFade("Hit", 0);
            }
                
        }
    }
}
