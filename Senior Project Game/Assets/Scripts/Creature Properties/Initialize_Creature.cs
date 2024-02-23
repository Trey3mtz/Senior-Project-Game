using System;
using AnimationCurveManipulationTool;
using Cyrcadian.Creatures;
using Cyrcadian.UtilityAI;
using Unity.Assertions;
using UnityEngine;

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
            if(creature.MatOverride)
                 spriteRenderer.material = creature.MatOverride;
            animator.runtimeAnimatorController = creature.AnimatorController;

            // If we want a Random Behavior at birth, pick a random number. (Random.Range is MaxEXCLUSIVE so it does not include the last Behavior, which is Random Behavior)
            if(creature.Behavior == Creature.BehaviorType.RandomizeAtBirth)
            {
                int totalAmountOfBehaviors = Enum.GetNames(typeof(Creature.BehaviorType)).Length;
                int randomBehavior = UnityEngine.Random.Range( 0, totalAmountOfBehaviors);
                creatureController.behavior = (Creature.BehaviorType)randomBehavior;
            }
            else
                creatureController.behavior = creature.Behavior;


            hitbox = creature.collider2D;
            rb = creature.rb;
            
            // Create all of our attacks as children
            foreach(Attack attack in creature.ListOfPossibleAttacks)
            {   
                Assert.IsTrue(attack != null);
                GameObject myAttack = Instantiate(attack.AttackPrefab, transform.Find("Body").Find("Mouth").transform);
                myAttack.name = attack.name;
                myAttack.SetActive(false);
            }


            // Randomize a size for creatures to add variety
            float sizeFactor = 1;
            if(UnityEngine.Random.Range(0,3) == 0)
            {   sizeFactor = UnityEngine.Random.Range(0.9f, 1.1f);
                gameObject.transform.localScale *= sizeFactor;  }

            initialStats = creature.Stats;

                initialStats.healthPool = (int)(initialStats.healthPool * sizeFactor);
                initialStats.stomachSize = (int)(initialStats.stomachSize * sizeFactor);

                health.MaxHP = initialStats.healthPool;
                initialStats.currentHP = initialStats.healthPool;            
                health.SetHealth(initialStats.currentHP);

                hunger.StomachSize = initialStats.stomachSize;
                initialStats.currentHunger = initialStats.stomachSize;
                hunger.SetHunger(initialStats.currentHunger);

                initialStats.currentStamina = initialStats.staminaPool;
                initialStats.proteinScore = creature.GetFoodScore();

            // Adjust shadow to look better for different sized creatures
            transform.Find("Body").Find("Shadow").position += new Vector3(0,creature.ShadowHeightAdjust);
            transform.Find("Body").Find("Shadow").localScale = new Vector3(1.5f + creature.ShadowLengthAdjust,1,1);

            // Lets us test without sounds
            if(creature.CreatureSounds.Length == 0)
                return;
            // Creatures will have a set list of sounds no matter what species.
            // What sounds those are, are determined by the game asset of that creature.
            animationHandler.walkSFX = creature.CreatureSounds[0];
            animationHandler.hurtSFX = creature.CreatureSounds[1];
            animationHandler.deathSFX = creature.CreatureSounds[2];
        }

        void Awake()
        {
            brain = GetComponentInChildren<AIBrain>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            creatureController= GetComponentInChildren<CreatureController>();
            moveController = GetComponentInChildren<MoveController>();
            health = GetComponentInChildren<HealthBar>();
            hunger = GetComponentInChildren<HungerBar>();
            animationHandler = GetComponentInChildren<Creature_Animation>();
            animator = GetComponentInChildren<Animator>();

            rb = transform.root.GetComponent<Rigidbody2D>();

            OnSpawn?.Invoke(this, new Creature() );
        }

        void Start()
        {
            creatureController.stats = initialStats;
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
