using System;
using Cyrcadian.Creatures;
using Cyrcadian.UtilityAI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cyrcadian
{
    public class Initialize_Creature : MonoBehaviour
    {
        public static event EventHandler<Creature> OnSpawn;
        public static event EventHandler<Creature> OnDespawn;

        public static void SpawnCreature(Vector3 position, Creature creature)
        {
            // Instantiate a blank creature prefab
            GameObject spawnedCreature = Instantiate(creature.CreaturePrefab, position, Quaternion.identity);
            // Fill out that prefab with specified data
            spawnedCreature.GetComponent<Initialize_Creature>().SetCreature(creature);
        }


       
        private SpriteRenderer spriteRenderer;
        [SerializeField] Collider2D hitbox;
        private Animator animator;
        private Rigidbody2D rb;

        private HealthBar health;
        private HungerBar hunger;
        private AIBrain brain;
        private CreatureController creatureController;
        private MoveController moveController;
        private Creature_Stats initialStats;
        private Creature_Animation animationHandler;

        public void SetCreature(Creature creature)
        {
            creatureController.creatureSpecies = creature;
            creatureController.actionsAvailable = creature.ListOfPossibleActions;
            spriteRenderer.sprite = creature.Sprite;
            animator.runtimeAnimatorController = creature.AnimatorController;

            hitbox = creature.collider2D;
            rb = creature.rb;

            initialStats = creature.Stats;
            health.MaxHP = initialStats.healthPool;
            health.SetHealth(initialStats.healthPool);
            hunger.StomachSize = initialStats.stomachSize;
            hunger.SetHunger(initialStats.stomachSize);


            // Creatures will have a set list of sounds no matter what species.
            // What sounds those are, are determined by the game asset of that creature.
            animationHandler.walkSFX = creature.CreatureSounds[0];
            animationHandler.hurtSFX = creature.CreatureSounds[1];
            animationHandler.deathSFX = creature.CreatureSounds[2];
        }

        void Awake()
        {
            brain = GetComponentInChildren<AIBrain>();
            creatureController= GetComponentInChildren<CreatureController>();
            moveController = GetComponentInChildren<MoveController>();
            health = GetComponentInChildren<HealthBar>();
            hunger = GetComponentInChildren<HungerBar>();
            animationHandler = GetComponentInChildren<Creature_Animation>();
            animator = GetComponentInChildren<Animator>();

            rb = transform.root.GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

            creatureController.stats = initialStats;

            OnSpawn?.Invoke(this, new Creature() );
        }

        void OnDestroy()
        {
            OnDespawn?.Invoke(this, new Creature());
        }


        


        /// <summary>
        ///                 Save / Load system:
        ///                             
        ///                             Below is the save and load system for creature's so that they can persist past opening and closing a game.
        ///                             Will be saving their stats, and their world position.
        ///                             
        ///                             A static class in charge of monitoring all spawned creatures, and the spawning of them, will hold an array.
        ///                             An array of CreaturePrefab GameObjects instanciated, so it can save and load each creature. It will call
        ///                             These methods below to do that.
        /// </summary>

        public struct CreatureSavedData
        {
            public Creature_Stats stats;
            public Vector3 position;
        }

        public void Save(ref CreatureSavedData data)
        {
            data.stats = initialStats;
            data.position = transform.position;
        }

        public void Load(CreatureSavedData data)
        {
            initialStats = data.stats;
            transform.position = data.position;

            health.SetHealth(data.stats.currentHP);
            hunger.SetHunger(data.stats.currentHunger);
        }
    }
}
