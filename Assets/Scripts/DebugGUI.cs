using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;


public class DebugGUI : MonoBehaviour
{
    private bool showConsole = false;
    private bool showStats = false;
    private Rect consoleWindowRect = new Rect(20, 32, 550, 400);
    private Rect statsWindowRect = new Rect(220, 32, 350, 280);
    private string consoleCmd = "";
    private FPSManager fPSManager = null;
    private bool betterMemorySize = true;
    private bool onlyError = true;
    private bool onlyWarning = true;
    private bool onlyDebug = true;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition2;
    private Vector2 scrollPosition3;

    readonly Rect titleBarRect2 = new Rect(0, 0, 10000, 20);
    readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
    readonly Rect titleBarRect3 = new Rect(0, 0, 10000, 20);
    readonly Rect titleBarRect4 = new Rect(0, 0, 10000, 20);

    public Texture2D redTexture;
    public Texture2D organgeTexture;
    public Font consoleFont;
    public int consoleFontSize;

    void Start()
    {
        fPSManager = gameObject.AddComponent<FPSManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(ShortcutKey)) showConsole = !showConsole;
        if (ShakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration) showConsole = true;
    }
    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 80, 23), fPSManager.fps.ToString("FPS: 0.0"));
        showConsole = GUI.Toggle(new Rect(80, 10, 65, 23), showConsole, "控制台");
        if (showConsole)
            consoleWindowRect = GUI.Window(0, consoleWindowRect, ConsoleWindowFun, "Debug Console");
        if (showStats)
            statsWindowRect = GUI.Window(3, statsWindowRect, StatsWindowFun, "Statistics");
    }

    //窗口绘制

    void ConsoleWindowFun(int windowid)
    {
        DrawTopToolbar();
        DrawLogsList();
        DrawToolbar();
        GUI.DragWindow(titleBarRect);
    }
    void StatsWindowFun(int windowid)
    {
        scrollPosition3 = GUILayout.BeginScrollView(scrollPosition2);

        GUIStyle styleLeft = new GUIStyle();
        styleLeft.alignment = TextAnchor.MiddleRight;
        styleLeft.normal.textColor = new Color(0.733f, 0.733f, 0.733f);

        GUILayout.Space(15);

        GUILayout.BeginHorizontal(); GUILayout.Label("FPS: ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(fPSManager.fps.ToString("0.00"), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("ProfilerEnabled : ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(Profiler.enabled.ToString(), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("ProfilerSupported : ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(Profiler.supported.ToString(), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("MaxUsedMemory: ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(byteToReadableSize(Profiler.maxUsedMemory), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("UsedHeapSize: ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(byteToReadableSize(Profiler.usedHeapSizeLong), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("MonoUsedSize: ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(byteToReadableSize(Profiler.GetMonoUsedSizeLong()), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("MonoHeapSize: ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(byteToReadableSize(Profiler.GetMonoHeapSizeLong()), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("AllocatedMemoryForGraphicsDriver: ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(byteToReadableSize(Profiler.GetAllocatedMemoryForGraphicsDriver()), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("TotalAllocatedMemory: ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(byteToReadableSize(Profiler.GetTotalAllocatedMemoryLong()), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("TotalReservedMemory: ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(byteToReadableSize(Profiler.GetTotalReservedMemoryLong()), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal(); GUILayout.Label("TotalUnusedReservedMemory: ", styleLeft, GUILayout.MaxWidth(216f)); GUILayout.Label(byteToReadableSize(Profiler.GetTotalUnusedReservedMemoryLong()), GUILayout.MaxWidth(100f)); GUILayout.EndHorizontal();

        GUILayout.Space(15);
        betterMemorySize = GUILayout.Toggle(betterMemorySize, "BetterMemorySize");
        GUILayout.Space(5);

        GUILayout.EndScrollView();
        GUI.DragWindow(titleBarRect3);
    }

    private string byteToReadableSize(long bytes)
    {
        string result = "";
        double resultd = 0;
        if (betterMemorySize)
        {
            if (bytes >= 1073741824)
            {
                resultd = Math.Round(bytes / 1073741824d * 100d) / 100d;
                result = resultd.ToString("0.00") + "GB";
            }
            else if (bytes >= 1048576)
            {
                resultd = Math.Round(bytes / 1048576d * 100d) / 100d;
                result = resultd.ToString("0.00") + "MB";
            }
            else
            {
                resultd = Math.Round(bytes / 1024d * 100d) / 100d;
                result = resultd.ToString("0.00") + "KB";
            }
        }
        else result = bytes.ToString();
        return result;
    }

    #region Inspector 面板属性

    [Tooltip("快捷键-开/关控制台")] public KeyCode ShortcutKey = KeyCode.Q;
    [Tooltip("摇动开启控制台？")] public bool ShakeToOpen = true;
    [Tooltip("窗口打开加速度")] public float shakeAcceleration = 3f;
    [Tooltip("是否保持一定数量的日志")] public bool restrictLogCount = false;
    [Tooltip("最大日志数")] public int maxLogs = 1000;

    #endregion

    #region Log

    private class Log
    {
        public string Message;
        public string Time;
        public string Level;
        public string StackTrace;
        public LogType LogType;
        public bool ShowTrace;
    }

    #region LogSets

    private List<Log> logs = new List<Log>();
    private Log log;
    public bool collapse;

    static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
        {
            {LogType.Assert, new Color(1, 0.3f, 00)},
            {LogType.Error, new Color(1, 0.250f, 0.250f)},
            {LogType.Exception, new Color(1, 0.1f, 0.1f)},
            {LogType.Log, Color.white},
            {LogType.Warning, new Color(1, 0.756f, 0.145f)},
        };
    static readonly Dictionary<LogType, string> logTypeStr = new Dictionary<LogType, string>
        {
            {LogType.Assert, "A"},
            {LogType.Error, "E"},
            {LogType.Exception, "F"},
            {LogType.Log, "I"},
            {LogType.Warning, "W"},
        };
    private const int Edge = 20;
    readonly GUIContent clearLabel = new GUIContent("清空", "清空控制台内容");
    readonly GUIContent runLabel = new GUIContent("执行", "运行命令");

    void OnEnable()
    {
#if UNITY_4
            Application.RegisterLogCallback(HandleLog);
#else
        Application.logMessageReceived += HandleLog;
#endif
    }
    void OnDisable()
    {
#if UNITY_4
            Application.RegisterLogCallback(null);
#else
        Application.logMessageReceived -= HandleLog;
#endif
    }

    void RunCmd()
    {
        if (consoleCmd == "")
            Debug.LogError("请输入命令！");
        else
        {
            consoleCmd = "";
        }
    }

    #endregion

    void DrawTopToolbar()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Statistics", GUILayout.MaxWidth(60)))
            showStats = !showStats;

#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
        {
            if (GUILayout.Button("停止", GUILayout.MaxWidth(50)))
                UnityEditor.EditorApplication.isPlaying = false;
        }
        if (!UnityEditor.EditorApplication.isPaused)
        {
            if (GUILayout.Button("暂停", GUILayout.MaxWidth(50)))
                UnityEditor.EditorApplication.isPaused = true;
        }
#else
            if (GUILayout.Button("强制退出游戏", GUILayout.MaxWidth(120)))
                Application.Quit();
#endif


        GUILayout.EndHorizontal();
    }
    void DrawLogsList()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        RectOffset paddingLableOld = GUI.skin.label.padding;
        Font fontLableOld = GUI.skin.label.font;
        int fontSizeLableOld = GUI.skin.label.fontSize;

        GUI.skin.label.padding = new RectOffset(0, 0, 0, 0);
        GUI.skin.label.font = consoleFont;
        GUI.skin.label.fontSize = consoleFontSize;
        for (var i = 0; i < logs.Count; i++)
        {
            GUILayout.BeginHorizontal();

            GUI.contentColor = Color.white;
            GUILayout.Label(logs[i].Level, GUILayout.Width(30f));
            GUILayout.Label(logs[i].Time, GUILayout.Width(60f));
            GUI.contentColor = logTypeColors[logs[i].LogType];
            GUILayout.Label(logs[i].Message);

            logs[i].ShowTrace = GUILayout.Toggle(logs[i].ShowTrace, "", GUILayout.MaxWidth(25), GUILayout.MaxHeight(16));

            GUILayout.EndHorizontal();

            if (logs[i].ShowTrace)
            {
                GUILayout.Label("[" + logs[i].LogType + "] Stack Trace");
                GUI.contentColor = Color.white;
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label(logs[i].StackTrace);
                GUILayout.EndVertical();
                GUILayout.Space(10);
            }
        }
        GUILayout.EndScrollView();
        GUI.contentColor = Color.white;
        GUI.skin.label.padding = paddingLableOld;
        GUI.skin.label.font = fontLableOld;
        GUI.skin.label.fontSize = fontSizeLableOld;
    }
    void DrawToolbar()
    {
        GUILayout.BeginHorizontal();

        GUI.SetNextControlName("CommandInput");
        consoleCmd = GUILayout.TextField(consoleCmd);

        if (GUI.GetNameOfFocusedControl() == "CommandInput")
        {
            if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.Used)
                RunCmd();
        }

        if (GUILayout.Button(runLabel, GUILayout.MaxWidth(50)))
            RunCmd();
        if (GUILayout.Button(clearLabel, GUILayout.MaxWidth(50)))
            logs.Clear();

        onlyDebug = GUILayout.Toggle(onlyDebug, "调试", GUILayout.ExpandWidth(true), GUILayout.Width(45));
        onlyWarning = GUILayout.Toggle(onlyWarning, "警告", GUILayout.ExpandWidth(true), GUILayout.Width(45));
        onlyError = GUILayout.Toggle(onlyError, "错误和异常", GUILayout.ExpandWidth(true), GUILayout.Width(90));


        GUILayout.EndHorizontal();
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        logs.Add(new Log
        {
            Time = DateTime.Now.ToString("HH:MM:ss"),
            Level = "[" + logTypeStr[type] + "] ",
            Message = message,
            StackTrace = stackTrace,
            LogType = type,
        });
        DeleteExcessLogs();
    }
    void DeleteExcessLogs()
    {
        if (!restrictLogCount) return;
        var amountToRemove = Mathf.Max(logs.Count - maxLogs, 0);
        print(amountToRemove);
        if (amountToRemove == 0)
        {
            return;
        }

        logs.RemoveRange(0, amountToRemove);
    }

    #endregion


}
