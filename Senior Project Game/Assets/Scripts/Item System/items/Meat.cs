using UnityEngine;


using Cyrcadian;


    [CreateAssetMenu(fileName = "Meat", menuName = "2D Survial/Items/Meat")]
    public class Meat : Item
    {
        private AudioSource sound = new AudioSource();
        private int foodValue = 5; 

        public override bool CanUse(Vector3Int target)
        {
            //  return GameManager.Instance?.Terrain != null && GameManager.Instance.Terrain.IsTillable(target);
            return true;
        }

        public override bool Use(Vector3Int target, GameObject gameObject)
        { 
            // If the player uses this, Hunger is not a child gameobject.
            if(gameObject.GetComponent<PlayerData>())
                gameObject.GetComponentInChildren<PlayerData>().GetPlayerHunger().ChangeHunger(foodValue);
            else if(gameObject.GetComponentInChildren<HungerBar>())
                gameObject.GetComponentInChildren<HungerBar>().ChangeHunger(foodValue);

            PlaySound();
            return true;
        }
    }
