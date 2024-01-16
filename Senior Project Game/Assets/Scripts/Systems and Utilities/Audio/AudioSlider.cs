using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyrcadian
{
    public class AudioSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        
        // Start is called before the first frame update
        void Start()
        {
            slider = GetComponent<Slider>();
            slider.onValueChanged.AddListener(val => AudioManager.Instance.ChangeMasterVolume(val));
        }

    }
}
