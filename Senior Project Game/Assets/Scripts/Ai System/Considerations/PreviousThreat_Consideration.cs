using System.Collections;
using System.Collections.Generic;
using Cyrcadian.Creatures;
using Unity.VisualScripting;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Previous Threat", menuName = "UtilityAI/Consideration/Previous Threat")]
    public class PreviousThreat_Consideration : Consideration
    {
        
        [Header("Response curve for urgency")]
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(CreatureController thisCreature)
        {
            
            // If there is a threat nearby, find out how close it is. Zero means he's touching me, and over 1 is far away
            // Also tracks how much of a threat they were.
            if(thisCreature.awareness.IsThreatNearby())
            {
                float sqrLosingDistance = thisCreature.awareness.sqrLosingDistance;
                Transform nearestThreat = thisCreature.awareness.FindNearestThreat();
                if(!nearestThreat)
                {
                    score = 1;
                }
                else
                {
                thisCreature.awareness.KnownThreats.TryGetValue(nearestThreat, out Awareness.CreatureData creatureData);
               

                // Get normalized scores based on distance they are from you, and how much health they beat out of you.
                // Give distance a score from 0 to 1.5 but clamp its value at one. Subtract up to 0.5 points based on how much their health they've taken from you is.
                float scoreDistanceAway = Mathf.Clamp01((thisCreature.awareness.FindNearestThreat().position - thisCreature.transform.position).sqrMagnitude / sqrLosingDistance * 0.5f);
                float scoreHealthInfluenced = Mathf.Clamp01( creatureData.healthInfluenced / thisCreature.stats.healthPool) * 0.5f;


                score = scoreDistanceAway - scoreHealthInfluenced;}
            }
            else
                score = 1;
            
            
            score = responseCurve.Evaluate(score);

            return score;
        }
    }
}