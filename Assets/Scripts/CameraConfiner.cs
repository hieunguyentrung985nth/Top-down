using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConfiner : MonoBehaviour
{
    private GameManager gameManager;

    private CinemachineConfiner2D confiner;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        
    }

    private void Start()
    {
       
    }

    public void SetConfiner()
    {
        transform.position = gameManager.GetCurrentLevelObject().Find("Spawn Character").transform.position;

        confiner.m_BoundingShape2D = gameManager.GetCurrentLevelObject().Find("Confiner").GetComponent<PolygonCollider2D>();
    }
}
