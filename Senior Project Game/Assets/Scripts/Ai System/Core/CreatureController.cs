using System.Collections;
using Cyrcadian.Creatures;
using Cyrcadian.WorldTime;
using Cyrcadian.Items;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using NavMeshPlus.Extensions;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering.Universal;


namespace Cyrcadian.UtilityAI
{

    public class CreatureController : MonoBehaviour
    {

        public MoveController mover{ get; set;}
        public AIBrain aiBrain { get; set;}
        public Action[] actionsAvailable;

        public Creature_Stats stats;
        public Creature creatureSpecies;
        public Creature.BehaviorType behavior;
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
            Unconcious,
            Calm,
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
            alertness = AlertState.Calm;
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
                mover.BrieflyPauseMove(1.8f);
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

                    while(mover.agent.remainingDistance > mover.agent.stoppingDistance)
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
            mover.BrieflyPauseMove(time);
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

                    alertness = AlertState.Unconcious;
                    sleepParticle.Play();
                    awarenessRange.enabled = false;
                    
                    switch(creatureSpecies.CircadianRhythm) 
                    {
                        case Creature.CyrcadianRhythm.Nocturnal:
                            while(DayCycle.GetTimeOfDay() != 0 && alertness == AlertState.Unconcious)
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.1f) break;   }
                            break;
                        case Creature.CyrcadianRhythm.Diurnal:
                            while(DayCycle.GetTimeOfDay() != 1 && alertness == AlertState.Unconcious)
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.1f) break;    }
                            break;
                        case Creature.CyrcadianRhythm.Crepuscular:
                            while(DayCycle.GetTimeOfDay() != 2 && alertness == AlertState.Unconcious)
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.1f) break;    }
                            break;
                        case Creature.CyrcadianRhythm.Cathemeral:
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.1f) break;    }
                            break;
                        default:
                            break;
                    }
                   
                    if(alertness == AlertState.Unconcious)
                        alertness = AlertState.Calm;

                    stats.currentStamina = stats.staminaPool;
                    sleepParticle.Stop();
                    awarenessRange.enabled = true;
                    
                    UponCompletedAction();
                }

        // For Omnivores (they need to decided on what to eat first)
        public void DoHunt()
        {    StartCoroutine(HuntCoroutine());    }

        IEnumerator HuntCoroutine()
        {
        
            Transform huntingTarget = awareness.FindTastiestCreature(this);
          
            // If found, chase that target
            if(huntingTarget)
            {  Debug.Log("Hunting creature " + huntingTarget.name);
                DoChase(huntingTarget);
                while(huntingTarget.GetComponentInChildren<HealthBar>().CurrentHP()  >  0 && stats.currentStamina > 0 )
                {

                    yield return new WaitForEndOfFrame();
                    
                    // Check first if an attack exists for this creature
                    if(creatureSpecies.PossibleAttacks.Count != 0)
                    {// If I was able to attempt an attack, reduce stamina
                        if(creatureSpecies.PossibleAttacks.Find(x => x.UniqueName.Contains("Pounce")).TryAttacking(this, huntingTarget))
                            stats.currentStamina -= 2;
                    }
                }                
            }

            
            UponCompletedAction();
        }


                public void DoChase(Transform transform)
                {   StartCoroutine(ChaseCoroutine(transform));    }

                IEnumerator ChaseCoroutine(Transform target)
                {
                    mover.IncreaseMoveSpeed(.1f);
                    mover.MoveTo(target.position);

                    // If I have stamina, and they haven't gotten out of range
                    while( stats.currentStamina > 0 && awareness.VisibleCreatures.Contains(target))
                    {
                        // Increase move speed as they get closer?
                        yield return new WaitForEndOfFrame();
                        mover.MoveTo(target.position);
                    }

                    mover.ResetSpeed();
                    UponCompletedAction();      // Questionable... maybe put "Chase" into the helper functions
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

            Vector3 fleeDirection;
            while(awareness.IsThreatNearby())
            {
                fleeDirection = (transform.position - awareness.FindNearestThreat().position).normalized;
                mover.MoveTo((fleeDirection * 4) + transform.position);

                yield return new WaitForSeconds(0.25f);

                if(stats.currentStamina > 0)
                    stats.currentStamina -= 2;
                else
                    stats.currentStamina = 0;

                mover.DrainingStamina(this);
            }

            // By here we have successfully flee-d from danger
            alertness = AlertState.Calm;
            mover.agent.ResetPath();
            mover.ResetSpeed();
            mover.ResetAcceleration();
            UponCompletedAction();
        }


                public void DoCalmDown()
                {   StartCoroutine(CalmDownCoroutine());    }

                IEnumerator CalmDownCoroutine()
                {
                    yield return new WaitForSeconds(2f);
                    if(!awareness.IsThreatNearby())
                        alertness = AlertState.Calm;
                }



                
        #endregion

        #region HelperMethods
        /*****************************************************************************************************************
            This region contains helper methods that will only ever be called from inside this class
            For example, dying is something handled only internally in this script.
            Transitioning phases between actions perhaps, like awaking up, would be another example as its implied to happen with anything that can sleep.
            Likely will use less Coroutines in this region to handle logic so it can happen sequentially.
        */

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

        public static implicit operator Transform(CreatureController v)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}

