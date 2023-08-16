using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Slider sliderBGMPauseMenu;

    [SerializeField] private Slider sliderSFXPauseMenu;

    private SoundManager soundManager;

    private void Awake()
    {
        soundManager = FindObjectOfType<SoundManager>();

        sliderBGMPauseMenu.onValueChanged.AddListener(delegate { soundManager.OnChangeBGMVolume(sliderBGMPauseMenu.value); });

        sliderSFXPauseMenu.onValueChanged.AddListener(delegate { soundManager.OnChangeSFXVolume(sliderSFXPauseMenu.value); });
    }

    private void OnEnable()
    {
        sliderBGMPauseMenu.value = soundManager.GetBGMVolume();

        sliderSFXPauseMenu.value = soundManager.GetSFXVolume();
    }
}
