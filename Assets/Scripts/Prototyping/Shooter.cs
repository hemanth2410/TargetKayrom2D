using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Shooter : MonoBehaviour
{
    public Vector2 ForceVector;
    public bool RandomDirection;
    public float Magnitude;
    Rigidbody2D rig;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { AddForce(); }
    }

    public void AddForce()
    {
        if (!RandomDirection)
        {
            rig.AddForce(ForceVector, ForceMode2D.Impulse);
        }
        else
        {
            var forceVector = Random.insideUnitCircle;

            rig.AddForce(forceVector * Magnitude, ForceMode2D.Impulse);
        }
    }
}
