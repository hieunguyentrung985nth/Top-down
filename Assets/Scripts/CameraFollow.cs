using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Camera cam;

    private Transform player;

    [SerializeField] private float range;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void TurnOnCameraFollow()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    private void Update()
    {
        if (gameManager.isPauseGame)
        {
            return;
        }

        if (player != null)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 targetPos = (player.position + mousePos) / 2f;

            targetPos.x = Mathf.Clamp(targetPos.x, -range + player.position.x, range + player.position.x);

            targetPos.y = Mathf.Clamp(targetPos.y, -range + player.position.y, range + player.position.y);

            transform.position = targetPos;
        }
        else
        {
            return;
        }
    }
}
