using Cyrcadian.WorldTime;
using Cyrcadian.Creatures;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "CyrcadianRhythm", menuName = "UtilityAI/Consideration/CyrcadianRhythm")]
    public class CyrcadianRhythm_Consideration : Consideration
    {
        Day_Cycle DayCycle;

        [Header("Response curve for urgency")]
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(CreatureController thisCreature)
        {

            DayCycle = FindAnyObjectByType<Day_Cycle>();
            
            score = 0f;

            switch(thisCreature.creatureSpecies.CircadianRhythm) 
            {
                case Creature.CyrcadianRhythm.Nocturnal:
                    if(DayCycle.GetTimeOfDay() == 0)
                        score = 1f;
                    break;
                case Creature.CyrcadianRhythm.Diurnal:
                    if(DayCycle.GetTimeOfDay() == 1)
                        score = 1f;
                    break;
                case Creature.CyrcadianRhythm.Crepuscular:
                    if(DayCycle.GetTimeOfDay() == 2)
                        score = 1f;
                    break;
                case Creature.CyrcadianRhythm.Cathemeral:
                        score = 1f;
                    break;
                default:
                    break;
            }

            
            score = responseCurve.Evaluate(score);
            
            return score;
        }
    }
}
