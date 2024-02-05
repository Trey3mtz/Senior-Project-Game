using UnityEngine;

namespace Cyrcadian.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "FindFood", menuName = "UtilityAI/Actions/FindFood")]
    public class FindFood : Action
    {
        public override void Execute(CreatureController thisCreature)
        {
            thisCreature.DoFindFood();// Place logic to sleep here
        }
    }
}

