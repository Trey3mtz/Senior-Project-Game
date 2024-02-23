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
                thisCreature.gameObject.AddComponent<FindFoodTree>();
            }


        class FindFoodTree : CreatureBehaviorTree
        {
            // Constructs an action tree, based on the behavior of finding food. Needs a specific creature that will execute this action.
            public FindFoodTree(CreatureController thisCreature)
            {
                _myCreature = thisCreature;
                
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
                        new GoToFoodTask(this),
                        new EatFoodTask(this)       // This destroys the monobehaviour and calls UponCompletedAction()
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
                                new WalkToFoodSourceTask(),
                                // new Interact_With_Target_Task() // returns to the root success
                            }),

                            new Sequence(new List<Node>
                            {
                                thisCreature.creatureSpecies.Hunting_Behavior.Execute(thisCreature), // Execute() returns a new Selector node.

                            }),                            
                        })
                       
                    
                    }),   

                    //  No food items or source found? Search for sources of our food.
                    new SearchForFoodTask(thisCreature), // returns to root
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
                {  Debug.Log(" Check for Food Items ");
                    Transform nextMeal = null;
                    foreach(Transform item in _myCreature.awareness.VisibleWorldItems)
                    {
                        if(!item)
                            continue;

                        // If my food selection contains this item, make it my next meal and break. Looks for the first possible choice.
                        if( _myCreature.creatureSpecies.GetFoodSelection().Contains( item.GetComponent<World_Item>().CheckFoodType()))
                        {
                            nextMeal = item;
                            break;
                        }
                    } 
                    
                    if(!nextMeal) 
                    {   state = NodeState.FAILURE;
                        return state;   }
                    else   
                    {
                        parent.parent.SetData("NextMeal", nextMeal);
                        state = NodeState.SUCCESS;
                        return state;

                    }
                }

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
                { Debug.Log("Checking for Food Sources ");
                    // If there are visible foodSources
                    if(myCreature.awareness.VisibleFoodSources.Count > 0)
                    {
                        // Target the BEST option. Which is determined by a Utility System taking into consideration distance from me and value of their food item drops.
                        _target = myCreature.awareness.DetermineBestFoodSource();

                       
                        Assert.IsNotNull(_target);
                        // Didn't find anything..?
                        if(!_target)
                        {
                            state = NodeState.FAILURE;
                            return state;
                        }

                        parent.parent.SetData("Target", _target);
                        
                        state = NodeState.SUCCESS;
                        return state;
                    }

                    state = NodeState.FAILURE;
                    return state;
                }

                state = NodeState.SUCCESS;
                return state;
            }
        }
#endregion

#region Tasks
        class GoToFoodTask : Node
        {
            MonoBehaviour _mono;
            bool isAlreadyMoving = false;
            CreatureController myCreature;
            public GoToFoodTask(MonoBehaviour monoBehaviour)
            {   _mono = monoBehaviour;  }

            public override NodeState Evaluate()
            {
                Debug.Log(" Moving towards food ");
                object t = GetData("NextMeal");
                // Food is eaten, or not assigned?
                if(t == null)
                {
                    state = NodeState.FAILURE;
                    return state;
                }

                if(!isAlreadyMoving)
                {
                    isAlreadyMoving = true;
                    myCreature = (CreatureController)GetData("MyCreature");
                    Transform nextMeal = (Transform)GetData("NextMeal");
                    myCreature.mover.MoveTo(nextMeal.position);

                    state = NodeState.RUNNING;
                    return state;
                }

                if(myCreature.mover.IsAtDestination())
                {
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
            bool isChewing = false;
            bool isFinished = false;
            MonoBehaviour _mono;

            public EatFoodTask(MonoBehaviour mono)
            { _mono = mono; }

            public override NodeState Evaluate()
            {
                Debug.Log(" Eating Food Item ");
                object t = GetData("NextMeal");
                // Food is eaten, or not assigned?
                if(t == null)
                {
                    state = NodeState.FAILURE;
                    return state;
                }

                if(!isChewing)
                {
                    _foodTransform = (Transform)GetData("NextMeal");
                    _foodStack = _foodTransform.GetComponent<World_Item>();

                    isChewing = true;
                    myCreature = (CreatureController)GetData("MyCreature");
                    myCreature.isEating = true;
                    _mono.StartCoroutine(Wait(myCreature));
                }

                if(isFinished)
                {
                    myCreature.GetComponent<HungerBar>().ChangeHunger(_foodStack.GetFoodValue());
                    myCreature.awareness.VisibleWorldItems.Remove(_foodTransform);
                    Destroy(_foodTransform.gameObject);

                    // We found and ate food, destroy this MonoBehaviour                            // FINISHED
                    ClearAllData();
                    myCreature.UponCompletedAction();
                    Destroy(_mono);
                }

                state = NodeState.RUNNING;
                return state;
            }   

            IEnumerator Wait(CreatureController myCreature)
            {    
                while(myCreature.isEating)
                {
                    yield return new WaitForEndOfFrame();
                }
                myCreature.isEating = false;
                isFinished = true;
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
                
                // If herbivore do NOT walk... Hunt instead
                if(myCreature.creatureSpecies.Diet != Creature.DietarySystem.Herbivore)
                {
                    state = NodeState.FAILURE;
                    return state;
                }
                Debug.Log("(Herbivore) Found food source , moving towards it  ");
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
                    myCreature.mover.MoveTo(target.position);

                    state = NodeState.RUNNING;
                    return state;
                }


                if(myCreature.mover.IsAtDestination())
                {
                    state = NodeState.SUCCESS;
                    return state;
                }

                state = NodeState.RUNNING;
                return state;
            }
        }

// Revisit... I want to break behavior when needed, like being attacked
    class SearchForFoodTask : Node
    {
        bool isSearching = false;
 
        CreatureController myCreature;

        public SearchForFoodTask(CreatureController thisCreature)
        {   
            myCreature = thisCreature;
        }

        public override NodeState Evaluate()
        {
            if(isSearching)
            {
                object t = GetData("Target");
                if(t != null)
                {
                    isSearching = false;
                    state = NodeState.SUCCESS;
                    return state;
                }

            }
            else
            {Debug.Log(" Searching ");
                object t = GetData("Target");
                if(t == null)
                {
                    isSearching = true;
                    myCreature.DoRandomRoam();
                }
            }         

            state = NodeState.RUNNING;
            return state;
        }
    }         
#endregion

    }
}
