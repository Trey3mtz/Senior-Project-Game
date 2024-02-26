using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Cyrcadian.UtilityAI;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Jobs;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.Burst;
using UnityEditor.UIElements;
using UnityEditor.ShaderGraph;
using Cyrcadian.Items;
using System.Security.Cryptography;
using Unity.Assertions;

namespace Cyrcadian.Creatures
{
    [RequireComponent(typeof(Collider2D))]
    public class Awareness : MonoBehaviour
    {

        [SerializeField] AnimationCurve responseCurve;

        // Creatures will get noticed by some, and we need to remember foe from not foe, and how much influence they had over our health
        [Serializable]
        public struct CreatureData
        {
            public bool isThreat;
            public int healthInfluenced;
        }

        // Target specific transforms in our list of visible things. Transforms are keys, and CreatureData is the value associated.
        public Dictionary<Transform, CreatureData> CurrentlyTargetedCreatures { get; private set; }
        private CreatureController myCreature;
        public void SetCreature(CreatureController thisCreature)
        {   myCreature = thisCreature;  }

        // A public method attacks can call, to register themselves as taggedCreatures of importance, with them identifing as threats, and health amount they taken from us
        // Only adds them if they aren't already in the respective lists. If they are, increase the amount of health they have influenced.
        public void AddTargetedCreature(Transform incomingCreature, bool isThreat, int healthInfluenced)
        {    

            incomingCreature = incomingCreature.root;
                // container for the incoming data, and its updated data if it exists already.
                int updatedHealthInfluenced = healthInfluenced;
                if(CurrentlyTargetedCreatures.TryGetValue(incomingCreature, out CreatureData oldValue))
                    updatedHealthInfluenced = oldValue.healthInfluenced + healthInfluenced;   
                
            Debug.Log(incomingCreature.name);

            // If they don't exist, add them to the dictionary, if they do already, update their data.
            if(!CurrentlyTargetedCreatures.TryAdd(incomingCreature, new CreatureData(){isThreat = isThreat,  healthInfluenced = updatedHealthInfluenced}))
                CurrentlyTargetedCreatures[incomingCreature] = new CreatureData(){isThreat = isThreat,  healthInfluenced = updatedHealthInfluenced};
            
            // Repeat the process for KnownThreats.
            if(isThreat && !KnownThreats.TryAdd(incomingCreature, new CreatureData(){isThreat = isThreat,  healthInfluenced = updatedHealthInfluenced}))
                KnownThreats[incomingCreature] = new CreatureData(){isThreat = isThreat,  healthInfluenced = updatedHealthInfluenced};

            //if(!KnownThreats.ContainsKey(incomingCreature))
            //Debug.Log(incomingCreature.name + " was added to known threats");
        }

        // A specific list for all visible creatures, and not just targeted ones by our behavior
        // A list for all visible items nearby
        // A dictionary of past threats, containing data such as how much they've harmed us. (Used to identify foreign and new threats this creature was unaware of)
        [HideInInspector] public List<Transform> VisibleCreatures { get; private set; }
        [HideInInspector] public List<Transform> VisibleWorldItems { get; private set; }
        [HideInInspector] public List<Transform> VisibleFoodSources {get; private set; }
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
            VisibleFoodSources = new List<Transform>();
            KnownThreats = new Dictionary<Transform, CreatureData>();
        }

        //                                          IMPORTANT NOTE:     layer 10 is "Creature", so we are checking if they are creatures
        //                                                              layer 15 is dropped items
        //                                                              Use creature collider's root transform, as that is where Creature data is.

        void OnTriggerEnter2D(Collider2D collider)
        {
            if(!gameObject.activeInHierarchy)
                return;
            if(collider.gameObject.layer == 10 && !collider.GetComponentInParent<CreatureController>())
                return;
            // Keep track of all visible creatures!
            if(collider.gameObject.layer == 10 && !VisibleCreatures.Contains(collider.transform.root))
                    VisibleCreatures.Add(collider.transform.root);
            else if(collider.gameObject.layer == 15 && !VisibleWorldItems.Contains(collider.transform))
                    VisibleWorldItems.Add(collider.transform);          


            // Check if whatever we collided with has something from myCreature.CreatureSpecies.PossibleFoodSources
            if(myCreature.creatureSpecies.PossibleFoodSources != null)
                if(myCreature.creatureSpecies.PossibleFoodSources.Contains(collider.transform.root.tag) && !VisibleFoodSources.Contains(collider.transform.root))
                    VisibleFoodSources.Add(collider.transform.root);
        }


