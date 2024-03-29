using System.Collections;
using Cyrcadian.Creatures;
using Cyrcadian.WorldTime;
using Cyrcadian.Items;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using NavMeshPlus.Extensions;
using UnityEditor.Callbacks;


namespace Cyrcadian.UtilityAI
{

    public class CreatureController : MonoBehaviour
    {

        public MoveController mover{ get; set;}
        public AIBrain aiBrain { get; set;}
        public Action[] actionsAvailable;

        public Creature_Stats stats;
        public Creature creatureSpecies;
        Spawnable_Loot lootTable;

        // Keeps track of nearby Creatures, and Items so far.
        [SerializeField] Collider2D awarenessRange;
        [HideInInspector] public Awareness awareness;

        // Animals will know the time of day, in order to wake up
        private Day_Cycle DayCycle;

        // Particles and sfx all creatures share
        [SerializeField] AudioClip poofSFX;
        private ParticleSystem sleepParticle;
        private ParticleSystem poofParticle;
        private Animator poofAnimator;
        private Transform myShadow;

        [HideInInspector] public HungerBar hungerBar;
        [HideInInspector] public HealthBar health;
        

        public enum AlertState{
            Asleep,
            Awake,
            Alert
        }

        public AlertState alertness;
        public bool isEating;

        // Start is called before the first frame update

        void Awake()
        {
            mover = GetComponent<MoveController>();
            aiBrain = GetComponent<AIBrain>();
            awareness = GetComponentInChildren<Awareness>();
            awarenessRange = awareness.GetComponent<Collider2D>();
            lootTable = GetComponentInChildren<Spawnable_Loot>();

            health = GetComponentInChildren<HealthBar>();
            hungerBar = GetComponentInChildren<HungerBar>();

            sleepParticle = GetComponentInChildren<ParticleSystem>();
            poofParticle = lootTable.GetComponentInChildren<ParticleSystem>();     
            poofAnimator = lootTable.GetComponentInChildren<Animator>();
        }

        void Start()
        {
            stats = creatureSpecies.Stats;
            lootTable.spawnableLoot = creatureSpecies.LootTable;
            alertness = AlertState.Awake;
            myShadow = transform.Find("Body").Find("Shadow");
            DayCycle  = FindAnyObjectByType<Day_Cycle>();
        }

        // If the brain had finished choosing a best action, Execute that action.
        // Feeds "this" specific entity into the execute method.
        void Update()
        {
            if(isDying || GameStateManager.IsPaused())
                return;

            if(aiBrain.isFinishedDeciding)
            {
                aiBrain.isFinishedDeciding = false;
                aiBrain.bestAction.Execute(this);
            }

            // Always keep our stats updated
            stats.currentHP = health.CurrentHP();
            stats.currentHunger = hungerBar.CurrentHunger();

            if(health.wasRecentlyAttacked)
                alertness = AlertState.Alert;


            if(health.CurrentHP() <= 0)
                StartCoroutine(DEATH());
        }

        // Upon completing an Action, choose the next best action from all available actions
        public void UponCompletedAction()
        {    aiBrain.ChooseBestAction(actionsAvailable);    }
        
    

        #region Coroutine
        /*****************************************************************************************************************
            This region contains the code and logic for all actions that will take Real Time to complete. 
            This gives room for animations to play.
            This also makes sense from a realism perspective.
        */

        public void DoIdle(float time)
        {   StartCoroutine(IdleCoroutine(time));     }

        IEnumerator IdleCoroutine(float time)
        {   
            float counter = time;
            while(counter > 0 && alertness != AlertState.Alert)
            {
                yield return new WaitForSeconds(2f);
                
                stats.currentStamina += 1;
                if(stats.currentStamina > stats.staminaPool)
                    stats.currentStamina = stats.staminaPool;

                counter--;
            }
            UponCompletedAction();
        }

                public void DoGraze()
                {   StartCoroutine(GrazeCoroutine());     }

                IEnumerator GrazeCoroutine()
                {
                    DoRandomRoam();

                    while(mover.agent.velocity.sqrMagnitude != 0)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    // Grass eating holds a static food value of 5 for now
                    DoEat(1f, 5);

                    yield return new WaitForEndOfFrame();
                    
                    UponCompletedAction();
                }

        
        public void DoEat(float time, int foodValue)
        {   StartCoroutine(EatCoroutine(time, foodValue));     }

        IEnumerator EatCoroutine(float time, int foodValue)
        {
            isEating = true;
            yield return new WaitForSeconds(time);
            hungerBar.ChangeHunger(foodValue);
            isEating = false;

            UponCompletedAction();
        }


