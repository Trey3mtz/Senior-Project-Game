using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Was Harmed", menuName = "UtilityAI/Consideration/Was Harmed")]
    public class WasHarmed_Consideration : Consideration
    {
        [Header("Response curve for urgency")]
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(CreatureController thisCreature)
        {  
            if(thisCreature.health.wasRecentlyAttacked)
                score = 1f;
            else
                score = 0f;
            
            score = responseCurve.Evaluate(score);

            return score;
        }
    }
}