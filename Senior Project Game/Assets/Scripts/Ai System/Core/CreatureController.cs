using System.Collections;
using Cyrcadian.Creatures;
using Cyrcadian.WorldTime;
using Cyrcadian.Items;
using UnityEngine;
using DG.Tweening;


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
        [HideInInspector] [SerializeField] Awareness awareness;

        // Particles and sfx all creatures share
        [SerializeField] AudioClip poofSFX;
        private ParticleSystem sleepParticle;
        private ParticleSystem poofParticle;
        private Animator poofAnimator;

        [HideInInspector] public HungerBar hungerBar;
        [HideInInspector] public HealthBar health;
        

        public enum AlertState{
            Asleep,
            Awake,
            Alert
        }

        public AlertState alertness;

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
        }

        // If the brain had finished choosing a best action, Execute that action.
        // Feeds "this" specific entity into the execute method.
        void Update()
        {
            if(isDying)
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
        {   Debug.Log("I am idle now");
            float counter = time;
            while(counter > 0)
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
                    float counter = 4;
                    while(counter > 0)
                    {
                        yield return new WaitForSeconds(1f);
                        counter--;
                    }
                    UponCompletedAction();
                }


        public void DoEat(float time)
        {   StartCoroutine(EatCoroutine(time));     }

        IEnumerator EatCoroutine(float time)
        {
            float counter = time;
            while(counter > 0)
            {
                yield return new WaitForSeconds(1f);
                counter--;
            }
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


                public void DoSleep()
                {    StartCoroutine(SleepCoroutine());    }
                
                IEnumerator SleepCoroutine()
                {       
                    Day_Cycle DayCycle = FindAnyObjectByType<Day_Cycle>();

                    alertness = AlertState.Asleep;
                    sleepParticle.Play();
                    awarenessRange.enabled = false;
                    
                    switch(creatureSpecies.CircadianRhythm) 
                    {
                        case Creature.CyrcadianRhythm.Nocturnal:
                            while(DayCycle.GetTimeOfDay() != 0 && alertness == AlertState.Asleep)
                            {    yield return new WaitForEndOfFrame();    }
                            break;
                        case Creature.CyrcadianRhythm.Diurnal:
                            while(DayCycle.GetTimeOfDay() != 1 && alertness == AlertState.Asleep)
                            {    yield return new WaitForEndOfFrame();    }
                            break;
                        case Creature.CyrcadianRhythm.Crepuscular:
                            while(DayCycle.GetTimeOfDay() != 2 && alertness == AlertState.Asleep)
                            {    yield return new WaitForEndOfFrame();    }
                            break;
                        case Creature.CyrcadianRhythm.Cathemeral:
                            {    yield return new WaitForEndOfFrame();    }
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
            mover.IncreaseMoveSpeed(.1f);
            mover.IncreaseAcceleration(.1f);
            while(awareness.IsTargetInVision())
            {
                Vector3 fleeDirection = (transform.position - awareness.Target.position).normalized;
                mover.MoveTo((fleeDirection * 4) + transform.position);
                yield return new WaitForSeconds(1f);
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
            
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponentInChildren<Collider2D>().enabled = false;

            yield return new WaitForSeconds(1.1f);

            Destroy(transform.root.gameObject);
        }

    }
}

