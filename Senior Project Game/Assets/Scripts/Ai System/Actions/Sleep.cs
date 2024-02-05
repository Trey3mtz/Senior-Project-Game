using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Sleep", menuName = "UtilityAI/Actions/Sleep")]
    public class Sleep : Action
    {
        public override void Execute(CreatureController thisCreature)
        {
            if(thisCreature.alertness != CreatureController.AlertState.Asleep && thisCreature.alertness != CreatureController.AlertState.Alert)
                thisCreature.DoSleep();
        }
    }
}

