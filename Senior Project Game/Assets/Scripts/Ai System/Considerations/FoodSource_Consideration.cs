using Cyrcadian.Creatures;
using Mono.Cecil.Cil;
using UnityEngine;

namespace Cyrcadian.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "Diet", menuName = "UtilityAI/Consideration/Diet")]
    public class FoodSource_Consideration : Consideration
    {
        [Header("Response curve for urgency")]
        [Tooltip("Looks at information for the best choice in nearby Food Sources.")]
        [SerializeField] private AnimationCurve responseCurve;

         public override float ScoreConsideration(CreatureController thisCreature)
         {
            // Graze Action is intended for Herbivores. Hunt Action is intended for Carnivores. FindFood Action is intended for Omnivores.

            // This consideration is intended to inluence what their next meal choice should be based on information they know about their
            // Immediate environment.
            switch(thisCreature.creatureSpecies.Diet)
            {
                case Creature.DietarySystem.Herbivore:
                        score=0;
                    break;

                case Creature.DietarySystem.Carnivore:
                        score=1;
                    break;
                    
                case Creature.DietarySystem.Omnivore:
                    // Fill this out with thisCreature's Awareness of nearby foodsources.
                    // Determine what is easiest, for the most bang for buck. Hunting should consider that creature's will give more hunger back.
                    // Creature's have a GetProteinScore method to calulate how much food they could drop. 
                    // Dropped items will know
                    // But they also consider it takes energy to hunt, and that things can fight back.
                    // Depending on how easy the next meal is, they might be more likely to side on the Carnivore, or Herbivore behavior.

                    // Set to between the middle of both values
                    score = 0.5f;

                    float droppedFoodWorth = 0f;
                    // Keep track of the value of food that has been dropped, will compare score to living options later.
                    // It doesn't keep track of only the best, nor does it combine all foods values. It becomes harder to raise the score as it goes higher.
                    foreach(Transform item in thisCreature.awareness.VisibleWorldItems)
                    {
                        World_Item droppedItem = item.GetComponent<World_Item>();
                        float value = droppedItem.GetFoodValue();
                        if(value >= droppedFoodWorth)
                            droppedFoodWorth += value;
                    }

                    float livingFoodWorth = 0f;
                    // Keep track of the only the best value of a creature. 
                    foreach(Transform creature in thisCreature.awareness.VisibleCreatures)
                    {
                        CreatureController otherCreature = creature.GetComponent<CreatureController>();
                        
                        // NO CANIBALISM PLS
                        if(otherCreature.creatureSpecies == thisCreature.creatureSpecies)
                            continue;

                        float value = otherCreature.creatureSpecies.Stats.proteinScore;
                        if(value > livingFoodWorth)
                        {
                           livingFoodWorth = value; 
                        }
                    }

                    livingFoodWorth = (livingFoodWorth + (thisCreature.stats.stomachSize / thisCreature.stats.currentHunger + 0.01f)) * (thisCreature.stats.currentStamina / thisCreature.stats.staminaPool);

                    score = Mathf.Clamp01((score + livingFoodWorth) - (score - droppedFoodWorth));

                    break;
                default:
                    break;
            }


            return responseCurve.Evaluate(score);
         }
    }
}
