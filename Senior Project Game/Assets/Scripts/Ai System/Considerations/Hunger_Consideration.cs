using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Hunger", menuName = "UtilityAI/Consideration/Hunger")]
    public class Hunger_Consideration : Consideration
    {
        [Header("Response curve for urgency")]
        [Tooltip("The more hungry a creature is, the greater the urgency to eat is")]
        [SerializeField] private AnimationCurve responseCurve;

         public override float ScoreConsideration(CreatureController thisCreature)
         {
            score = responseCurve.Evaluate(Mathf.Clamp01(thisCreature.stats.currentHunger / 100f));
            return score;
         }
    }
}
