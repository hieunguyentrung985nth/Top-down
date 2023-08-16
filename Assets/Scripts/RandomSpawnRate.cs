using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RandomSpawnRate
{   
    public GameObject prefab;

    [Range(0, 100f)]
    public float rate;
}
