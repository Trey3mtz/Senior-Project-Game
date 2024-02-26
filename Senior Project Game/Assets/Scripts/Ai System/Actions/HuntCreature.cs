using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Hunt Creature", menuName = "UtilityAI/Actions/Hunt Creature")]
    public class HuntCreature : Action
    {
        public override void Execute(CreatureController thisCreature)
        {
            thisCreature.DoHunt();
        }
    }
}

