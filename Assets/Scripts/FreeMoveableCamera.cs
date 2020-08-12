using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FreeMoveableCamera : MonoBehaviour
{

    float cameraSpeed = 4f;

    public Button ButtomUp;
    public Button ButtomDown;
    public Button ButtomLeft;
    public Button ButtomRight;
    public Button ButtomForward;
    public Button ButtomBack;
    public Button ButtomDrag;

    public Text TextCameraSpeed;
    public Slider SliderCameraSpeed;

    public bool mouseLeftMoveCamera = true;

    void Start()
    {
        InitUIEvents();
    }

    private bool bButtomUpDown = false;
    private bool bButtomDownDown = false;
    private bool bButtomLeftDown = false;
    private bool bButtomRightDown = false;
    private bool bButtomForwardDown = false;
    private bool bButtomBackDown = false;
    private bool bButtomDragDown = false;

    //Roate camera

    public float m_sensitivityX = 10f;
    public float m_sensitivityY = 10f;
    // 水平方向的 镜头转向
    public float m_minimumX = -360f;
    public float m_maximumX = 360f;
    // 垂直方向的 镜头转向 (这里给个限度 最大仰角为45°)
    public float m_minimumY = -45f;
    public float m_maximumY = 45f;

    private float m_rotationY = 0f;
    private int lastDownTouchId = 0;

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GUIUtility.hotControl == 0)
        {
#if false || (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
        if (Input.touchCount > 0 && lastDownTouchId >= 0 && bButtomDragDown)
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].fingerId == lastDownTouchId && Input.touches[i].phase == TouchPhase.Moved)
                {
                    float m_rotationX = transform.localEulerAngles.y + Input.touches[i].deltaPosition.x * Time.deltaTime * m_sensitivityX;
                    m_rotationY += Input.touches[i].deltaPosition.y * Time.deltaTime * m_sensitivityY;
                    m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);
                    transform.localEulerAngles = new Vector3(-m_rotationY, m_rotationX, 0);
                    break;
                }
            }
        }
        else
        {
            lastDownTouchId = 0;
        }
#else
            if (bButtomDragDown || (!mouseLeftMoveCamera && Input.GetMouseButton(1))
           || (mouseLeftMoveCamera && Input.GetMouseButton(0)))
            {
                float m_rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * m_sensitivityX;
                m_rotationY += Input.GetAxis("Mouse Y") * m_sensitivityY;
                m_rotationY = Mathf.Clamp(m_rotationY, m_minimumY, m_maximumY);

                transform.localEulerAngles = new Vector3(-m_rotationY, m_rotationX, 0);
            }
#endif
        }

        //空格键抬升高度
        if (Input.GetKey(KeyCode.Q) || bButtomUpDown)
        {
            transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime, Space.Self);
        }
        if (Input.GetKey(KeyCode.E) || bButtomDownDown)
        {
            transform.Translate(Vector3.down * cameraSpeed * Time.deltaTime, Space.Self);
        }
        //w键
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || bButtomForwardDown)
        {
            this.gameObject.transform.Translate(new Vector3(0, 0, 2 * cameraSpeed * Time.deltaTime));
        }
        //s键
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || bButtomBackDown)
        {
            this.gameObject.transform.Translate(new Vector3(0, 0, 2 * -cameraSpeed * Time.deltaTime));
        }
        //a键
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || bButtomLeftDown)
        {
            this.gameObject.transform.Translate(new Vector3(-(cameraSpeed) * Time.deltaTime, 0, 0));
        }
        //d键
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || bButtomRightDown)
        {
            this.gameObject.transform.Translate(new Vector3((cameraSpeed) * Time.deltaTime, 0, 0));
        }

        //Zoom out
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cameraSpeed < 1)
                cameraSpeed -= 0.01f;
            else if (cameraSpeed < 10)
                cameraSpeed -= 0.1f;
            else if (cameraSpeed < 50)
                cameraSpeed -= 1f;
            else if (cameraSpeed < 100)
                cameraSpeed -= 5f;
            else cameraSpeed -= 10f;

            UpdateCameraSpeed();
        }
        //Zoom in
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (cameraSpeed < 1)
                cameraSpeed += 0.01f;
            else if (cameraSpeed < 10)
                cameraSpeed += 0.1f;
            else if (cameraSpeed < 50)
                cameraSpeed += 1f;
            else if (cameraSpeed < 100)
                cameraSpeed += 5f;
            else cameraSpeed += 10f;

            UpdateCameraSpeed();
        }

    }

    private void InitUIEvents()
    {
        SliderCameraSpeed.onValueChanged.AddListener(SliderCameraSpeedOnValueChanged);

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
#endif

        EventTriggerListener.Get(ButtomUp.gameObject).onDown = (GameObject go) => { bButtomUpDown = true; };
        EventTriggerListener.Get(ButtomDown.gameObject).onDown = (GameObject go) => { bButtomDownDown = true; };
        EventTriggerListener.Get(ButtomLeft.gameObject).onDown = (GameObject go) => { bButtomLeftDown = true; };
        EventTriggerListener.Get(ButtomRight.gameObject).onDown = (GameObject go) => { bButtomRightDown = true; };
        EventTriggerListener.Get(ButtomForward.gameObject).onDown = (GameObject go) => { bButtomForwardDown = true; };
        EventTriggerListener.Get(ButtomBack.gameObject).onDown = (GameObject go) => { bButtomBackDown = true; };
        EventTriggerListener.Get(ButtomDrag.gameObject).onDown = (GameObject go) => {
            bButtomDragDown = true;
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (EventSystem.current.currentSelectedGameObject.GetInstanceID() == ButtomDrag.gameObject.GetInstanceID()
                    && Input.touches[i].phase == TouchPhase.Began)
                {
                    lastDownTouchId = Input.touches[i].fingerId;
                    return;
                }
            }
            lastDownTouchId = -1;
#endif
        };

        EventTriggerListener.Get(ButtomUp.gameObject).onUp = (GameObject go) => { bButtomUpDown = false; };
        EventTriggerListener.Get(ButtomDown.gameObject).onUp = (GameObject go) => { bButtomDownDown = false; };
        EventTriggerListener.Get(ButtomLeft.gameObject).onUp = (GameObject go) => { bButtomLeftDown = false; };
        EventTriggerListener.Get(ButtomRight.gameObject).onUp = (GameObject go) => { bButtomRightDown = false; };
        EventTriggerListener.Get(ButtomForward.gameObject).onUp = (GameObject go) => { bButtomForwardDown = false; };
        EventTriggerListener.Get(ButtomBack.gameObject).onUp = (GameObject go) => { bButtomBackDown = false; };
        EventTriggerListener.Get(ButtomDrag.gameObject).onUp = (GameObject go) => {
            lastDownTouchId = -1;
            bButtomDragDown = false;
        };

    }

    public void SliderCameraSpeedOnValueChanged(float x)
    {
        cameraSpeed = x;
        TextCameraSpeed.text = x.ToString("0.000");
    }

    private void UpdateCameraSpeed()
    {
        SliderCameraSpeed.value = cameraSpeed;
    }
}
