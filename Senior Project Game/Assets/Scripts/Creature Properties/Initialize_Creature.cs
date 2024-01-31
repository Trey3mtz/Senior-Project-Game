using System;
using Cyrcadian.Creatures;
using Cyrcadian.UtilityAI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
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
            GameObject spawnedCreature = Instantiate(creature.CreaturePrefab, position, Quaternion.identity);

            spawnedCreature.GetComponent<Initialize_Creature>().SetCreature(creature);
        }


        private Creature creature;
        private SpriteRenderer spriteRenderer;
        private Collider2D hitbox;
        //private Animator animator;
        private Rigidbody2D rb;

        private HealthBar health;
        private HungerBar hunger;
        private AIBrain brain;
        private CreatureController creatureController;
        private MoveController moveController;
        public Creature_Stats MyStats;

        public void SetCreature(Creature newCreature)
        {
            this.creature = newCreature;
            spriteRenderer.sprite = newCreature.Sprite;

            hitbox = newCreature.collider2D;
            rb = newCreature.rb;

            MyStats = newCreature.Stats;
            health.MaxHP = MyStats.healthPool;
            health.SetHealth(MyStats.currentHP);
            hunger.StomachSize = MyStats.stomachSize;
            hunger.SetHunger(MyStats.currentHunger);
        }

        void Awake()
        {
            brain = GetComponentInChildren<AIBrain>();
            creatureController= GetComponentInChildren<CreatureController>();
            moveController = GetComponentInChildren<MoveController>();
            health = GetComponentInChildren<HealthBar>();
            hunger = GetComponentInChildren<HungerBar>();

            creatureController.stats = MyStats;

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
            data.stats = MyStats;
            data.position = transform.position;
        }

        public void Load(CreatureSavedData data)
        {
            MyStats = data.stats;
            transform.position = data.position;

            health.SetHealth(data.stats.currentHP);
            hunger.SetHunger(data.stats.currentHunger);
        }
    }
}
