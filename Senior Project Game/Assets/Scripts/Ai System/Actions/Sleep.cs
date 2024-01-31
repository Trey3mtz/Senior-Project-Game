using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Sleep", menuName = "UtilityAI/Actions/Sleep")]
    public class Sleep : Action
    {
        public override void Execute(CreatureController thisCreature)
        {
            thisCreature.DoSleep(3);// Place logic to sleep here
        }
    }
}

