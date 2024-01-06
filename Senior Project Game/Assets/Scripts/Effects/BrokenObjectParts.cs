using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class BrokenObjectParts : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform parentObject;
    private float x,y;

    [SerializeField] private float minForce,maxForce;

    void Start()
    {
        parentObject = transform;
        BreakApart();
    }
    
    private void BreakApart()
    {
        foreach(Transform part in parentObject)
        {
            x = Random.Range(minForce,maxForce);
            y = Random.Range(minForce,maxForce);
            //part.SetParent(transform.root);
            part.GetComponent<Rigidbody2D>().AddForce(new Vector2(x,y));
        }

        StartCoroutine(destroyLater());
    }

    private IEnumerator destroyLater()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
