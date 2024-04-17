using UnityEngine;
using Cyrcadian.BehaviorTrees;
using System.Collections.Generic;
using Unity.Collections;
using System;
using System.Linq;
using Cyrcadian.Creatures;
using System.Collections;
using System.Reflection;
using Unity.Assertions;


namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Find Food", menuName = "UtilityAI/Actions/Find Food")]
    public class FindFood : Action
    {

        /// <summary>
        ///                 This is a Behavior Tree:    We will follow a flow, which decides what we need to execute in order to find food.
        ///                                             Depending on the Diet and Info we have on thisCreature, we will go down different 
        ///                                             paths which either end the Action, or loop back to the top as we need our Action
        ///                                             to result in us getting food for survival.
        ///                                                  
        /// </summary>

            public override void Execute(CreatureController thisCreature)
            {
                // Must self destruct once we reach the end of the logic
                // MUST SELF DESTRUCT ONCE FINISHED
                // DO NOT LET THIS MONOBEHAVIOUR INSTANCE STAY IN MEMORY

                // Assume CreatureController is at the top of gameobject's hierarchy
                if(!thisCreature.myBehaviorDictionary.ContainsKey("FindFoodTree"))
                    thisCreature.gameObject.AddComponent<FindFoodTree>();
                else
                    thisCreature.myBehaviorDictionary["FindFoodTree"].StartBehavior();
            }


        class FindFoodTree : CreatureBehaviorTree
        {
            // Constructs an action tree, based on the behavior of finding food. Needs a specific creature that will execute this action.
            public FindFoodTree(CreatureController thisCreature)
            {
                _myCreature = thisCreature;
                _myCreature.myBehaviorDictionary.TryAdd("FindFoodTree",this);
            }

            // Called on Start(), so make sure you initialize a the FindFoodTree with a creature.
            protected override Node SetupTree(CreatureController thisCreature)
            {

                Node root = new Selector(new List<Node>
                {
                    
                    // EXAMPLE: A sequence does the following: Once it succeeds the firstCheck it goes to the firstTask, and doesn't start the second until finish with the first
                    //new Sequence(new List<Node>
                    //{
                    //    new firstCheck(),
                    //
                    //    new firstTask(),
                    //    new secondTask(),
                    //}),


                    //  First prioritize food items in the immediate area
                    new Sequence(new List<Node>
                    {
                        new CheckFoodIsNearby(thisCreature),
                        // If food is nearby walk to it, then eat it.
                        new Sequence(new List<Node>
                        {
                            new GoToFoodTask(),         // StopBehavior() and calls UponCompletedAction() upon Success.
                            new EatFoodTask()       // StopBehavior() and calls UponCompletedAction() upon success.
                        }),                
                    }),
                    
                    //  No immediate food items on the ground, check for nearby sources of food
                    new Sequence(new List<Node>
                    {
                        //  Gets a target
                        new CheckNearbyFoodSources(thisCreature),

                        //  Behavior changes for static non-living targets, and living targets
                        new Selector(new List<Node>
                        {
                            new Sequence(new List<Node>
                            {
                                new CheckIfCreature(),
                                thisCreature.creatureSpecies.Hunting_Behavior.Execute(thisCreature), // Execute() returns a new Selector node based on this Creature's hunting habits.
                            }),      

                            new Sequence(new List<Node>
                            {
                                // Check if they have a creature component? If so, HUNT them instead.
                                new WalkToFoodSourceTask(),
                                // new Interact_With_Target_Task() // returns to the root success                       INCOMPLETE NEED THIS NODE FOR NON-LIVING FOOD SOURCES
                            }),

                      
                        })
                       
                    
                    }),   

                    //  No food items or source found? Search for sources of our food.
                    new RandomPatrolSearch(thisCreature),  // StopBehavior() and calls UponCompletedAction() upon success.
                });

                return root;
            }

        }

#region Checks
        class CheckFoodIsNearby : Node
        {
            private CreatureController _myCreature;

            public CheckFoodIsNearby(CreatureController thisCreature)
            {
                Assert.IsNotNull(thisCreature);

                _myCreature = thisCreature;
     
            }

            public override NodeState Evaluate()
            {              
                                                    Assert.IsNotNull(parent);
                                                    Assert.IsNotNull(parent.parent);
                object t = GetData("NextMeal");
                if(t == null)
                {   //Debug.Log(" Check for Food Items ");
                    Transform nextMeal = null;
                    List<Transform> considerationsForMeal = new();

                    foreach(Transform item in _myCreature.awareness.VisibleWorldItems)
                    {   
                        if(!item)
                            continue;

                        // If my food selection contains this item, add it to my considerations for nextMeal.
                        if( _myCreature.creatureSpecies.GetFoodSelection().Contains( item.GetComponent<World_Item>().CheckFoodType()))
                        {
                            considerationsForMeal.Add(item);
                        }

                    } 

                    float score;
                    float highestScore = -1;

                    // Figure out the best option for a snack.
                    foreach(Transform possibleSnack in considerationsForMeal)
                    {
                        score = possibleSnack.GetComponent<World_Item>().GetFoodValue();
                        if(score > highestScore)
                        {   highestScore = score;
                            nextMeal = possibleSnack;}
                    }
                    
                    // If I didn't find anything, fail check.
                    if(!nextMeal) 
                    {   //Debug.Log("Didn't find food item");
                        state = NodeState.FAILURE;
                        return state;   
                    }
                    else   
                    {
                        SetRootData("NextMeal", nextMeal);
                    }
                }

                //Debug.Log("I have found an edible item");
                state = NodeState.SUCCESS;
                return state;
            }
        }

        class CheckNearbyFoodSources : Node
        {
            CreatureController myCreature;
            Transform _target = null;
            

            public CheckNearbyFoodSources(CreatureController thisCreature)
            {
                myCreature = thisCreature;
            }

            public override NodeState Evaluate()
            {
                
                object t = GetData("Target");
                if(t == null)
                {
                    // If there are visible foodSources
                    if(myCreature.awareness.VisibleFoodSources.Count > 0)
                    {   Debug.Log("I see some possible food sources!");
                        // Target the BEST option. Which is determined by a Utility System taking into consideration distance from me and value of their food item drops.
                        _target = myCreature.awareness.DetermineBestFoodSource();

                        if(!_target)
                        {Debug.Log("Didn't find any food source");
                            state = NodeState.FAILURE;
                            return state;
                        }

                        // We assert since VisibleFoodSources being greater than 1 implies a target will exist.
                        Assert.IsNotNull(_target);

                        SetRootData("Target", _target);

                         Debug.Log("Found a food source");
                        
                        state = NodeState.SUCCESS;
                        return state;
                    }
                    // We didn't have any visible Food Sources
                    state = NodeState.FAILURE;
                    return state;
                }
                
                _target = t as Transform;

                // Target has died or doesn't exist.
                if(!_target)
                {
                    ClearData("Target");
                    state = NodeState.FAILURE;
                    return state;
                }
                
                Debug.Log("Found a food source");

                state = NodeState.SUCCESS;
                return state;
            }
        }

        public class CheckIfCreature : Node
        {
            public override NodeState Evaluate()
            {
                object target = GetData("Target");
                
                Transform myTarget = (Transform)target;

                if(!myTarget)
                {
                    state = NodeState.FAILURE;
                    return state;
                }

             
                if(myTarget.root.gameObject.layer == 10)
                {
                    Debug.Log("Food source IS CREATURE");
                    state = NodeState.SUCCESS;
                    return state;
                }

                state = NodeState.FAILURE;
                return state;
            }
        }
#endregion

#region Tasks
        class GoToFoodTask : Node
        {           
            CreatureController myCreature;
            bool beganWalking = false;
            
            public override NodeState Evaluate()
            {
                //Debug.Log(" Moving towards food ");
                Transform nextMeal = (Transform)GetData("NextMeal");
                // Food is eaten, or destroyed?
                if(!nextMeal)
                {
                    ClearData("NextMeal");
                    beganWalking = false;
                    StopBehavior();
                    myCreature.UponCompletedAction();
                    state = NodeState.FAILURE;
                    return state;
                }

                // If my creature hadn't yet starting their path
                if( !beganWalking)
                {
                    beganWalking = true;
                    myCreature = (CreatureController)GetData("MyCreature");
                    myCreature.mover.UpdatePath(nextMeal.position);
                }

                if(Vector3.Distance(nextMeal.position, myCreature.gameObject.transform.position) > 0.1f)
                {   
                    //Debug.Log("Arrived at food");
                    beganWalking = false;
                    state = NodeState.SUCCESS;
                    return state;
                }
                state = NodeState.RUNNING;
                return state;
            }
        }

        class EatFoodTask : Node
        {
            private World_Item _foodStack;
            private Transform _foodTransform;
            private CreatureController myCreature;
            bool beganChewing = false;
            float _chewTime = 1f;
            float timer;

            public override NodeState Evaluate()
            {   myCreature = (CreatureController)GetData("MyCreature");
                Debug.Log(" Eating Food Item ");
                object t = GetData("NextMeal");
                // Food is eaten, or not assigned?
                if(t == null)
                {
                    StopBehavior();
                    myCreature.UponCompletedAction();
                    state = NodeState.FAILURE;
                    return state;
                }
                _foodTransform = (Transform)t;

                if(!_foodTransform)
                {
                    StopBehavior();
                    myCreature.UponCompletedAction();
                    state = NodeState.FAILURE;
                    return state;
                }

                _foodStack = _foodTransform.GetComponent<World_Item>();

                // Start chewing
                if(timer <= 0 && !beganChewing)
                {
                    timer = _chewTime;
                    beganChewing = true;
                    myCreature.isEating = true;
                }

                // Animation timer countdown
                if(timer > 0)
                {
                    timer -= Time.deltaTime;
                }

                // Finished animation
                if(timer <= 0 && beganChewing)
                {
                    Debug.Log("Finished eating item");
                    myCreature.hungerBar.ChangeHunger(_foodStack.GetFoodValue());
                    myCreature.awareness.VisibleWorldItems.Remove(_foodTransform);
                    Destroy(_foodTransform.gameObject);

                    beganChewing = false;

                    
                    StopBehavior();
                    myCreature.UponCompletedAction();
                    state = NodeState.SUCCESS;
                    return state;                    
                }

                state = NodeState.RUNNING;
                return state;
            }   

        }

/*
// 4 parents from root
        class HuntFoodSourceTask : Node
        {

            public override NodeState Evaluate()
            {
                object target = parent.parent.GetData("Target");
                if(target == null)
                {
                    state = NodeState.FAILURE;
                    return state;
                }


                CreatureController myCreature = (CreatureController)parent.parent.GetData("MyCreature");
                
                var huntingBehavior = myCreature.creatureSpecies.HuntingBehavior;
                Type hunt= huntingBehavior.GetType();
                Component myComponent;
             
                if(!myCreature.gameObject.GetComponent(hunt))
                    myComponent = myCreature.gameObject.AddComponent(hunt);
                else
                    myComponent = myCreature.gameObject.GetComponent(hunt);

                // Call a method from the huntingBehaviorComponent instance
                MethodInfo methodInfo = hunt.GetMethod("Execute");

                // CreatureController, then target Transform as parameters for the Execute method
                object[] parameters = new object[]{myCreature, target};

                if(methodInfo.Invoke(myComponent, parameters) != null)
                {
                    state = NodeState.SUCCESS;
                    return state;
                }

                state = NodeState.RUNNING;
                return state;
            }

        }   */
// 4 parents from root
        class WalkToFoodSourceTask : Node
        {
            bool isAlreadyMoving = false;
            CreatureController myCreature;

            public override NodeState Evaluate()
            {
                myCreature = (CreatureController)GetData("MyCreature");
                
                //Debug.Log("(static source) Found food source , moving towards it  ");
                object t = GetData("Target");
                // Food is eaten, or not assigned?
                if(t == null)
                {
                    state = NodeState.FAILURE;
                    return state;
                }

                if(!isAlreadyMoving)
                {
                    isAlreadyMoving = true;
                    
                    Transform target = (Transform)t;
                    myCreature.mover.UpdatePath(target.position);

                    state = NodeState.RUNNING;
                    return state;
                }


                if(!myCreature.mover.UpdatePath())
                {
                    state = NodeState.SUCCESS;
                    return state;
                }

                state = NodeState.RUNNING;
                return state;
            }
        }

// Revisit... I want to break behavior when needed, like being attacked
    class RandomPatrolSearch : Node
    {
        bool isWaiting = false;
        float _waitCounter = 0;

        CreatureController myCreature;

        public RandomPatrolSearch(CreatureController thisCreature)
        {   
            myCreature = thisCreature;
        }

        public override NodeState Evaluate()
        {
            Debug.Log("Doing a patrol to find food");
            // If I am still waiting to randomly roam again
            if (isWaiting)
            {
                _waitCounter += Time.deltaTime;
                // Stand still after roaming for (low number, high number) seconds
                if (_waitCounter >= UnityEngine.Random.Range(.8f , 1.2f))
                {   
                    isWaiting = false;
                    myCreature.DoRandomRoam();
                }
            }
            // I finished waiting, I will now randomly roam.
            else
            {
                // Actively walking and searching / sniffing around
                if(myCreature.mover.IsMoving())
                {
                    // I am already moving...
                    // Could do custom "Searching" animation logic later by calling myCreature.GetComponentInChildren<Creature_Animation>().SearchingAndMoving();
                }
                else
                {
                    _waitCounter = 0f;
                    isWaiting = true;
                }
            }         

            state = NodeState.RUNNING;
            return state;
        }
    }         
#endregion

    }
}
