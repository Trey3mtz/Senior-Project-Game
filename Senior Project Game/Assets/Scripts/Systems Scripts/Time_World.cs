using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WorldTime
{
    public class Time_World : MonoBehaviour
    {
        public event EventHandler<TimeSpan> WorldTimeChanged;
 
        [SerializeField]
        private float day_length;   //measured in seconds

        private TimeSpan current_time;
        private float minute_length => day_length/Time_Constants.MinutesInDay;
    
        private void Start()
        {
            StartCoroutine(AddMinute());
        }

        private IEnumerator AddMinute()
        {
            current_time += TimeSpan.FromMinutes(1);
            WorldTimeChanged?.Invoke(this, current_time);
            yield return new WaitForSeconds(minute_length);
            StartCoroutine(AddMinute());
        }
    
    }
}