        void OnTriggerExit2D(Collider2D collider)
        {
            if(!gameObject.activeInHierarchy)
                return;
            if(collider.gameObject.layer == 10 && !collider.GetComponentInParent<CreatureController>())
                return;

            // If we lost track of a creature that was visible,  wait a moment to remove them from the stack
            // If they died, immediately remove their transform.
            if (collider.gameObject.layer == 10 && VisibleCreatures.Contains(collider.transform.root)) {
                if(collider.GetComponentInParent<CreatureController>().isDying)
                    VisibleCreatures.Remove(collider.transform);
                else if((collider.transform.position - gameObject.transform.position).sqrMagnitude > sqrLosingDistance)     
                    StartCoroutine(CreatureOutOfVision(collider.transform.root));   
            }
            else if(collider.gameObject.layer == 15 && VisibleWorldItems.Contains(collider.transform))
                {   VisibleWorldItems.Remove(collider.transform);       VisibleWorldItems.TrimExcess();}

            if(CurrentlyTargetedCreatures.ContainsKey(collider.transform.root))
                StartCoroutine(TargetOutOfVision(collider.transform.root));

            if(VisibleFoodSources.Contains(collider.transform.root))
            {
                if(collider.GetComponentInParent<CreatureController>().isDying)
                    VisibleFoodSources.Remove(collider.transform);
                else if((collider.transform.position - gameObject.transform.position).sqrMagnitude > sqrLosingDistance)     
                    StartCoroutine(FoodSourceOutOfVision(collider.transform.root));}
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

            if(Mathf.Abs((targetTransform.position - transform.position).sqrMagnitude) > sqrLosingDistance)
            {            
                VisibleCreatures.Remove(targetTransform);
                VisibleCreatures.TrimExcess();
            }
        }

        IEnumerator FoodSourceOutOfVision(Transform targetTransform)
        {
            yield return new WaitForSeconds(.5f);

            if(Mathf.Abs((targetTransform.position - transform.position).sqrMagnitude) > sqrLosingDistance)
            {            
                VisibleFoodSources.Remove(targetTransform);
                VisibleFoodSources.TrimExcess();
            }
        }

        public bool IsThreatNearby()
        {
            bool foundThreat = false;

            foreach(Transform knownThreat in KnownThreats.Keys)
            {
                foundThreat = VisibleCreatures.Contains(knownThreat);
                if(foundThreat)
                    break;
            }

            return foundThreat;
        }

        
        // Returns the transform of the closest threat
        
        public Transform FindNearestThreat()
        {
            var threatsNearMe = VisibleCreatures.Intersect(KnownThreats.Keys);
            Dictionary<float3, Transform> tempDictionary = new Dictionary<float3, Transform>();

            foreach(Transform nearByThreat in threatsNearMe)
                tempDictionary.TryAdd(nearByThreat.position, nearByThreat);
            
            NativeArray<float3> TempNativeArray = new NativeArray<float3>(tempDictionary.Count, Allocator.TempJob);
            TempNativeArray.CopyFrom(tempDictionary.Keys.ToArray());

            NativeArray<float3> result = new NativeArray<float3>(1, Allocator.TempJob);

            FindNearestThreatJob _findNearestThreatJob = new FindNearestThreatJob(){
                myPosition = transform.position,
                visibleCreatures = TempNativeArray, 
                closestThreatPosition = result
            };

            JobHandle findThreatJobHandle = _findNearestThreatJob.Schedule(); 
            findThreatJobHandle.Complete();     

            tempDictionary.TryGetValue(_findNearestThreatJob.closestThreatPosition[0], out Transform nearestThreat);

            TempNativeArray.Dispose();
            result.Dispose();       

            if(nearestThreat)
                return  nearestThreat;
            else
                return null;
        }


        // Simply Grabs the first food item it can that has value. Does not evaluate if it should eat this unknown food.
        public Transform FindEasyMeal()
        {
            Transform nextMeal = null;

                // Keep track of the value of food that has been dropped, will compare score to living options later.
                // It doesn't keep track of only the best, nor does it combine all foods values. It becomes harder to raise the score as it goes higher.
                foreach(Transform item in VisibleWorldItems)
                {
                    World_Item droppedItem = item.GetComponent<World_Item>();
                    
                    if(droppedItem.GetFoodValue() > 0)
                    {
                        nextMeal = item;
                        break;
                    }
                }

            return nextMeal;
        }

