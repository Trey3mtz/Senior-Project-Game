using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Calm Down", menuName = "UtilityAI/Actions/Calm Down")]
    public class CalmDown : Action
    {
        public override void Execute(CreatureController thisCreature)
        {
           thisCreature.DoCalmDown();
        }
    }
}