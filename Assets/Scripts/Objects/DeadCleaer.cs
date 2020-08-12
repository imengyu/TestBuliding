using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadCleaer : MonoBehaviour {

	void Start () {
		
	}
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        ShowText("Clear falling Object : " + collision.gameObject.name + " ID : " + collision.gameObject.GetInstanceID());
        Destroy(collision.gameObject);
    }

    int textShowInt = 0;
    string textShow = "";

    private void ShowText(string text, int sec = 3)
    {
        textShow = text;
        textShowInt = sec * 60;
    }
    private void OnGUI()
    {
        if (textShowInt > 0)
        {
            GUI.Label(new Rect(320, 20, 300, 30), textShow);
            textShowInt--;
        }
    }
}