        public Transform FindTastiestCreature(CreatureController thisCreature)
        {
            Transform tastiestCreature = null;
            float bestValue = 0;
            float currentValue;

            foreach(Transform otherCreature in VisibleCreatures)
            {
                if(!otherCreature.GetComponent<CreatureController>())
                    continue;
                
                var otherCreatureSpecies = otherCreature.GetComponent<CreatureController>().creatureSpecies;
                
                if(otherCreatureSpecies == thisCreature.creatureSpecies)
                    continue;

                currentValue = otherCreature.GetComponent<CreatureController>().stats.proteinScore;
                Debug.Log(" Creature : " + otherCreatureSpecies.name + " is of value : " + currentValue );
                if(currentValue > bestValue)
                {
                    bestValue = currentValue;
                    tastiestCreature = otherCreature;
                }
            } 
            Debug.Log("Tastiest creature is : " + tastiestCreature + " : ");
            return tastiestCreature;
        }

        public bool FindTastiestCreature(CreatureController thisCreature, out Transform tastiestCreature)
        {
            tastiestCreature = null;
            float bestValue = 0;
            float currentValue;

            foreach(Transform otherCreature in VisibleCreatures)
            {
                if(!otherCreature.GetComponent<CreatureController>())
                    continue;

                var otherCreatureSpecies = otherCreature.GetComponent<CreatureController>().creatureSpecies;
                
                if(otherCreatureSpecies == thisCreature.creatureSpecies)
                    continue;

                currentValue = otherCreature.GetComponent<CreatureController>().stats.proteinScore;
                if(currentValue > bestValue)
                {
                    bestValue = currentValue;
                    tastiestCreature = otherCreature;
                }
            } 
            
            if(tastiestCreature)
                return true;
            else
                return false;
        }



        // Will attempt to engineer an algorithm to determine the BEST choice to target, in my given list of Food Sources.
        //
        // Use Utility Theory. 2 Considerations per target, their distance from me and their perceived "Food Value" (Will probably base it off their LootTable)
        // Normalize the values such that all are between 0 and 1.
        // 

        public Transform DetermineBestFoodSource()
        {
            float highestFinalScore = -1;
            Transform highestScoringChoice = null;
            float highestFoodScore = GetHighestRawFoodScore();

            foreach(Transform foodSource in VisibleFoodSources)
            {   
                if(ScoreFoodSource(foodSource, highestFoodScore, out float score) > highestFinalScore)
                {
                    highestScoringChoice = foodSource;
                    highestFinalScore = score;
                }
                Debug.Log(transform.root.name + " saw "+ foodSource.name + " with score of " + score);
            }
        
            return highestScoringChoice;
        }

        // Returns a normalized value based on 2 considerations, distance and foodvalue.
        public float ScoreFoodSource(Transform thisSource, float highestFoodScore, out float score)
        {
            List<float> tempScoreList = new List<float>
            {
                // Add all considerations of an individual to this list.
                // Check the distance from myself. Then check my foodvalue in relation to the highest one. All values must be between 0 and 1.
                ScoreDistance(thisSource),
                GetNormalizedFoodValue(thisSource, highestFoodScore)
            };

                NativeArray<float> considerationsTemp = new NativeArray<float>(tempScoreList.Count, Allocator.TempJob);
                considerationsTemp.CopyFrom(tempScoreList.ToArray());

                NativeArray<float> finalScoreTemp = new NativeArray<float>(1, Allocator.TempJob);

            ScoreFoodSourceJob _scoreJob = new ScoreFoodSourceJob(){
                modificationFactor = 1 - (1.0f / tempScoreList.Count),
                ConiderationScores = considerationsTemp,
                ArrayLength = tempScoreList.Count,
                FinalActionScore = finalScoreTemp
            };
             

            JobHandle ScoringJobHandle = _scoreJob.Schedule();
            ScoringJobHandle.Complete();
            
            score = _scoreJob.FinalActionScore[0];

            considerationsTemp.Dispose();
            finalScoreTemp.Dispose();
            
            return score;
        }