                public void DoAlert()
                {   StartCoroutine(AlertCoroutine());}

                IEnumerator AlertCoroutine()
                {  
                    yield return new WaitForEndOfFrame();
                    
                    alertness = AlertState.Alert;
                    awarenessRange.enabled = true;
                                          
                            //   IDEAS:   - Check if successfully escaped threat                                                                       
                            //            - check if anything else threatening is nearby, if not wait a few seconds, then become calm                                      

                    UponCompletedAction();
                }


        public void DoRandomRoam()
        {   StartCoroutine(RandomRoam());   }

        IEnumerator RandomRoam()
        {
            mover.MoveToRandomPoint(awarenessRange.bounds.extents.x * 0.5f);
            
            // While still traveling a path, wait before deciding
            while(mover.agent.remainingDistance > mover.agent.stoppingDistance)
            {
                yield return new WaitForEndOfFrame();
            }

            UponCompletedAction();
        }


                public void DoSleep()
                {    StartCoroutine(SleepCoroutine());    }
                
                IEnumerator SleepCoroutine()
                {       

                    alertness = AlertState.Asleep;
                    sleepParticle.Play();
                    awarenessRange.enabled = false;
                    
                    switch(creatureSpecies.CircadianRhythm) 
                    {
                        case Creature.CyrcadianRhythm.Nocturnal:
                            while(DayCycle.GetTimeOfDay() != 0 && alertness == AlertState.Asleep)
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.1f) break;   }
                            break;
                        case Creature.CyrcadianRhythm.Diurnal:
                            while(DayCycle.GetTimeOfDay() != 1 && alertness == AlertState.Asleep)
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.1f) break;    }
                            break;
                        case Creature.CyrcadianRhythm.Crepuscular:
                            while(DayCycle.GetTimeOfDay() != 2 && alertness == AlertState.Asleep)
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.1f) break;    }
                            break;
                        case Creature.CyrcadianRhythm.Cathemeral:
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.1f) break;    }
                            break;
                        default:
                            break;
                    }
                   
                    if(alertness == AlertState.Asleep)
                        alertness = AlertState.Awake;

                    stats.currentStamina = stats.staminaPool;
                    sleepParticle.Stop();
                    awarenessRange.enabled = true;
                    
                    UponCompletedAction();
                }


        public void DoFindFood()
        {    StartCoroutine(FindFoodCoroutine(3));    }

        IEnumerator FindFoodCoroutine(float time)
        {
            float counter = time;
            while(counter > 0)
            {
                yield return new WaitForSeconds(1f);
                counter--;
            }
            UponCompletedAction();
        }


                public void DoChase()
                {   StartCoroutine(ChaseCoroutine());    }

                IEnumerator ChaseCoroutine()
                {
                    mover.IncreaseMoveSpeed(.2f);

                    while(mover.agent.hasPath)
                    {
                        yield return new WaitForEndOfFrame();
                        mover.MoveTo(mover.agent.nextPosition);
                    }

                    mover.ResetSpeed();
                    UponCompletedAction();
                }


        public void DoFlee()
        {   StartCoroutine(FleeCoroutine());    }

        IEnumerator FleeCoroutine()
        {
            if(stats.currentStamina > stats.staminaPool * 0.5f)
            {
                mover.IncreaseMoveSpeed(.1f);
                mover.IncreaseAcceleration(.1f);
            }

            while(awareness.IsThreatNearby())
            {
                Vector3 fleeDirection = (transform.position - awareness.NearestThreat().position).normalized;
                mover.MoveTo((fleeDirection * 4) + transform.position);

                yield return new WaitForSeconds(0.25f);

                if(stats.currentStamina > 0)
                    stats.currentStamina -= 2;
                else
                    stats.currentStamina = 0;

                mover.DrainingStamina(this);
            }

            // By here we have successfully flee-d from danger
            alertness = AlertState.Awake;
            mover.agent.ResetPath();
            mover.ResetSpeed();
            mover.ResetAcceleration();
            UponCompletedAction();
        }


        #endregion



        public bool isDying = false;
        IEnumerator DEATH()
        {
            isDying = true;
            mover.agent.isStopped = true;

            yield return new WaitForSeconds(.8f);
            
            poofAnimator.CrossFade("PoofAnimation 1",0);
            poofParticle.Play();
            AudioManager.Instance.PlaySoundFX(poofSFX);
            lootTable.SpawnLoot();

            myShadow.gameObject.SetActive(false);
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponentInChildren<Collider2D>().enabled = false;

            yield return new WaitForSeconds(1.1f);

            Destroy(transform.root.gameObject);
        }

    }
}

