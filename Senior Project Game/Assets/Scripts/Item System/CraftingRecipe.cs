using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian.Items
{
    [CreateAssetMenu(fileName = "Crafting Recipe", menuName = "2D Survial/Crafting Recipe")]
    public class CraftingRecipe : ScriptableObject
    {

        /// <summary>
        ///                   CAUTION:      Please, do not add more than one of the same item into a recipe's requirements.
        ///                                 It won't subtract items correctly, and it wouldn't make sense either.
        ///                                 Since it's not a runtime concern, there are no safe guards against doing this.
        /// </summary>

        [Serializable]
        public struct RecipeComponent
        {
            public Item item;
            public int amount;
        }

        [Tooltip("Recipes are an array of items and the amount needed of that item")]
        public RecipeComponent[] Recipe;

        [Tooltip("The resulting item of this recipe")]
        public Item RecipeResult;

        [Tooltip("How much of the result we want to give back")]
        public int ResultAmount = 1;


        // If we Can Craft this recipe, returns a list of all indexes needing to be taken from
        private List<List<int>> FindRecipeComponents(Inventory searchInventory)
        {
            // This variable will be used to keep track of item amounts accross multiple stacks
            int itemCounter;
            // Keeps track of a specific index in the searched inventory
            int index;
            // A list for all the indexes we will take from in the inventory
            List<int> entryIndexes;
            // A list for the list of indexes, so each component will have its known indexes to take from when satisfied
            List<List<int>> satisfiedRecipeComponents = new();

            // Go through the recipe and check for each component
            for(int r_Component = 0; r_Component < Recipe.Length; r_Component++)
            {
                // Start a fresh search for each recipe component in this recipe
                itemCounter = Recipe[r_Component].amount;
                index = -1;
                entryIndexes = new();

                // Go through inventory until we know there is enough for this component in the recipe
                foreach(Inventory.InventoryEntry entry in searchInventory.GetInventory())
                {
                    // Add index early as we can break out of the loop early
                    index++;

                    // Ignore null entries
                    if(!entry.item)
                        continue;

                    // If matching items, Make note of where this index is, and subtract from the amount needed left
                    if(entry.item == Recipe[r_Component].item)
                    {   
                        entryIndexes.Add(index);
                        itemCounter -= entry.stackSize;                       
                    }    

                    // We have enough of this component, add list of indexes to satisfied components
                    if(itemCounter <= 0)
                    {
                        satisfiedRecipeComponents.Add(entryIndexes);
                        break;
                    }
                }
            }
            return satisfiedRecipeComponents;
        }

        // If we satisfy all crafting recipe components, returns the indexes of the items we can pull out of our inventory
        private List<List<int>> CanCraft(Inventory searchInventory)
        {
            List<List<int>> matching_recipe_components = FindRecipeComponents(searchInventory);

            // For future testing 
            //Debug.Log("Recipe needs " + Recipe.Length + " different components");
            //Debug.Log("I have satisfied " + matching_recipe_components.Count+ " of those components");

            if(matching_recipe_components.Count == Recipe.Length)
               return matching_recipe_components;
            else
                return null; 
        }

        // Returns false if we can't craft the item, else remove the items needed to make the recipe and give us that item, then returning true;
        public bool CraftItem(Inventory inventory)
        {
            List<List<int>> matching_recipe_components = new();
            matching_recipe_components = CanCraft(inventory);
            if(matching_recipe_components == null)
                return false;
            
            int recipeIndex = 0;
            int requiredAmountLeft;
            int amountInStack;
            foreach(List<int> indexes in matching_recipe_components)
            {
                requiredAmountLeft = Recipe[recipeIndex].amount;
                foreach(int index in indexes)
                {   
                    amountInStack = inventory.GetInventory()[index].stackSize;
                    inventory.DecrementItemByAmount(index, requiredAmountLeft);
                    requiredAmountLeft -= amountInStack;
                }
                recipeIndex++;
            }

            inventory.AddItem(RecipeResult, ResultAmount);

            return true;
        }
    }


}
