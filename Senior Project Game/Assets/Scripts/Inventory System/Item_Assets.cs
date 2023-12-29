using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Assets : MonoBehaviour
{
    public static Item_Assets Instance {get; private set;}

    private void Awake()
    {
        Instance = this;
    }

    public Transform World_Item_prefab;

    public Sprite meatSprite;
    public Sprite woodSprite;
    public Sprite weaponSprite;
    public Sprite medicineSprite;
}
