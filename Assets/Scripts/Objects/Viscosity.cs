using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viscosity : MonoBehaviour
{
    public float ViscosityMass = 0.2f;

    private List<int> stuckObjects = new List<int>();

    private void OnCollisionEnter(Collision collision)
    {
        if (!stuckObjects.Contains(collision.gameObject.GetInstanceID()))
        {
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();

            fixedJoint.connectedBody = collision.rigidbody;
            fixedJoint.breakForce = 256f;
            fixedJoint.breakTorque = 64f;

            rigidbody.velocity = Vector3.zero;
            rigidbody.mass = ViscosityMass;

            stuckObjects.Add(collision.gameObject.GetInstanceID());
        }
    }
}
