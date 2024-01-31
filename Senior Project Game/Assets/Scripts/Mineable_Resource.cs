using System.Collections;
using System.Collections.Generic;
using Cyrcadian.Items;
using Unity.VisualScripting;
using UnityEngine;

namespace Cyrcadian
{
    [RequireComponent(typeof(Spawnable_Loot))]
    public class Mineable_Resource : MonoBehaviour
    {

        //                                IMPORTANT NOTE: If you want to animate a resource that can be "Mined" with a tool,
        //                                                animation state name "Hit" must be used in all their animatorcontrollers.
        //                                                Whether its a tree, a rock, etc they all must share this name.

        //                                                This should be applicable to any static thing that drops resources.

        [Tooltip("The GameObject tag of the correct tool to interact. Capitalization and Spacing matter!")]
        [SerializeField] string correctTool;
        [HideInInspector] [SerializeField] Spawnable_Loot spawnLoot;
        [SerializeField] AudioClip minedSFX;
        private Animator animator;

        void Awake()
        {
            spawnLoot = GetComponentInChildren<Spawnable_Loot>();
            animator = GetComponentInChildren<Animator>();
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            var randomVariable = Random.insideUnitCircle / 3;
            if(collider.gameObject.tag == correctTool)
            {
                spawnLoot.SpawnLoot();
                AudioManager.Instance.PlaySoundFX(minedSFX);
                animator.CrossFade("Hit", 0);
            }
                
        }
    }
}
