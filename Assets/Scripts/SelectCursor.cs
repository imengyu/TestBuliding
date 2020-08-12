using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCursor : MonoBehaviour
{
    private Material material;

    public Color dragColor = Color.red;
    private Color normalColor = Color.red;
    private Camera cam;
    private GameObject go; //射线碰撞的物体

    private Vector3 screenSpace;
    private Vector3 offset;
    private bool isDrage = false;

    public static string btnName; //射线碰撞物体的名字

    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        normalColor = material.color;
        cam = Camera.main;
    }
    void Update()
    {
        //整体初始位置
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //从摄像机发出到点击坐标的射线
        RaycastHit hitInfo;
        if (isDrage == false)
        {
            if (Physics.Raycast(ray, out hitInfo))
            {
                //划出射线 只有在Scene视图中才能看到
                Debug.DrawLine(ray.origin, hitInfo.point);
                go = hitInfo.collider.gameObject;

                screenSpace = cam.WorldToScreenPoint(go.transform.position);
                offset = go.transform.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));

                btnName = go.name;
            }
            else
            {
                btnName = null;
            }
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
            Vector3 currentPosition = cam.ScreenToWorldPoint(currentScreenSpace) + offset;
            if (btnName != null)
            {
                go.transform.position = currentPosition;
            }
            isDrage = true;
        }
        else
        {
            isDrage = false;
        }
    }
   
}
