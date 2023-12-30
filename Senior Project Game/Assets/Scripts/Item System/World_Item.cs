using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cyrcadian
{
public class World_Item : MonoBehaviour
{
    public static World_Item Instance {get; private set;}

    public static World_Item SpawnWorldItem(Vector3 position, Item item)
    {
        Transform transform = Instantiate(Instance.World_Item_prefab, position, Quaternion.identity);
        World_Item worldItem = transform.GetComponent<World_Item>();
        worldItem.SetItem(item);

        return worldItem;
    }
        public Transform World_Item_prefab;
        private Item item;
        private SpriteRenderer spriteRenderer;
        private TextMeshPro textMeshPro;
        
    private void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.ItemSprite;
        if(item.amount > 1)
            textMeshPro.SetText(item.amount.ToString());
        else
            textMeshPro.SetText("");
    }

    public Item GetItem()
    {
        return item;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
}