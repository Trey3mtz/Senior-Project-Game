using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScroll : MonoBehaviour
{
    [SerializeField]
    private float parallaxSpeed;

    private Transform cameraTransform;
    private float startPosX;
    private float spriteSizeX;

    // Start is called before the first frame update
    private void Start()
    {
        cameraTransform = Camera.main.transform;
        startPosX = transform.position.x;
        spriteSizeX = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float relativeDistance = cameraTransform.position.x * parallaxSpeed;
        transform.position = new Vector3(startPosX + relativeDistance, transform.position.y, transform.position.z);
    
        // Loop parallax effect here
        float relativeCameraDistance = cameraTransform.position.x * (1 - parallaxSpeed);

        if(relativeCameraDistance > startPosX + spriteSizeX)
            startPosX += spriteSizeX;  
        else if (relativeCameraDistance < startPosX - spriteSizeX)
            startPosX -= spriteSizeX;
        
    }
}
