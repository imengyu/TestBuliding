using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FanControll : MonoBehaviour
{

    public Fan Fan;
    public Break Break;

    public Fan FanSmall;
    public Break Break2;

    public Text TextFanForce;
    public Slider SliderFanForce;
    public Toggle ToggleFan;
    public Text TextBreakForce;
    public Slider SliderBreakForce;
    public Toggle ToggleBreak;

    public Text TextFanForce2;
    public Slider SliderFanForce2;
    public Toggle ToggleFan2;
    public Text TextBreakForce2;
    public Slider SliderBreakForce2;
    public Toggle ToggleBreak2;

    void Start()
    {
        InitUIEvents();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadSceneAsync(0);
    }

    void InitUIEvents()
    {
        SliderFanForce.onValueChanged.AddListener(SliderFanForceOnValueChanged);
        SliderBreakForce.onValueChanged.AddListener(SliderBreakForceOnValueChanged);
        ToggleFan.onValueChanged.AddListener((on) => { Fan.run = on; });
        ToggleBreak.onValueChanged.AddListener((on) => { Break.run = on; });

        SliderFanForce2.onValueChanged.AddListener(SliderFanForce2OnValueChanged);
        SliderBreakForce2.onValueChanged.AddListener(SliderBreakForce2OnValueChanged);
        ToggleFan2.onValueChanged.AddListener((on) => { FanSmall.run = on; });
        ToggleBreak2.onValueChanged.AddListener((on) => { Break2.run = on; });
    }

    public void SliderBreakForceOnValueChanged(float x)
    {
        Break.force = x;
        TextBreakForce.text = x.ToString("0.000");
    }
    public void SliderFanForceOnValueChanged(float x)
    {
        Fan.force = x;
        TextFanForce.text = x.ToString("0.000");
    }
    public void SliderBreakForce2OnValueChanged(float x)
    {
        Break2.force = x;
        TextBreakForce2.text = x.ToString("0.000");
    }
    public void SliderFanForce2OnValueChanged(float x)
    {
        FanSmall.force = x;
        TextFanForce2.text = x.ToString("0.000");
    }

}
