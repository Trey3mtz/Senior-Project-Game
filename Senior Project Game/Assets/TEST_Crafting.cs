using Cyrcadian.PlayerSystems;
using UnityEngine;

namespace Cyrcadian.Items
{
    public class TEST_Crafting : MonoBehaviour
    {
        [SerializeField] CraftingRecipe recipe;
        private Inventory inventory;

        void Start()
        {
            this.inventory = FindAnyObjectByType<PlayerData>().GetSavedInventory();
        }

        public void CraftItem()
        {
            recipe.CraftItem(inventory);
        }
        
    }
}
