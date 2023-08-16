using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player")]
public class PlayerData : ScriptableObject
{
    public int id;

    [Range(0, 1)]
    public int gender;

    public float moveSpeed;

    public float recoil;

    public float maxHealth;

    public float strength;
}
