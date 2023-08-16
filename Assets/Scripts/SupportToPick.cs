using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportToPick : MonoBehaviour
{
    [SerializeField] private SupportItem item;

    [SerializeField] private float duration;

    private void Awake()
    {
        Destroy(gameObject, 45f);
    }

    public float GetShieldDuration()
    {
        return duration;
    }
}
