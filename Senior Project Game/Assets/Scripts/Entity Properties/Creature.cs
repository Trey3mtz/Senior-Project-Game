using System.Collections;
using System.Collections.Generic;
using Cyrcadian.UtilityAI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cyrcadian.Creatures
{
    public class Creature : ScriptableObject
    {
        // Here are some behavior types to give to a variety of creatures
        // Even those creatures of the same species can have a different behavior type
        // Maybe a predator will sometimes be skittish
        public enum BehaviorType{
            Skittish,
            Passive,
            Aggressive,
        }


        [Tooltip("Name for this creature and it's behavior")]        
        public string DisplayName;
        public BehaviorType Behavior;

        [Tooltip("Stats")]
        public int HealthPool;
        public int StomachSize;

        [Tooltip("Visual Components that a creature should have")]  
        public SpriteRenderer Sprite;
        public Animator Animator;
        public AnimatorController AnimatorControl;
        public Creature_Animation Animation;

        [Tooltip("The physics of a creature")]
        public Rigidbody2D rb;

        [Tooltip("Survival components of a creature")]
        public HungerBar Hunger;
        public GameObject HealthBar;

        [Tooltip("The AI components of a creature")]
        public AIBrain Brain;
        public EntityController CreatureController;
        public MoveController MoveCreature;

        [Tooltip("This is a generic gameobject prefab, which will hold all of a Creature's components")]
        public GameObject CreaturePrefab;
    }
}
