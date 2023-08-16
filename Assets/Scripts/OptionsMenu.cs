using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private Slider sliderBGMOptionMenu;

    [SerializeField] private Slider sliderSFXOptionMenu;

    private SoundManager soundManager;

    private void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();

        sliderBGMOptionMenu.onValueChanged.AddListener(delegate { soundManager.OnChangeBGMVolume(sliderBGMOptionMenu.value); });

        sliderSFXOptionMenu.onValueChanged.AddListener(delegate { soundManager.OnChangeSFXVolume(sliderSFXOptionMenu.value); });
    }

    private void OnEnable()
    {
        sliderBGMOptionMenu.value = soundManager.GetBGMVolume();

        sliderSFXOptionMenu.value = soundManager.GetSFXVolume();
    }
}
