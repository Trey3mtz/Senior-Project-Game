using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMoveX : MonoBehaviour
{
    [SerializeField]
    public float ForceAlongX;
    // Start is called before the first frame update
    private void Start()
    {
        this.GetComponent<Rigidbody2D>().AddForce(new Vector2(ForceAlongX,0));
    }

}
