using System.Collections;
using Cyrcadian.Creatures;
using Cyrcadian.WorldTime;
using Cyrcadian.Items;
using UnityEngine;
using System.Collections.Generic;
using Cyrcadian.BehaviorTrees;
using TMPro;
using UnityEditor.Callbacks;

namespace Cyrcadian.UtilityAI
{

    public class CreatureController : MonoBehaviour
    {

        public MoveController mover{ get; set;}
        public AIBrain aiBrain { get; set;}
        public Action[] actionsAvailable;

        // Can manage each Behavior tree of a creature.
        public Dictionary<string, CreatureBehaviorTree> myBehaviorDictionary;

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
        public bool isEating = false;

        // Start is called before the first frame update

        void Awake()
        {
            myBehaviorDictionary = new();
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
            awarenessRange.enabled = true;
            awareness.SetCreature(this);
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
                //Debug.Log(" next action is " + aiBrain.bestAction.name + " with score : " + aiBrain.bestAction.score + " from creature " + transform.name);
                aiBrain.isFinishedDeciding = false;
                aiBrain.bestAction.Execute(this);
            }

            // Always keep our stats updated
            stats.currentHP = health.CurrentHP();
            stats.currentHunger = hungerBar.CurrentHunger();

            if(health.WasHit())
                {Debug.Log("I WAS HIT");
                Debug.Log(awareness.FindNearestThreat());
                    alertness = AlertState.Alert;}


            if(health.CurrentHP() <= 0)
                StartCoroutine(DEATH());
        }

        // Upon completing an Action, choose the next best action from all available actions
        public void UponCompletedAction()
        {   aiBrain.ChooseBestAction(actionsAvailable);    }
        
    

        #region Coroutine
        /*****************************************************************************************************************
            This region contains the code and logic for all actions that will take Real Time to complete. 
            This gives room for animations to play.
            This also makes sense from a realism perspective.
        */

        public void DoIdle(float time)
        {   StartCoroutine(IdleCoroutine(time));    }

                IEnumerator IdleCoroutine(float time)
                {   
                    float counter = time;
                    float timeSpentIdle = 0; 

                    while(counter > 0 && alertness != AlertState.Alert)
                    {
                        if(awareness.IsThreatNearby())          
                        {alertness = AlertState.Alert;    break;}
                            
                        yield return new WaitForEndOfFrame();

                        timeSpentIdle += Time.deltaTime;
                        counter -= Time.deltaTime;
                    }
                        
                    // Increases stamina by the time for every whole second I was able to spend in Idle.
                    stats.currentStamina += Mathf.FloorToInt(timeSpentIdle);
                    Mathf.Clamp(stats.currentStamina, 0, stats.staminaPool);

                    UponCompletedAction();
                }

        public void DoGraze()
        {   StartCoroutine(GrazeCoroutine());   }

                IEnumerator GrazeCoroutine()
                {
                    //Current: Move to random spot and eat there.
                    //Future: Pick a random point, and validate it (make sure I can walk there and it's a grass tile)
                    //        If point isn't valid, try check another spot(repeat until found spot). Once a valid point is found move there and then eat grass.
                    DoRandomRoam();

                    // BUG HERE : WILL GET STUCK BECAUSE THEY AREN'T MOVING AND REMAINING DISTANCE NEVER GOES BELOW STOPPING DISTANCE
                    while(mover.agent.remainingDistance > mover.agent.stoppingDistance)
                    {
                        if(awareness.IsThreatNearby())          
                        {alertness = AlertState.Alert;    break;}
                        yield return new WaitForEndOfFrame();
                    }

                    // Grass eating holds a static food value of 4 for now.
                    isEating = true;
                    float timer = 2f;
                    while(timer > 0)
                    {
                        if(awareness.IsThreatNearby())          
                        {alertness = AlertState.Alert;    break;}

                        yield return new WaitForEndOfFrame();
                        timer -= Time.deltaTime;
                    }

                    hungerBar.ChangeHunger(4);
                    isEating = false;

                    yield return new WaitForEndOfFrame();
                    
                    UponCompletedAction();
        }


        public void DoAlert()
        {   StartCoroutine(AlertCoroutine());   }

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
        {   StartCoroutine(SleepCoroutine());   }
                
