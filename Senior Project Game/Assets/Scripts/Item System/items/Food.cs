using Cyrcadian.PlayerSystems;
using UnityEngine;


namespace Cyrcadian.Items
{
    [CreateAssetMenu(fileName = "Food", menuName = "2D Survial/Items/Food")]
    public class Food : Item
    {
        [SerializeField] int foodValue = 5; 
        [SerializeField] GameObject foodParticles;
        
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
    }
}



