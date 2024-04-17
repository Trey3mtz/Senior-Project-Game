using System.Collections;
using Cyrcadian.PlayerSystems;
using Cyrcadian.AttackSystem;
using UnityEngine;

namespace Cyrcadian.Items
{
    [CreateAssetMenu(fileName = "Tool", menuName = "2D Survival/Items/Tool")]
    public class Tool : Item
    {
        private GameObject obj;

        // Create a Tag named after the specific tool type, and fill this out when you make a new tool
        [SerializeField] string item_Tag;
        [SerializeField] int damageDealt;
        [SerializeField] AudioClip hitSFX;

        public override bool CanUse(Vector3Int target)
        {
            return true;
        }

        public override bool Use(Vector3Int target, GameObject gameObject)
        {  
            // Create the gameobject of this tool, set it's tag to what tool type it is (Axe, Pickaxe,etc)
            obj = new GameObject("Tool_Collider", typeof(CircleCollider2D))
            {
                layer = 13,
                tag = item_Tag
            };
            obj.transform.position = gameObject.transform.position;
            obj.transform.parent = gameObject.transform.GetChild(0);
            obj.transform.localRotation = gameObject.transform.localRotation;


            // Set it's hitbox
            var hitbox = obj.GetComponent<CircleCollider2D>();
            hitbox.isTrigger = true;
            hitbox.offset = new Vector2(0.5f,0.75f);
            
            // If this tool is can double as a weapon as well, set it's attack properties
            if(this.Type == ItemType.Weapon)
            {
                obj.AddComponent<Damage_HealthBar>();
                obj.GetComponent<Damage_HealthBar>().damage = damageDealt;
                obj.GetComponent<Damage_HealthBar>().SFX = hitSFX;
            }

            gameObject.GetComponentInChildren<Player_animation_logic>().tool.GetComponent<SpriteRenderer>().sprite = ItemSprite;
            gameObject.GetComponentInChildren<Player_animation_logic>().GenericToolSwing();



            Destroy(obj,0.55f);
            PlaySound();

            return true;
        }            


    }  
}
