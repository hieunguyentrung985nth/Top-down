using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthToPick : MonoBehaviour
{
    [SerializeField] private HealthItem item;

    private void Awake()
    {
        Destroy(gameObject, 45f);
    }

    public HealthItem GetItem()
    {
        return item;
    }
}
