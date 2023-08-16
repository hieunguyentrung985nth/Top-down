using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    private BattleSystem battleSystem;

    [SerializeField] private bool isLightOn = false;

    private void Awake()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isLightOn)
            {
                LightSystem.Instance.IncreaseLight(0.3f);

                battleSystem.GetWaves().gameObject.SetActive(true);

                transform.parent.gameObject.SetActive(false);
            }

            else
            {
                LightSystem.Instance.DecreaseLight(0.3f);

                CameraShake.Instance.ShakeCamera(7f, 0.5f);

                battleSystem.GetWaves().gameObject.SetActive(true);

                transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
