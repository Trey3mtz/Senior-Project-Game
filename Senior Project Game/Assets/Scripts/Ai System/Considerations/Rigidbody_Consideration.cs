using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Rigidbody", menuName = "UtilityAI/Consideration/Rigidbody")]
    public class Rigidbody_Consideration : Consideration
    {
        // The left end of the curve is near 0 movement, and right means lots of movement
        [Header("Response curve for urgency")]
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(CreatureController thisCreature)
        {

            score = Mathf.Clamp01(thisCreature.mover.rb.velocity.sqrMagnitude);
            
            score = responseCurve.Evaluate(score);

            return score;
        }
    }
}