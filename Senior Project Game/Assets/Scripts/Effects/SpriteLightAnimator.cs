using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Cyrcadian
{
    public class SpriteLightAnimator : MonoBehaviour
    {
        private Light2D spriteLight;
        [SerializeField] SpriteRenderer spriteRenderer;

        void Start()
        {
            spriteLight = GetComponent<Light2D>();
        }


        // Update is called once per frame
        void LateUpdate()
        {
            spriteLight.lightCookieSprite = spriteRenderer.sprite;
        }
    }
}
