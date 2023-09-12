using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoundaryCollisionHandler : MonoBehaviour
{

    public float BoardRadius = 8.5f;
    public float BounceMultiplier = 0.8f;

    float currentDistFromCenter;
    Rigidbody2D rig;
    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Calculate distance from center of the parent
        currentDistFromCenter = transform.localPosition.magnitude;

        if (currentDistFromCenter >= BoardRadius)
        {
            var normal = -transform.position.normalized;
            rig.velocity = Vector2.Reflect(rig.velocity, normal) * BounceMultiplier;
        }

    }
}
