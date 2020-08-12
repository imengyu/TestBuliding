using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPusher : MonoBehaviour {

    public bool runX = false;
    public bool runY = false;
    public bool runZ = false;
    public float toX = 0;
    public float toY = 0;
    public float toZ = 0;
    public float formX = 0;
    public float formY = 0;
    public float formZ = 0;
    public float speed = 0.5f;

    float otoX = 0;
    float otoY = 0;
    float otoZ = 0;

    float dX, dY, dZ;

    bool reverse = false;
    bool running = false;
    bool runningX = false;
    bool runningY = false;
    bool runningZ = false;

    // Use this for initialization
    void Start () {
        formX = transform.localPosition.x;
        formY = transform.localPosition.x;
        formZ = transform.localPosition.x;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (running)
        {
            if (runningX)
            {
                if (dX > 0)
                {
                    if(transform.localPosition.x < toX)
                        transform.localPosition = new Vector3(transform.localPosition.x + speed * Time.deltaTime, transform.localPosition.y, transform.localPosition.z);
                    else runningX = false;
                }
                else if (dX < 0)
                {
                    if (transform.localPosition.x > toX)
                        transform.localPosition = new Vector3(transform.localPosition.x - speed * Time.deltaTime, transform.localPosition.y, transform.localPosition.z);
                    else runningX = false;
                }
                else runningX = false;
            }
            if (runningY)
            {
                if (dY > 0)
                {
                    if (transform.localPosition.y < toY)
                        transform.localPosition = new Vector3(transform.localPosition.x , transform.localPosition.y + speed * Time.deltaTime, transform.localPosition.z);
                    else runningY = false;
                }
                else if (dY < 0)
                {
                    if (transform.localPosition.y > toY)
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - speed * Time.deltaTime, transform.localPosition.z);
                    else runningY = false;
                }
                else runningY = false;
            }
            if (runningZ)
            {
                if (dZ > 0)
                {
                    if (transform.localPosition.z < toZ)
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + speed * Time.deltaTime);
                    else runningZ = false;
                }
                else if (dZ < 0)
                {
                    if (transform.localPosition.z > toZ)
                        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - speed * Time.deltaTime);
                    else runningZ = false;
                }
                else runningZ = false;
            }

            if (!runningX && !runningX && !runningZ)
            {
                if (reverse)
                {
                    dX = formX - toX;
                    dY = formY - toY;
                    dZ =  formZ - toZ;

                    otoX = toX;
                    otoY = toY;
                    otoZ = toZ;

                    toX = formX;
                    toY = formY;
                    toZ = formZ;

                    if (runX) runningX = true;
                    if (runY) runningY = true;
                    if (runZ) runningZ = true;

                    reverse = false;
                }
                else
                {
                    toX = otoX;
                    toY = otoY;
                    toZ = otoZ;

                    running = false;
                }
            }
        }
	}

    public void Run()
    {
        dX = toX - formX;
        dY = toY - formY;
        dZ = toZ - formZ;

        running = true;
        reverse = true;

        RunR();
    }

    void RunR()
    {
        if (runX) runningX = true;
        if (runY) runningY = true;
        if (runZ) runningZ = true;
    }

}
