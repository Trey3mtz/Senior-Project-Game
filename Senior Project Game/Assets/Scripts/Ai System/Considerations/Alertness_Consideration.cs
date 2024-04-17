using System.Collections;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Alertness", menuName = "UtilityAI/Consideration/Alertness")]
    public class Alertness_Consideration : Consideration
    {
        [Header("Response curve for urgency")]
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(CreatureController thisCreature)
        {
            if(thisCreature.alertness == CreatureController.AlertState.Alert)
                score = 1f;
            else if(thisCreature.alertness == CreatureController.AlertState.Calm)
                score = .5f;
            else
                score = 0f;
        

            score = responseCurve.Evaluate(score);
          
            return score;
        }
    }
}
