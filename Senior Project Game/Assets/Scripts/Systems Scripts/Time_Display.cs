using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WorldTime
{
    [RequireComponent(typeof(TMP_Text))]
    
    public class Time_Display : MonoBehaviour
    {
        [SerializeField] 
        private Time_World world_time;

        private TMP_Text text;

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            world_time.WorldTimeChanged += OnWorldTimeChanged;
        }

        private void OnDestroy()
        {
            world_time.WorldTimeChanged -= OnWorldTimeChanged;
        }

        private void OnWorldTimeChanged(object sender, TimeSpan newTime)
        {
            text.SetText(newTime.ToString(@"hh\:mm"));
        }
    
    }
}

