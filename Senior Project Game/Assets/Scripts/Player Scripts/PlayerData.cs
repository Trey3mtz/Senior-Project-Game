using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cyrcadian.PlayerSystems.InventorySystem;


namespace Cyrcadian
{
    /*********************************************************************************
        [Attach this to Player]
        
        This script is incharge of overseaing Player savedata.

    */


    // Properties of the Player we desire to save (i.e. world position and inventory)
    [System.Serializable]
    public struct PlayerSaveData
    {
        public Vector3 Position;   
        public List<InventorySaveData> Inventory;

        public int Health, Hunger, Oxygen;
    }


        public class PlayerData : MonoBehaviour
        {
            public Inventory Inventory => _inventory;
            
            [SerializeField] private Inventory_UI UI_inventory;
            [SerializeField] private Inventory _inventory;
            [SerializeField] private HealthBar _healthBar;
            [SerializeField] private HungerBar _hungerBar;
            [SerializeField] private HungerBar _oxygenBar;

            private Collect_World_Item collect_item;

            
            public void Awake()
            {       
               _inventory.InitializeInventory();
               
                if (GameManager.Instance.PlayerData != null)
                {   
                    Destroy(gameObject);
                    return;
                }
                                
                collect_item = GetComponentInChildren<Collect_World_Item>();
                // IF there isn't another Player with playerdata, dont destroy this object on Load. 
                // For debugging, in case someone accidentally places a second player down.
                //if(!FindAnyObjectByType<PlayerData>())
                    GameManager.Instance.PlayerData = this;
                    //DontDestroyOnLoad(gameObject);
 
            }

            private void Start()
            {
                UI_inventory.SetInventory(GetSavedInventory());
                collect_item.SetInventory(GetSavedInventory());
            }
            
            public void Save(ref PlayerSaveData data)
            {
                data.Position = GetComponent<Transform>().position;
                data.Inventory = new List<InventorySaveData>();
                _inventory.Save(ref data.Inventory);

                data.Health = _healthBar.currentHP();
                data.Hunger = _hungerBar.CurrentHunger();
                data.Oxygen = _oxygenBar.CurrentHunger();
            }

            public void Load(PlayerSaveData data)
            {
                _inventory.Load(data.Inventory);
                GetComponent<Transform>().position = data.Position;

                _healthBar.SetHealth(data.Health);
                _hungerBar.SetHunger(data.Hunger);
                _oxygenBar.SetHunger(data.Oxygen);
            }

            public Inventory GetSavedInventory()
            {   
                return _inventory;
            }

            public HungerBar GetPlayerHunger()
            {
                return _hungerBar;
            }

            public HungerBar GetPlayerOxygen()
            {
                return _oxygenBar;
            }
        }
}