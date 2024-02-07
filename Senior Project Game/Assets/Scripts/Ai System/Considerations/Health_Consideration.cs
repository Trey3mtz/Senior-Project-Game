using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Health", menuName = "UtilityAI/Consideration/Health")]
    public class Health_Consideration : Consideration
    {
        [Header("Response curve for urgency")]
        [Tooltip("The lower the health of a creature is, the greater the urgency")]
        [SerializeField] private AnimationCurve responseCurve;

         public override float ScoreConsideration(CreatureController thisCreature)
         {
            if(thisCreature.stats.healthPool != 0)
                score = responseCurve.Evaluate(Mathf.Clamp01(thisCreature.stats.currentHP / thisCreature.stats.healthPool));

           
            return score;
         }
    }
}
