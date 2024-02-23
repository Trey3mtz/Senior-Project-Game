using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyrcadian.BehaviorTrees;

namespace Cyrcadian.UtilityAI
{
    [CreateAssetMenu(fileName = "Simple Hunting", menuName = "UtilityAI/BehaviorTrees/Simple Hunting")]
    public class SimpleHunting : HuntingBehavior
    {
        /*
        High level psudo code example for Simple Hunting

        Root Selector
            Sequence (Chase "Task" and Attack "Task")
                Chase Node  
                    - Return FAILURE if the target is lost
                    - Return SUCCESS when close to the target
                    - Return RUNNING Move towards the target
                Attack Node
                    - Return FAILURE if the target is dead
                    - Return SUCCESS if we were able to perform an Attack
                    - Return FAILURE if the attack is unable to be performed
                    
        return Root

        SUCCESS -> GO TO ROOT
        RUNNING -> DO CURRENT NODE
        FAILURE -> RETURN TO CALL AND RETURN ROOT
        */
        public override Node Execute(CreatureController myCreature)
        {  
                Node root = new Selector(new List<Node>
                {

                    //  First prioritize attack?
                    new Sequence(new List<Node>
                    {
                        new SimpleChase(myCreature)
                    }),


                    new Sequence(new List<Node>
                    {
                        new ShortestRangeAttack(myCreature)
                    }),   

                  
                });


            return root;
        }
    }

    public class SimpleChase : Node
    {
        Transform target;   object TARGET;
        CreatureController myCreature;  

        public SimpleChase(CreatureController thisCreature)
        {
            myCreature =  thisCreature;
        }

        public override NodeState Evaluate()
        {
            TARGET = GetData("Target");
            target = TARGET as Transform;            
            // If no target, FAILED (they either escaped, or are dead already)
            if(TARGET == null)
            {
                state = NodeState.FAILURE;
                return state;
            }

            // If in range to attack, SUCCESS
            // Look for the shortest ranged attack, hopefully they in order...
            string attack_name = "";
            foreach(Attack attack in  myCreature.creatureSpecies.ListOfPossibleAttacks)
            {
                if(attack.IsInAttackRange(myCreature, target))
                {
                    attack_name = attack.name;
                    break;
                }   
            }

            // One of our attacks is in Range
            if(attack_name != "")
            {
                state = NodeState.SUCCESS;
                return state;
            }

            // Move to their position
            myCreature.mover.MoveTo(target.position);


            state = NodeState.RUNNING;
            return state;
        }
    }

    public class ShortestRangeAttack : Node
    {
        Transform target;   object TARGET;
        CreatureController myCreature;  

        public ShortestRangeAttack(CreatureController thisCreature)
        {
            myCreature = thisCreature;
        }

        public override NodeState Evaluate()
        {            
            TARGET = GetData("Target");
            target = TARGET as Transform;
            // If no target, FAILED (they either escaped, or are dead already)
            if(TARGET == null)
            {
                state = NodeState.FAILURE;
                return state;
            }

            // If in range to attack, SUCCESS
            // Look for the shortest ranged attack, hopefully they in order...
            string attack_name = "";
            foreach(Attack attack in  myCreature.creatureSpecies.ListOfPossibleAttacks)
            {
                if(attack.IsInAttackRange(myCreature, target))
                {
                    attack_name = attack.name;
                    break;
                }   
            }

            // One of our attacks is in Range
            if(attack_name != "")
            {   // Try doing the shortest ranged attack we have, if we performed an attack, lower our stamina.
                if(myCreature.creatureSpecies.ListOfPossibleAttacks.Find(x => x.UniqueName.Contains("Pounce")).TryAttacking(myCreature, target))
                {    
                    myCreature.stats.currentStamina -= 2;
                    state = NodeState.SUCCESS;
                    return state;
                }
            }

            // Either we have no attacks, or we didn't attack.
            state = NodeState.FAILURE;
            return state;
        }
    }
}