                IEnumerator SleepCoroutine()
                {
                    alertness = AlertState.Unconcious;
                    sleepParticle.Play();
                    awarenessRange.enabled = false;
                    
                    switch(creatureSpecies.CircadianRhythm) 
                    {
                        case Creature.CyrcadianRhythm.Nocturnal:
                            while(DayCycle.GetTimeOfDay() != 0 && alertness == AlertState.Unconcious)
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.5f) break;   }
                            break;
                        case Creature.CyrcadianRhythm.Diurnal:
                            while(DayCycle.GetTimeOfDay() != 1 && alertness == AlertState.Unconcious)
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.5f){break;  }   }
                            break;
                        case Creature.CyrcadianRhythm.Crepuscular:
                            while(DayCycle.GetTimeOfDay() != 2 && alertness == AlertState.Unconcious)
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.5f) break;    }
                            break;
                        case Creature.CyrcadianRhythm.Cathemeral:
                            {    yield return new WaitForEndOfFrame(); if(mover.rb.velocity.sqrMagnitude > 0.5f) break;    }
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

        // Check immediate awareness for a creature to hunt, if none, move to a random point and check along that path. 
        // If found target, chase them and until they are taken down. If no target still, complete the action of Hunting.
        public void DoHunt()
        {    StartCoroutine(HuntCoroutine());    }

                IEnumerator HuntCoroutine()
                {
                    Transform huntingTarget;
                    huntingTarget = awareness.FindTastiestCreature(this);

                    if(!huntingTarget)
                    { Debug.Log("Searching for creature to hunt");
                        DoRandomRoam();
                        // Wait till I reach a random point in roaming, or find a target to hunt before continuing.
                        while(movingToRandomPoint && !awareness.FindTastiestCreature(this, out huntingTarget))
                        { yield return new WaitForEndOfFrame(); }
                        
                        DoWait(Random.Range(0.8f, 2f));
   
                        while(isWaiting)
                        { yield return new WaitForEndOfFrame(); }
                    }

                    // If found, chase that target
                    if(huntingTarget)
                    {  
                        DoChase(huntingTarget);
                        while(huntingTarget && stats.currentStamina > 0)
                        {
                            yield return new WaitForEndOfFrame();
                            if(!huntingTarget)
                                break;

                            // Check first if an attack exists for this creature
                            if(creatureSpecies.ListOfPossibleAttacks.Count != 0 )
                            {// If I was able to attempt an attack, reduce stamina
                                if(creatureSpecies.ListOfPossibleAttacks.Find(x => x.UniqueName.Contains("Pounce")).TryAttacking(this, huntingTarget))
                                    stats.currentStamina -= 2;
                            }
                        }    

                        awareness.VisibleCreatures.Remove(huntingTarget);            
                    }

                    
                    UponCompletedAction();
                }





        public void DoFlee()
        {   StartCoroutine(FleeCoroutine());    }

                IEnumerator FleeCoroutine()
                {
                    if(stats.currentStamina > stats.staminaPool * 0.5f)
                    {
                        mover.IncreaseAcceleration(.1f);
                    }

                    float timer = 0.25f;
                    Vector3 fleeDirection;
                    while(awareness.IsThreatNearby())
                    { 
                        fleeDirection = (transform.position - awareness.FindNearestThreat().position).normalized;
                        mover.UpdatePath((fleeDirection *3) + transform.position);

                        yield return new WaitForEndOfFrame();
                        
                        if(timer <= 0)
                        {
                            timer = 0.25f;
                            if(stats.currentStamina > 0)
                                stats.currentStamina -= 2;
                            else
                                stats.currentStamina = 0;

                            mover.DrainingStamina(this);                            
                        }
                        else
                            timer -= Time.deltaTime;
                    }
                    
                    // By here we have successfully flee-d from danger
                    Debug.Log("Made it to safety");
                    mover.ResetAcceleration();
                    UponCompletedAction();
                }


        public void DoCalmDown()
                {   StartCoroutine(CalmDownCoroutine());    }

                IEnumerator CalmDownCoroutine()
                {
                    yield return new WaitForEndOfFrame();
                    if(!awareness.IsThreatNearby())
                        alertness = AlertState.Calm;
                    
                    UponCompletedAction();
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

            foreach(var KeyValuePair in myBehaviorDictionary)
            {
                KeyValuePair.Value.DeleteTree();
            }

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



        public void DoRandomRoam()
        {   if(!movingToRandomPoint)StartCoroutine(RandomRoam());   }

                bool movingToRandomPoint = false;
                IEnumerator RandomRoam()
                {
                    if(movingToRandomPoint)
                        yield break;

                    mover.MoveToRandomPoint(awarenessRange.bounds.extents.x * 0.5f);
                    movingToRandomPoint = true;

                    // While still traveling a path, wait before deciding
                    while(mover.IsMoving())
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    movingToRandomPoint = false;
                }

        public void DoChase(Transform transform)
        {   StartCoroutine(ChaseCoroutine(transform));    }

                IEnumerator ChaseCoroutine(Transform target)
                {
                    if(!target)
                        yield break;

                    mover.IncreaseAcceleration(.1f);
                    mover.UpdatePath(target.position);

                    // If I have stamina, and they haven't gotten out of range
                    while( stats.currentStamina > 0 && awareness.VisibleCreatures.Contains(target) && target)
                    {
                        // Increase move speed as they get closer?
                        yield return new WaitForEndOfFrame();
                        if(!target)
                            break;

                        mover.UpdatePath(target.position);
                    }

                    mover.ResetAcceleration();
                }        

        bool isWaiting = false;
        public void DoWait(float time)
        {   StartCoroutine(WaitCoroutine(time));    }

                IEnumerator WaitCoroutine(float time)
                {
                    isWaiting = true;
                    float counter = time;

                    while(counter > 0 && alertness != AlertState.Alert)
                    {
                        yield return new WaitForEndOfFrame();
                        counter -= Time.deltaTime;
                    }
                    isWaiting = false;
                }
        #endregion

    }
}

