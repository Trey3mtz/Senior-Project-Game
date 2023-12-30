using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Cyrcadian
{

public class camera_logic : MonoBehaviour
{

    [SerializeReference] GameObject Parent;
    [SerializeReference] PlayerController playerController;
    Transform parent;

    //[SerializeField] GameObject forcedLookAt;
    public float offSetY = 0;
    public float offSetX = 0;

    // How close the camera stays to the player's body
    public int PlayerHugFactor = 10;

    Vector2 parentPoint;
    Vector2 halfPoint;

    // Start is called before the first frame update
    void Start()
    {
        playerController = this.gameObject.GetComponentInParent<PlayerController>();
        parent = Parent.GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        parentPoint = parent.position;

        halfPoint = playerController.lookdirection - parentPoint;
        halfPoint = halfPoint/PlayerHugFactor;
        halfPoint += (Vector2)parentPoint;
        halfPoint.y += offSetY;
        halfPoint.x += offSetX;

        // if the Player is not forced to stop in place, look at point is close to where mouse is
        //    if(!Parent.GetComponent<Player>().mandatedStop)
        transform.position = halfPoint;


    }
}
}