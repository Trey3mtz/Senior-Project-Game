using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grab_script : MonoBehaviour
{

    [SerializeField] Collider2D grabHitbox;
    [SerializeField] DistanceJoint2D thisJoint;
    [SerializeField] GameObject parentObject;

    public bool inRange = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!grabHitbox)
            grabHitbox = GetComponent<Collider2D>();
        if(!thisJoint)
            thisJoint = this.GetComponentInParent<DistanceJoint2D>();
        if(!parentObject)
            parentObject = this.gameObject.transform.parent.gameObject;
    }


    // If inside grab range, check for a rigidbody and attach it to the joint
    void OnTriggerEnter2D(Collider2D collider)
     {
        if(collider.gameObject.TryGetComponent<Rigidbody2D>( out Rigidbody2D rb))
        {
                inRange = true;
                thisJoint.connectedBody = collider.attachedRigidbody;       
        }
     }

    // If nothing is in grab range, disconnect any attached rigidbodies
    void OnTriggerExit2D(Collider2D collider)
    {
        thisJoint.connectedBody = null;
        inRange = false;
    }
}
