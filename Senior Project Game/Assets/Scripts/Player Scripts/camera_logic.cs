using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_logic : MonoBehaviour
{

    [SerializeReference] GameObject Parent;
    Transform parent;

    //[SerializeField] GameObject forcedLookAt;
    public float offSetY = 0;
    public float offSetX = 0;

    public int PlayerHugFactor = 5;

    Vector3 parentPoint;
    Vector2 halfPoint;
    // Start is called before the first frame update
    void Start()
    {
        parent = Parent.GetComponentInParent<Transform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        parentPoint = parent.position;

        halfPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition) - parentPoint;
        halfPoint = halfPoint/PlayerHugFactor;
        halfPoint += (Vector2)parentPoint;
        halfPoint.y += offSetY;
        halfPoint.x += offSetX;
        // if the Player is not forced to stop in place, look at point is close to 1/3 the distance where mouse is
    //    if(!Parent.GetComponent<Player>().mandatedStop)
            transform.position = halfPoint;


    }
}
