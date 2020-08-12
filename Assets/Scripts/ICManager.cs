using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICManager : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        
    }

    private class ICStorage
    {
        public GameObject go;
        public Vector3 position;
        public Vector3 rotation;
    }

    private List<ICStorage> storage = new List<ICStorage>();

    private ICStorage FindObjectIC(GameObject go)
    {
        foreach (ICStorage s in storage)
            if (s.go == go) return s;
        return null;
    }
    public void SetObjectIC(GameObject go)
    {
        ICStorage s = FindObjectIC(go);
        if (s == null)
        {
            s = new ICStorage();
            s.go = go;
            storage.Add(s);
        }

        s.position = go.transform.position;
        s.rotation = go.transform.eulerAngles;

    }
    public void ResetObjectIC(GameObject go)
    {
        ICStorage s = FindObjectIC(go);
        if (s != null)
        {
            go.transform.position = s.position;
            go.transform.eulerAngles = s.rotation;
        }
    }
}
