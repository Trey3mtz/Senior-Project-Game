using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Cyrcadian.Creatures
{
    [RequireComponent(typeof(Collider2D))]
    public class Awareness : MonoBehaviour
    {
        // Creatures will get noticed by some, and we need to remember foe from not foe, and how much influence they had over our health
        [Serializable]
        public struct CreatureData
        {
            public bool isThreat;
            public int healthInfluenced;
        }

        // Target specific transforms in our list of visible things. Transforms are keys, and CreatureData is the value associated.
        public Dictionary<Transform, CreatureData> CurrentlyTargetedCreatures { get; private set; }


        // A public method attacks can call, to register themselves as taggedCreatures of importance, with them identifing as threats, and health amount they taken from us
        // Only adds them if they aren't already in the respective lists. If they are, increase the amount of health they have influenced.
        public void AddTargetedCreature(Transform incomingCreature, bool isThreat, int healthInfluenced)
        {    
                // container for the incoming data, and its updated data if it exists already.
                int updatedHealthInfluenced = healthInfluenced;
                if(CurrentlyTargetedCreatures.TryGetValue(incomingCreature, out CreatureData oldValue))
                    updatedHealthInfluenced = oldValue.healthInfluenced + healthInfluenced;   
                
            var newCreatureData = new CreatureData(){isThreat = isThreat,  healthInfluenced = updatedHealthInfluenced};  

            // If they don't exist, add them to the dictionary, if they do already, update their data.
            if(!CurrentlyTargetedCreatures.TryAdd(incomingCreature, newCreatureData))
                CurrentlyTargetedCreatures[incomingCreature] = newCreatureData;
            
            // Repeat the process for KnownThreats.
            if(isThreat && !KnownThreats.TryAdd(incomingCreature, newCreatureData))
                KnownThreats[incomingCreature] = newCreatureData;

            //if(!KnownThreats.ContainsKey(incomingCreature))
            //Debug.Log(incomingCreature.name + " was added to known threats");
        }

        // A specific list for all visible creatures, and not just targeted ones by our behavior
        // A list for all visible items nearby
        // A dictionary of past threats, containing data such as how much they've harmed us. (Used to identify foreign and new threats this creature was unaware of)
        [HideInInspector] public List<Transform> VisibleCreatures { get; private set; }
        [HideInInspector] public List<Transform> VisibleWorldItems { get; private set; }
        [HideInInspector] public Dictionary<Transform, CreatureData> KnownThreats {get; private set;}

        [Tooltip("The distance at which you lose sight")]
        public float losingDistance = 4f; 
        [HideInInspector] public float sqrLosingDistance;
        
        void Awake()
        {
            sqrLosingDistance = losingDistance * losingDistance;
            CurrentlyTargetedCreatures = new Dictionary<Transform, CreatureData>();
            VisibleCreatures = new List<Transform>();
            VisibleWorldItems = new List<Transform>();
            KnownThreats = new Dictionary<Transform, CreatureData>();
        }

        //                                          IMPORTANT NOTE:     layer 10 is "Creature", so we are checking if they are creatures
        //                                                              layer 15 is dropped items
        //                                                              Use creature collider's root transform, as that is where Creature data is.

        void OnTriggerEnter2D(Collider2D collider)
        {
            if(!gameObject.activeInHierarchy)
                return;
            // Keep track of all visible creatures!
            if(collider.gameObject.layer == 10 )
                    VisibleCreatures.Add(collider.transform.root);
            else if(collider.gameObject.layer == 15)
                    VisibleWorldItems.Add(collider.transform);          
        }


        void OnTriggerExit2D(Collider2D collider)
        {
            if(!gameObject.activeInHierarchy)
                return;

            // If we lost track of a creature that was visible,  wait a moment to remove them from the stack
            if (collider.gameObject.layer == 10 && VisibleCreatures.Contains(collider.transform.root)) {
                if((collider.transform.position - gameObject.transform.position).sqrMagnitude > sqrLosingDistance)     
                    StartCoroutine(CreatureOutOfVision(collider.transform.root));   
            }
            else if(collider.gameObject.layer == 15 && VisibleWorldItems.Contains(collider.transform))
                {   VisibleWorldItems.Remove(collider.transform);       VisibleWorldItems.TrimExcess();}

            if(CurrentlyTargetedCreatures.ContainsKey(collider.transform.root))
                StartCoroutine(TargetOutOfVision(collider.transform.root));
        }


        // If Target stepped out of vision, wait for 1 seconds and check if they are in the list still.
        // If Target is in list of visible creatures, try again Until they leave.
        // Once left the Target is added to a special ever growing list of previously identified unique threats.
        IEnumerator TargetOutOfVision(Transform target)
        { 
            yield return new WaitForSeconds(1f);

            if(!VisibleCreatures.Contains(target))
                CurrentlyTargetedCreatures.Remove(target);
            else if(Mathf.Abs((target.position - transform.position).sqrMagnitude) > sqrLosingDistance)
            {   
                // In case we missed the first check for if out of vision.
                VisibleCreatures.Remove(target);
                CurrentlyTargetedCreatures.Remove(target);
                CurrentlyTargetedCreatures.TrimExcess();
            }
            else
                StartCoroutine(TargetOutOfVision(target));
        }

        // If creature is out of vision for too long, no longer can see them
        IEnumerator CreatureOutOfVision(Transform targetTransform)
        {
            yield return new WaitForSeconds(.5f);
            VisibleCreatures.Remove(targetTransform);
            VisibleCreatures.TrimExcess();
        }


        public bool IsThreatNearby()
        {
            bool foundThreat = false;

            foreach(Transform transform in VisibleCreatures)
                if(KnownThreats.ContainsKey(transform))
                    {   foundThreat = true;    break;  }

            return foundThreat;
        }

        public Transform NearestThreat()
        {
            
            if(IsThreatNearby())
            {   
                Transform closestThreat = null;
                foreach(Transform visibleThreat in VisibleCreatures)
                {
                    // If no closest threat, place the first found one in there. Then start comparing.
                    if(KnownThreats.ContainsKey(visibleThreat))
                        if(!closestThreat)
                            closestThreat = visibleThreat;
                        else if(Mathf.Abs((visibleThreat.position - transform.position).sqrMagnitude) > Mathf.Abs((closestThreat.position - transform.position).sqrMagnitude))
                            closestThreat = visibleThreat;
                }
                return closestThreat;
            }
            else
                return null;
        }
    }
}
