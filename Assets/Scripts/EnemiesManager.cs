using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField] private InventoryDatabase database;

    List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        enemies = database.enemies.ToList();
    }
    private void Start()
    {
        
    }
}
