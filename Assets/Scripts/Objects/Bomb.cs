using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Explosive
{
    public FixedJoint fixedJoint = null;
    public bool isRemote = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isRemote)
            Boom();
    }
}
