using Cyrcadian.PlayerSystems;
using Cyrcadian.Creatures;
using UnityEngine;


namespace Cyrcadian.Items
{
    [CreateAssetMenu(fileName = "Food", menuName = "2D Survival/Items/Food")]
    public class Food : Item
    {
        // Useful for different types of meat eaters and vegi eaters.
        // Can Add more types if needed like fish, grasses, seeds, etc.
        public enum FoodType{
            Meats,
            Fishes,
            Bugs,
            Vegetables,
            Fruits,
            grasses,
            NonEdibles
        }

        [SerializeField] public int foodValue = 5; 
        [SerializeField] GameObject foodParticles;
        [SerializeField] FoodType foodType = FoodType.NonEdibles;
        
        public override bool CanUse(Vector3Int target)
        {
            return true;
        }

        public override bool Use(Vector3Int target, GameObject gameObject)
        { 
            // If the player uses this, Hunger is not a child gameobject.
            if(gameObject.GetComponent<PlayerData>())
                gameObject.GetComponentInChildren<PlayerData>().GetPlayerHunger().ChangeHunger(foodValue);
            else if(gameObject.GetComponentInChildren<HungerBar>())
                gameObject.GetComponentInChildren<HungerBar>().ChangeHunger(foodValue);

            gameObject.GetComponentInChildren<Player_animation_logic>().Nom();
            var particles = Instantiate(foodParticles);
            particles.transform.position = gameObject.transform.position + new Vector3(0,1);

            PlaySound(1.5f);
            return true;
        }

        public FoodType GetFoodType()
        {
            return foodType;
        }

        public override int GetFoodValue()
        {
            return foodValue;
        }
    }
}



