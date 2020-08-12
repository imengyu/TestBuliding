using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    public bool isSim = false;
    public bool run = false;
    public float force = 30;

    private new Rigidbody rigidbody;
    private float degree = 0;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(run)
        {
            if (isSim)
            {
                transform.Rotate(Vector3.left, 1 * force * Time.deltaTime);
            }
            else
            {
                degree = (transform.eulerAngles.x % 360);
                rigidbody.AddForceAtPosition(
                    new Vector3(0, ((degree > 90 && degree < 180) || (degree > 270 && degree < 360) ? 1 : -1) * Mathf.Sin(Mathf.Deg2Rad * degree),
                        ((degree > 0 && degree < 90) || (degree > 180 && degree < 270) ? 1 : -1) * Mathf.Cos(Mathf.Deg2Rad * degree)) * force,
                    new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * degree) * 5,
                        Mathf.Cos(Mathf.Deg2Rad * degree) * 5));
            }
        }
    }
}
