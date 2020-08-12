using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour {

    int restSec = 0;
    float oldY = 0;
    float toY = 0;

    // Use this for initialization
    void Start () {
        oldY = transform.localPosition.y;
        toY = transform.localPosition.y - 0.3f;
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(restSec > 0)
        {
            if(restSec == 1)
                transform.localPosition = new Vector3(transform.localPosition.x, oldY, transform.localPosition.z);
            restSec--;
        }
	}

    public event System.EventHandler onSwitch;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
            transform.localPosition = new Vector3(transform.localPosition.x, toY, transform.localPosition.z);
            restSec = 120;
            SwitchOn();
        }
    }

    private void SwitchOn()
    {
        if (onSwitch != null) onSwitch.Invoke(null, null);
    }
}
