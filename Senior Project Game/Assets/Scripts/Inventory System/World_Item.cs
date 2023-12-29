using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class World_Item : MonoBehaviour
{

    public static World_Item SpawnWorldItem(Vector3 position, Item item)
    {
        Transform transform = Instantiate(Item_Assets.Instance.World_Item_prefab, position, Quaternion.identity);
        World_Item worldItem = transform.GetComponent<World_Item>();
        worldItem.SetItem(item);

        return worldItem;
    }

        private Item item;
        private SpriteRenderer spriteRenderer;
        private TextMeshPro textMeshPro;
        
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.GetSprite();
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
