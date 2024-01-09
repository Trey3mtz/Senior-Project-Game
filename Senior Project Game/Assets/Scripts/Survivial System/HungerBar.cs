using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyrcadian
{
    public class HungerBar : MonoBehaviour
    {
        [SerializeField] int StomachSize = 10;
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

        private bool isPlayer = false;

        private void Awake()
        {
            if(this.gameObject.CompareTag("Player"))
                isPlayer = true;    
        }

        // Right now, hunger is set to full on start, but ideally is saved and loaded
        // Will try make this work with both player and non-player things ideally
        // Player will have this visible in HUD but not the npcs
        private void Start()
        {
            _hunger = StomachSize;
            SetMaxHunger();
            StartCoroutine(hungerTickDown());    
        }

        public void ChangeHunger(int amount)
        {
            if(amount > 0)
                recentlyFed = true;

            _hunger = Mathf.Clamp(_hunger + amount, 0, StomachSize);

            if(isPlayer)
                VisualizeHungerChange();
        }

        public int CurrentHunger()
        {
            return _hunger;
        }

        // For the Player's HUD
        private void SetMaxHunger()
        {
            slider.maxValue = StomachSize;
            slider.value = _hunger;

            fill.color = gradient.Evaluate(1f);
        }
        
        // For the Player's HUD
        private void VisualizeHungerChange()
        {
            slider.value = _hunger;
            fill.color = gradient.Evaluate(slider.normalizedValue);
        }

        IEnumerator hungerTickDown()
        {
            yield return new WaitForSeconds(hungerTickRate);
            ChangeHunger(-1);

            if(!recentlyFed)
                StartCoroutine(hungerTickDown());
            else
                StartCoroutine(recentlyFedTimer());
        }

        IEnumerator recentlyFedTimer()
        {
            yield return new WaitForSeconds(recentlyFedDelay);
            recentlyFed = false;
            StartCoroutine(hungerTickDown());
        }
    }
}