        // Returns the highest foodvalue without normalization
        private float GetHighestRawFoodScore()
        {
            float highestScore = 0;
            float score;
            foreach(Transform foodSource in VisibleFoodSources)
            {
                if(!foodSource)
                {
                    VisibleFoodSources.Remove(foodSource);
                    continue;
                }

                if(foodSource.root.tag == "Player")
                    score = foodSource.GetComponent<PlayerSystems.PlayerData>().GetFoodScore();
                else if(foodSource.gameObject.layer == 10)
                    score = foodSource.GetComponent<CreatureController>().creatureSpecies.GetFoodScore();
                else
                    score = foodSource.GetComponentInChildren<Spawnable_Loot>().GetFoodScore();

                if(score > highestScore)
                    highestScore = score;

            }
            return highestScore;
        }

        // Normalizes food score
        private float GetNormalizedFoodValue(Transform thisSource, float highestFoodScore)
        {
            float score = 0;

                if(thisSource.root.tag == "Player")
                    score = thisSource.GetComponent<PlayerSystems.PlayerData>().GetFoodScore();
                else if(thisSource.gameObject.layer == 10)
                    score = thisSource.GetComponent<CreatureController>().creatureSpecies.GetFoodScore();
                else
                    score = thisSource.GetComponentInChildren<Spawnable_Loot>().GetFoodScore();
            

            return score / highestFoodScore;
        }


        [BurstCompile]
        public struct ScoreFoodSourceJob : IJob
        {
            [ReadOnly] public float modificationFactor;
            [ReadOnly] public NativeArray<float> ConiderationScores;
            [ReadOnly] public int ArrayLength;
        
            public NativeArray<float> FinalActionScore;

            public void Execute()
            {
                float totalConsiderationScore = 1f;

                for(int i = 0; i < ArrayLength; i++)
                {

                    float makeUpValue = (1 - ConiderationScores[i]) * modificationFactor;
                    float finalScore = ConiderationScores[i] + (makeUpValue * ConiderationScores[i]);

                    totalConsiderationScore *= finalScore;

                    // If a consideration is zero, it has no point in computing further.
                    if(totalConsiderationScore == 0)
                    {
                        FinalActionScore[0] = 0;
                        break;
                    }
                }

                FinalActionScore[0] = totalConsiderationScore;
            }
        }






        // Using Unity's Job System for multi-threading calculations
        [BurstCompile]
        public struct FindNearestThreatJob : IJob
        {
            [ReadOnly] public float3 myPosition;
            [ReadOnly] public NativeArray<float3> visibleCreatures;

            public NativeArray<float3> closestThreatPosition;
            
            public void Execute()
            {   
                for(int index = 0; index < visibleCreatures.Length; index++)
                {
                    if(index == 0)
                        closestThreatPosition[0] = visibleCreatures[index];
                    else if(math.distancesq(visibleCreatures[index], myPosition) > math.distancesq(closestThreatPosition[0], myPosition))
                        closestThreatPosition[0] = visibleCreatures[index];                       
                }
            }
        }

        // Returns a normalized score of distance, in relation to my awareness range.
        public float ScoreDistance(Transform target)
        {
            NativeArray<float> finalScoreTemp = new NativeArray<float>(1, Allocator.TempJob);
            CalculateClosenessScoreJob scoreJob = new CalculateClosenessScoreJob(){
                currentPosition = transform.position,
                targetPosition = target.position,
                sqrLosingDistance = sqrLosingDistance,
                closenessScore = finalScoreTemp
            };
             

            JobHandle ScoringJobHandle = scoreJob.Schedule();
            ScoringJobHandle.Complete();     
            // How close are they in relation to my range of perception and myself? 
            float score;

            // The lower the distance the higher the score.
            score = responseCurve.Evaluate(scoreJob.closenessScore[0]);

            finalScoreTemp.Dispose();

            return score;
        }

        [BurstCompile]
        public struct CalculateClosenessScoreJob : IJob
        {
            public NativeArray<float> closenessScore;
            [ReadOnly] public float3 targetPosition;
            [ReadOnly] public float3 currentPosition;
            public float sqrLosingDistance;

            public void Execute()
            {
                closenessScore[0] = math.clamp(1 - math.lengthsq(targetPosition - currentPosition) / sqrLosingDistance, 0f, 1f);
            }
        }
    }
}
