using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Stamina", menuName = "UtilityAI/Consideration/Stamina")]
    public class Stamina_Consideration : Consideration
    {
        [Header("Response curve for urgency")]
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(CreatureController thisCreature)
        {
            if(thisCreature.stats.staminaPool != 0)
                score = responseCurve.Evaluate(thisCreature.stats.currentStamina/thisCreature.stats.staminaPool);

            return score;
        }
    }
}