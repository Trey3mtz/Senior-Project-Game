using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Cyrcadian.Creatures
{
    public class HungerBar : MonoBehaviour
    {
        [SerializeField] public int StomachSize = 10;
        [SerializeField] int _hunger;
        [SerializeField] bool recentlyFed = false;

        [Header("Seconds waited before ticking -1 hunger")]
        [SerializeField] float hungerTickRate = 2f;
        [Header("Seconds before hunger starts ticking again after eating")]
        [SerializeField] float recentlyFedDelay = 5f;

        [Header("This section is for Player only")]
        [SerializeField] Slider slider;
        [SerializeField] Gradient gradient;
        [SerializeField] Image fill;

        [Header("Starving Events")]
        [SerializeField] UnityEvent Starved;

        private bool isPlayer = false;

        private void Awake()
        {
            if(this.gameObject.CompareTag("Player"))
                isPlayer = true;    
        }

        // Hunger should be loaded in by a seperate script
        // Will try make this work with both player and non-player things ideally
        // Player will have this visible in HUD but not the npcs
        private void Start()
        {
            if(_hunger <= 0)
                _hunger = StomachSize;

            StartCoroutine(hungerTickDown());    
        }

        public void ChangeHunger(int amount)
        {
            if(amount > 0)
                recentlyFed = true;

            _hunger = Mathf.Clamp(_hunger + amount, 0, StomachSize);

            if(isPlayer)
                VisualizeHunger();
        }

        public int GetStomachSize()
        {   return StomachSize; }

        public int CurrentHunger()
        {   return _hunger;     }
        

        // For the Player's HUD
        public void SetHunger(int value)
        {
            _hunger = value;

            if(isPlayer)
                VisualizeHunger();
        }
        
        // For the Player's HUD
        private void VisualizeHunger()
        {
            slider.value = _hunger;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        IEnumerator starvation()
        {
            Starved.Invoke();
            yield return new WaitForSeconds(1);
            if(_hunger > 0 )
                StartCoroutine(hungerTickDown());
            else
                StartCoroutine(starvation());
        }

        IEnumerator hungerTickDown()
        {
            yield return new WaitForSeconds(hungerTickRate);
            ChangeHunger(-1);
            
            if(!recentlyFed && _hunger > 0)
                StartCoroutine(hungerTickDown());
            else if(recentlyFed)
                StartCoroutine(recentlyFedTimer());
            else if(_hunger <= 0)
                StartCoroutine(starvation());
        }

        IEnumerator recentlyFedTimer()
        {
            yield return new WaitForSeconds(recentlyFedDelay);
            recentlyFed = false;
            StartCoroutine(hungerTickDown());
        }
    }
}

