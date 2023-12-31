using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;

namespace Cyrcadian
{
public class World_Item : MonoBehaviour
{
    public static void SpawnWorldItem(Vector3 position, Item item)
    { 
        GameObject spawnedItem = Instantiate(item.WorldItemPrefab, position, Quaternion.identity);

        spawnedItem.GetComponent<World_Item>().SetItem(item);
    }

    public static void SpawnWorldItem(Vector3 position, Item item, int amount)
    {
        GameObject spawnedItem = Instantiate(item.WorldItemPrefab, position, Quaternion.identity);
        
        spawnedItem.GetComponent<World_Item>().amountDroped = amount;
        spawnedItem.GetComponent<World_Item>().SetItem(item);
    }
        
        private Item item;
        private SpriteRenderer spriteRenderer;
        private TextMeshPro textMeshPro;
        private int amountDroped;
        private Collider2D hitbox;
        
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
        hitbox = GetComponent<Collider2D>();
        amountDroped = 1;
    }

    public void SetItem(Item item)
    {
        this.item = item;
        spriteRenderer.sprite = item.ItemSprite;
        if(amountDroped > 1)
            textMeshPro.SetText(amountDroped.ToString());
        else
            textMeshPro.SetText("");
        StartCoroutine(WaitToPickUp());
    }

    public Item GetItem()
    {
        return item;
    }

    public int GetAmount()
    {
        return amountDroped;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public IEnumerator WaitToPickUp()
    {
        hitbox.enabled = false;
        yield return new WaitForSeconds(1.2f);
        hitbox.enabled = true;
    }
}
}