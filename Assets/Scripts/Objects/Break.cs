using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour
{
    public bool run = false;
    public float force = 10;

    public GameObject left;
    public GameObject right;

    private Rigidbody rigidbodyL;
    private Rigidbody rigidbodyR;

    void Start()
    {
        rigidbodyL = left.GetComponent<Rigidbody>();
        rigidbodyR = right.GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(run)
        {
            rigidbodyL.AddForceAtPosition(Vector3.forward * force, Vector3.forward, ForceMode.Force);
            rigidbodyR.AddForceAtPosition(Vector3.back * force, Vector3.back, ForceMode.Force);
        }
        else
        {
            rigidbodyL.AddForceAtPosition(Vector3.back * force, Vector3.back, ForceMode.Force);
            rigidbodyR.AddForceAtPosition(Vector3.forward * force, Vector3.forward, ForceMode.Force);
        }
    }
}
