using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactScript : MonoBehaviour
{
    [SerializeField] float lifespan = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, lifespan);
    }

}
