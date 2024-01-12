using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cyrcadian.WorldTime
{
//execute late to be sure the manager are instantiated 
    [DefaultExecutionOrder(999)]
    [ExecuteInEditMode]
    public class ShadowInstance : MonoBehaviour
    {
        [Range(0, 10f)] public float BaseLength = 1f;

        private void OnEnable()
        {
            DayCycleHandler.RegisterShadow(this);
        }

        // Enrique: No clue what this is for, but this code throws a "some objects were not cleaned up" error
        private void OnDisable()
        {
            //DayCycleHandler.UnregisterShadow(this);
        }
    }
}