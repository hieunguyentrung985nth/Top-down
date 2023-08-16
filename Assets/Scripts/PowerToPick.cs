using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerToPick : MonoBehaviour
{
    [SerializeField] private PowerItem item;

    private void Awake()
    {
        Destroy(gameObject, 45f);
    }

    public PowerItem GetItem()
    {
        return item;
    }
}
