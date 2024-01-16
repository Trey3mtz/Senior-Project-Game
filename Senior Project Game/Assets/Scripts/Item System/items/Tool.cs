using System.Collections;
using Cyrcadian.PlayerSystems;
using DG.Tweening;
using UnityEngine;

namespace Cyrcadian.Items
{
    [CreateAssetMenu(fileName = "Tool", menuName = "2D Survial/Items/Tool")]
    public class Tool : Item
    {
        private GameObject obj;

        // Create a Tag named after the specific tool type, and fill this out when you make a new tool
        [SerializeField] string item_Tag;

        public override bool CanUse(Vector3Int target)
        {
            return true;
        }

        public override bool Use(Vector3Int target, GameObject gameObject)
        {
            obj = new GameObject("Tool_Collider", typeof(CircleCollider2D))
            {
                layer = 13,
                tag = item_Tag
            };
            obj.transform.position = gameObject.transform.position;
            obj.transform.parent = gameObject.transform.GetChild(0);
            obj.transform.localRotation = gameObject.transform.localRotation;

            var hitbox = obj.GetComponent<CircleCollider2D>();
            hitbox.isTrigger = true;
            hitbox.offset = new Vector2(0.5f,0.75f);
            

            gameObject.GetComponentInChildren<Player_animation_logic>().tool.GetComponent<SpriteRenderer>().sprite = ItemSprite;
            gameObject.GetComponentInChildren<Player_animation_logic>().GenericToolSwing();



            Destroy(obj,0.25f);
            PlaySound();

            return true;
        }            


    }  
}
