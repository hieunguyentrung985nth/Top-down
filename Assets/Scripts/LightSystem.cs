using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSystem : MonoBehaviour
{
    public static LightSystem Instance { get; private set; }

    private Light2D globalLight;

    private float timer;

    private float totalTimer;

    private bool isLightOff = false;

    private float startingIntensity;

    [SerializeField] private float minIntensity;

    [SerializeField] private float maxIntensity;

    private void Awake()
    {
        Instance = this;

        globalLight = GetComponent<Light2D>();
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;

            if (isLightOff)
            {
                globalLight.intensity = Mathf.Lerp(startingIntensity, maxIntensity, timer / totalTimer);
            }
            else
            {
                globalLight.intensity = Mathf.Lerp(startingIntensity, minIntensity, timer / totalTimer);
            }
        }
    }

    public void SetLight(float intensity, Color color)
    {
        globalLight.intensity = intensity;

        globalLight.color = color;
    }

    public void DecreaseLight(float time)
    {
        isLightOff = true;

        timer = time;

        totalTimer = time;

        startingIntensity = minIntensity;
    }

    public void IncreaseLight(float time)
    {
        isLightOff = false;

        timer = time;

        totalTimer = time;

        startingIntensity = maxIntensity;
    }
}
