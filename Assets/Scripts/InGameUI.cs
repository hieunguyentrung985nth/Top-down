using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private UIManager ui;

    private void OnDisable()
    {
        ui.ClearQueue();
    }
}
