using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grab_script : MonoBehaviour
{

    [SerializeField] Collider2D grabHitbox;
    [SerializeField] DistanceJoint2D thisJoint;
    [SerializeField] GameObject parentObject;

    public bool isInRange = false;

    // NOTE: This grabbing script is usable for any entity/gameobject, so long as it has some collider set to isTrigger and has a distance joint.
    // It is ENCOURAGED to reuse this for later interesting AI behaviors and interactions.
    void Start()
    {
        if(!grabHitbox)
            grabHitbox = GetComponent<Collider2D>();
        if(!thisJoint)
            thisJoint = this.GetComponentInParent<DistanceJoint2D>();
        if(!parentObject)
            parentObject = this.gameObject.transform.parent.gameObject;

        thisJoint.enabled = false;
    }

    /****************************************************************************************************

    LOGIC DESCRIPTION:
        If inside grab hitbox, check for a rigidbody and attach that rigidbody to a DistanceJoint2D.
        If nothing is in the hitbox, decouple any currently attached rigidbody from the DistanceJoint2D.

    */
    void OnTriggerEnter2D(Collider2D collider)
     {
        if(collider.gameObject.TryGetComponent<Rigidbody2D>( out Rigidbody2D rb))
        {
                isInRange = true;
                thisJoint.connectedBody = collider.attachedRigidbody;       
        }
     }

    void OnTriggerExit2D(Collider2D collider)
    {
        thisJoint.connectedBody = null;
        isInRange = false;
    }

    /*****************************************************************************************************

    These following functions below are for non-player entities to call and use later.
    
    */ 
    public void EnableGrab()
    {
        if(isInRange)
            thisJoint.enabled = true;
    }

    public void DisableGrab()
    {
        thisJoint.enabled = false;
    }
}
