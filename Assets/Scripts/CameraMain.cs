using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraMain : MonoBehaviour {

    public static CameraMain Static { get; private set; }
    public static int PlayingBombParticle = 0;

    public GameObject TPlatform;
    public GameObject TPlane;
    public GameObject FloorTFunnel;
    public GameObject FloorTBallTable;
    public GameObject FloorTPlaneEnclosure;

    public SwitchPusher pusher;
    public SwitchPusher pusherLeft;
    public Pusher SwitchNoG;
    public Fan Fan;
    public Pusher pushersw;
    public Pusher pusherlsw;
    public Pusher pusherFloorSw;
    public Pusher pusherlarsw;
    public Pusher pusherClear;
    public GameObject floor;
    public GameObject floorDocker;
    public GameObject floorL;
    public GameObject floorR;

    public GameObject ballHost;
    public GameObject bulidingParts;
    public GameObject BulidingBlocks;

    public GameObject partPerfab;
    public GameObject ball8Perfab;
    public GameObject wallPerfab;
    public GameObject ballPerfab;
    public GameObject bigBallPerfab;
    public GameObject veryBigBallPerfab;
    public GameObject GrenadePerfab;
    public GameObject bombPerfab;
    public GameObject remoteBombPerfab;
    public GameObject boxPerfab;
    public GameObject oildrumPerfab;

    public ICManager iCManager;

    public Texture uiBg;

    GameObject currentBulidingObject;

    Vector3 oldMousePosition;
    Vector3 newMousePosition;

    float cameraSpeed = 4f;
    float shootForce = 1000f;

    int textShowInt = 0;
    string textShow = "";

    /// <summary>
    /// 鼠标左键是否按下
    /// </summary>
    bool mouseLeftDowned = false;
    /// <summary>
    /// 鼠标左键是否用来移动摄像机
    /// </summary>
    bool mouseLeftMoveCamera = false;


    private float setBlockMass = 10;
    private bool setBlockMassChanged = false;
    private int usePlatfrom = 3;
    private bool showMenu = true;
    private bool doNotCollectBuliding = false;
    private bool useG = true;

    Camera mainCamera;



    //按钮
    public Dropdown DropDwonBulletType;
    public Dropdown DropdownBulidingType;
    public Slider SliderBlockMass;
    public Slider SliderCameraSpeed;
    public Slider SliderShortForce;

    public Button ButtomRemoteBomb;
    public GameObject FrontSight;

    public Button ButtomUp;
    public Button ButtomDown;
    public Button ButtomLeft;
    public Button ButtomRight;
    public Button ButtomForward;
    public Button ButtomBack;
    public Button ButtomDrag;
    public Button ButtomShotMode;

    public Text TextBulletCount;
    public Text TextFloorType;
    public Text TextBlockMass;
    public Text TextShotMode;
    public Text TextCameraSpeed;
    public Text TextShortForce;
    public Text TextShowHideMenu;

    public GameObject PanelTop;
    public GameObject PanelBottom;
    public GameObject PanelQuit;
    private bool bButtomUpDown = false;
    private bool bButtomDownDown = false;
    private bool bButtomLeftDown = false;
    private bool bButtomRightDown = false;
    private bool bButtomForwardDown = false;
    private bool bButtomBackDown = false;
    private bool bButtomDragDown = false;



    private int cBulletCount = 0;
    private int cTicker = 0;

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


    //Init
    //===============================

    void Start()
    {
        Static = this;
        mainCamera = GetComponent<Camera>();
        m_rotationY = -transform.localEulerAngles.x;

        iCManager.SetObjectIC(mainCamera.gameObject);
        iCManager.SetObjectIC(floorL);
        iCManager.SetObjectIC(floorR);

        oldMousePosition = Input.mousePosition;
        newMousePosition = Input.mousePosition;
        pushersw.onSwitch += Pushersw_onSwitch;
        SwitchNoG.onSwitch += SwitchNoG_onSwitch;
        pusherFloorSw.onSwitch += PusherFloorSw_onSwitch;
        pusherlsw.onSwitch += Pusherlsw_onSwitch;
        pusherlarsw.onSwitch += Pusherlarsw_onSwitch;
        pusherClear.onSwitch += (object sender, System.EventArgs e) => { StartCoroutine(HideFloorLate()); };

        InitUIEvents();
        InitAllBulletType();
        InitAllBuildType();

        Debug.Log("Loaded");
    }
    private void InitUIEvents()
    {
        DropDwonBulletType.onValueChanged.AddListener(DropDwonBulletTypeOnValueChanged);
        DropdownBulidingType.onValueChanged.AddListener(DropDwonBulidingTypeOnValueChanged);
        SliderBlockMass.onValueChanged.AddListener(SliderBlockMassOnValueChanged);
        SliderCameraSpeed.onValueChanged.AddListener(SliderCameraSpeedOnValueChanged);
        SliderShortForce.onValueChanged.AddListener(SliderShortForceOnValueChanged);

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
        ButtomShotMode.gameObject.SetActive(false);
#endif

        EventTriggerListener.Get(FrontSight.gameObject).onClick = (GameObject go) => { ShootCenter(); };
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
    private void InitAllBulletType()
    {
        AddBulletType("小球", ballPerfab, null, 1100);
        AddBulletType("8号台球", ball8Perfab, null, 2300);
        AddBulletType("大球", bigBallPerfab, null, 3500);
        AddBulletType("大铁球", veryBigBallPerfab, null, 6000);
        AddBulletType("小炸弹", GrenadePerfab, null, 1100); 
        AddBulletType("超大炸弹", bombPerfab, null, 4000);
        AddBulletType("远程炸弹", remoteBombPerfab, null, 4500);
        AddBulletType("粘性远程炸弹", remoteBombPerfab, (GameObject prefab) => prefab.AddComponent<Viscosity>(), 4000);
        AddBulletType("箱子", boxPerfab, null, 5000);
        AddBulletType("油桶", oildrumPerfab, null, 4000);
        AddBulletType("粘性油桶", oildrumPerfab, (GameObject prefab) => prefab.AddComponent<Viscosity>(), 4000);
    }
    private void InitAllBuildType()
    {
        foreach (string s in buildPerfabNames)
            DropdownBulidingType.options.Add(new Dropdown.OptionData(s));
    }

    //===============================

    void Update()
    {
        if (cTicker < 2048) cTicker++;
        else cTicker = 0;

        //dely task
        if(cTicker % 60 == 0)
        {
            if (setBlockMassChanged) UpdateBlockMass();
        }

        int touchIndex = -1;

#if true || (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
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

        if (Input.GetKeyDown(KeyCode.Escape)) ShowQuitAsk();

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
            ShowText("Camera Speed : " + cameraSpeed);
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
            ShowText("Camera Speed : " + cameraSpeed);
        }

#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
        touchIndex = GetShootTouch();
        if (touchIndex != -1 && GUIUtility.hotControl == 0) { 
#else
        if (!EventSystem.current.IsPointerOverGameObject() && GUIUtility.hotControl == 0)
        {
#endif
            //左键按下事件
            //
            if (!mouseLeftMoveCamera)
            {
                if (Input.GetMouseButtonDown(0) && !mouseLeftDowned)
                {
                    Shoot(false, touchIndex);
                    mouseLeftDowned = true;
                }
                if (Input.GetMouseButtonUp(0) && mouseLeftDowned)
                    mouseLeftDowned = false;
            }
        }
    }

    private int GetShootTouch()
    {
        bool bPointerOverGameObject = false;
        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (!EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId))
                return i;
        }
        return -1;
    }
    private int GetMoveTouch()
    {
        bool bPointerOverGameObject = false;
        for(int i = 0; i < Input.touches.Length; i++)
        {
            bPointerOverGameObject = EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId);
            if((!bPointerOverGameObject 
                ||(bPointerOverGameObject 
                && EventSystem.current.currentSelectedGameObject == ButtomDrag.gameObject)
                && Input.touches[i].phase == TouchPhase.Began))
                return i;
        }
        return -1;
    }

    //Bullet and building
    //===============================

    private class BulletType
    {
        public delegate void InitBulletHandler(GameObject prefab);

        public GameObject prefab;
        public InitBulletHandler initCallback;
        public float force;

        public BulletType(GameObject prefab, InitBulletHandler initCallback, float force)
        {
            this.prefab = prefab;
            this.force = force;
            this.initCallback = initCallback;
        }
    }

    private List<BulletType> bulletTypes = new List<BulletType>();
    public GameObject[] buildPerfab;
    public string[] buildPerfabNames;

    private int choosedBulletType = 0;
    private int choosedBulidingType = 0;
   

    private void AddBulletType(string  name, GameObject prefab, BulletType.InitBulletHandler initCallback, float force)
    {
        DropDwonBulletType.options.Add(new Dropdown.OptionData(name));
        bulletTypes.Add(new BulletType(prefab, initCallback, force));
    }
    public void ShootCenter()
    {
        Shoot(true, -1);
    }
    public void Shoot(bool center, int touchIndex)
    {
        ShowText("Shoot !");

        //获取鼠标点击位置
        //创建射线;从摄像机发射一条经过鼠标当前位置的射线

        Vector3 mousePos = Input.mousePosition;
        if (center) mousePos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        else if (touchIndex >= 0)
        {
            Touch touch = Input.GetTouch(touchIndex);
            mousePos = new Vector3(touch.position.x, touch.position.y, 0);
        }

        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        
        //从摄像机的位置创建一个带有刚体的球ballPrefab为预制小球
        GameObject go = null;
        BulletType currentType = bulletTypes[choosedBulletType];
        go = Instantiate(currentType.prefab, transform.position, Quaternion.identity);
        go.transform.rotation = Random.rotation;

        if (currentType.initCallback != null) currentType.initCallback(go);
        go.transform.SetParent(ballHost.transform);

        //发射数来的球沿着摄像机到鼠标点击的方向进行移动
        Rigidbody ball = go.GetComponent<Rigidbody>() as Rigidbody;   
        ball.AddForce(shootForce * ray.direction);
    }

    public void ResetCamera()
    {
        iCManager.ResetObjectIC(mainCamera.gameObject);
        Debug.Log("Reset Camera");
    }
    public void RebuildBuliding()
    {
        GameObject p = buildPerfab[choosedBulidingType];
        if (p != null)
        {
            if (!doNotCollectBuliding && currentBulidingObject != null)
                Destroy(currentBulidingObject);
            currentBulidingObject = Instantiate(p, BulidingBlocks.transform);
        }
        UpdateBlockMass();
    }
    public void ClearBullet()
    {
        int objCount = 0;

        GameObject[] arr = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject g in arr)
        {
            Destroy(g);
            objCount++;
        }
        arr = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject g in arr)
        {
            Destroy(g);
            objCount++;
        }
        arr = GameObject.FindGameObjectsWithTag("BulletExplosive");
        foreach (GameObject g in arr)
        {
            Destroy(g);
            objCount++;
        }
        cBulletCount = 0;
        PlayingBombParticle = 0;

        Debug.Log("Clear" + objCount + " objects");
    }
    public void ClearAllBuilding()
    {
        int objCount = 0;

        GameObject[] arr = GameObject.FindGameObjectsWithTag("Blocks");
        foreach (GameObject g in arr)
        {
            Destroy(g);
            objCount++;
        }
        arr = GameObject.FindGameObjectsWithTag("BlocksExplosive");
        foreach (GameObject g in arr)
        {
            Destroy(g);
            objCount++;
        }
        PlayingBombParticle = 0;

        Debug.Log("Clear" + objCount + " objects");
    }
    public void RemoteBomb()
    {
        for (int i = 0; i < ballHost.transform.childCount; i++)
        {
            Bomb m1 = ballHost.transform.GetChild(i).gameObject.GetComponent<Bomb>();
            if (m1 != null) m1.Boom();
        }
    }
    public void BlockMassIncrease(float c)
    {
        setBlockMass += c;
        if (setBlockMass < 0.1f) setBlockMass = 0.1f;
        else if (setBlockMass > 30) setBlockMass = 30f;
        TextBlockMass.text = setBlockMass.ToString("0.000");
        SliderBlockMass.value = setBlockMass;
        UpdateBlockMass();
    }

    private void UpdateBlockMass()
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Blocks");
        Rigidbody r = null;
        foreach (GameObject g in arr)
        {
            r = g.GetComponent<Rigidbody>();
            if(r != null)
                r.mass = setBlockMass;
        }
        arr = GameObject.FindGameObjectsWithTag("BlocksExplosive");
        foreach (GameObject g in arr)
        {
            r = g.GetComponent<Rigidbody>();
            if (r != null)
                r.mass = setBlockMass;
        }
        setBlockMassChanged = false;
    }
    private void UpdateCameraSpeed()
    {
        SliderCameraSpeed.value = cameraSpeed;
    }
    private void UpdateShotForce()
    {
        shootForce = bulletTypes[choosedBulletType].force;
        SliderShortForce.value = shootForce;
    }

    // UI Events
    //===============================

    Rect uiBoxRect = new Rect(12, 30, 200, 63);

    private void ShowText(string text, int sec = 3)
    {
        textShow = text;
        textShowInt = sec * 60;
    }
    private void OnGUI()
    {
        GUI.Box(uiBoxRect, "");
        GUI.Label(new Rect(18, 36, 190, 46), "Camera : " + transform.position.ToString() + "\n" +
            transform.rotation.ToString() + "\n" +
             transform.eulerAngles.ToString());
        if (textShowInt > 0)
        {
            GUI.Label(new Rect(200, 15, 300, 30), textShow);
            textShowInt--;
        }
    }

    public void SwitchG(bool b)
    {
        useG = b;
        Physics.gravity = useG ? new Vector3(0, -9.8F, 0) : Vector3.zero;
    }
    public void SwitchFloorType()
    {
        if (usePlatfrom < 6) usePlatfrom++;
        else usePlatfrom = 0;

        TPlatform.SetActive(false);
        TPlane.SetActive(false);
        FloorTFunnel.SetActive(false);
        FloorTBallTable.SetActive(false);
        FloorTPlaneEnclosure.SetActive(false);

        switch (usePlatfrom)
        {
            case 0:
                TPlatform.SetActive(true);
                FloorTFunnel.SetActive(true);
                TextFloorType.text = "机关平台";
                break;
            case 1:
                TPlane.SetActive(true);
                FloorTPlaneEnclosure.SetActive(true);
                TextFloorType.text = "地面+围栏";
                break;
            case 2:
                TPlane.SetActive(true);
                TextFloorType.text = "平整地面";
                break;
            case 3:
                TPlane.SetActive(true);
                FloorTFunnel.SetActive(true);
                TextFloorType.text = "地面+漏斗";
                break;
            case 4:
                TPlatform.SetActive(true);
                TextFloorType.text = "仅平台";
                break;
            case 5:
                FloorTBallTable.SetActive(true);
                TextFloorType.text = "球桌";
                break;
            case 6:
                TextFloorType.text = "无地面";
                break;
        }
    }
    public void SwitchDoor()
    {
        PusherFloorSw_onSwitch(null, null);
    }
    public void BackToMain()
    {
        SceneManager.LoadSceneAsync(0);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void QuitBack()
    {
        PanelQuit.SetActive(false);
    }
    public void ShowQuitAsk()
    {
        PanelQuit.SetActive(true);
    }
    public void SwitchShotMode()
    {
        mouseLeftMoveCamera = !mouseLeftMoveCamera;
        TextShotMode.text = mouseLeftMoveCamera ? "鼠标左键移动摄像机" : "鼠标左键发射球";
    }
    public void SwitchShowMenuMode()
    {
        showMenu = !showMenu;
        PanelTop.SetActive(showMenu);
        PanelBottom.SetActive(showMenu);
        TextShowHideMenu.text = showMenu ? "折叠菜单 >" : "< 展开菜单";
    }

    public void DropDwonBulletTypeOnValueChanged(int x)
    {
        choosedBulletType = x;
        UpdateShotForce();
    }
    public void DropDwonBulidingTypeOnValueChanged(int x)
    {
        choosedBulidingType = x;
    }

    public void SliderBlockMassOnValueChanged(float x)
    {
        TextBlockMass.text = x.ToString("0.000");
        setBlockMass = x;
        setBlockMassChanged = true;
    }
    public void SliderCameraSpeedOnValueChanged(float x)
    {
        cameraSpeed = x;
        TextCameraSpeed.text = x.ToString("0.000");
    }
    public void SliderShortForceOnValueChanged(float x)
    {
        shootForce = x;
        bulletTypes[choosedBulletType].force = x;
        TextShortForce.text = x.ToString("0.000");
    }

    private void Pushersw_onSwitch(object sender, System.EventArgs e)
    {
        pusher.Run();
    }
    private void Pusherlsw_onSwitch(object sender, System.EventArgs e)
    {
        pusherLeft.Run();
    }
    private void Pusherlarsw_onSwitch(object sender, System.EventArgs e)
    {
        pusher.Run();
        pusherLeft.Run();
    }

    public void ToggleFrontSightOnValueChanged(bool show)
    {
        FrontSight.SetActive(show);
    }
    public void ToggleNoGOnValueChanged(bool show)
    {
        SwitchG(show);
    }
    public void ToggleNotCollectOnValueChanged(bool show)
    {
        doNotCollectBuliding = show;
    }

    bool swOn = false;

    private void SwitchNoG_onSwitch(object sender, System.EventArgs e)
    {
        Fan.run = (!Fan.run);
    }
    private void PusherFloorSw_onSwitch(object sender, System.EventArgs e)
    {
        if(swOn)
        {
            Rigidbody rigidbodyL =  floorL.GetComponent<Rigidbody>();
            Rigidbody rigidbodyR = floorR.GetComponent<Rigidbody>();

            rigidbodyL.velocity = Vector3.zero;
            rigidbodyR.velocity = Vector3.zero;

            iCManager.ResetObjectIC(floorL);
            iCManager.ResetObjectIC(floorR);

            floorDocker.SetActive(true);
            swOn = false;
        }
        else
        {
            floorDocker.SetActive(false);
            swOn = true;
        }
    }

    private IEnumerator HideFloorLate()
    {
        floor.SetActive(false);
        yield return new WaitForSeconds(2f);
        floor.SetActive(true);
    } 
}
