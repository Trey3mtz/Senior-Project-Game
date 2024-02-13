using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

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

    public static void SpawnWorldItem(Vector3 position, Item item, Vector3 forceDirection)
    {
        GameObject spawnedItem = Instantiate(item.WorldItemPrefab, position, Quaternion.identity);

        spawnedItem.GetComponent<World_Item>().SetItem(item);
        spawnedItem.GetComponent<Rigidbody2D>().AddForce(forceDirection); 
    }
        
        private Item item;
        private SpriteRenderer spriteRenderer;
        private TextMeshPro textMeshPro;
        private int amountDroped;
        private Collider2D hitbox;
        //private Animator animator;
        private Rigidbody2D rb;

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

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        hitbox = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        amountDroped = 1;

        transform.position += new Vector3(Random.value * 0.5f, Random.value* 0.15f);
    }

    public Item GetItem()
    {   return item;    }
       
    public int GetAmount()
    {   return amountDroped;    }

    public void DestroySelf()
    {    Destroy(gameObject);    }

    public float GetFoodValue()
    {
        // If not food, return a value of 0.
        if(item.Type != Item.ItemType.Food)
            return 0f;

        float foodValue = item.hasFoodValue() * amountDroped;

        

        return foodValue;
    }
    

    public IEnumerator WaitToPickUp()
    {
        hitbox.enabled = false;
        yield return new WaitForSeconds(1f);
        hitbox.enabled = true;
    }


    void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.gameObject.layer == 15)
        {   
            Vector3 direction = (gameObject.transform.position - collider.gameObject.transform.position).normalized;

            direction += new Vector3(Random.value, Random.value)*.5f;

            rb.velocity = (direction);
        }
    }
}
}