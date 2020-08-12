using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IntroMain : MonoBehaviour
{
    public GameObject uiPanel;
    public GameObject uiPanelLoading;
    public GameObject uiPanelQuit;
    public Light lightP;

    public Button ButtonStartDw;
    public Button ButtonStartFan;

    private bool downLight = false;

    void Start()
    {
        
    }
    void Update()
    {
        if(downLight)
        {
            if (lightP.intensity > 0)
                lightP.intensity -= 0.001f;
            else downLight = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitShow();
        
    }

    public void QuitBack()
    {
        uiPanelQuit.SetActive(false);
    }
    public void QuitShow()
    {
        uiPanelQuit.SetActive(true);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void StartBuilding()
    {
        uiPanel.SetActive(false);
        uiPanelLoading.SetActive(true);
        downLight = true;
        SceneManager.LoadSceneAsync(1);
    }
    public void StartFan()
    {
        uiPanel.SetActive(false);
        uiPanelLoading.SetActive(true);
        downLight = true;
        SceneManager.LoadSceneAsync(2);
    }
} 
