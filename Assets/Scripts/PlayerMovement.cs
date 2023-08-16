using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed;

    private float dirX;

    private float dirY;

    private Vector2 lastMoveDir;

    private Vector2 moveDir;

    private Rigidbody2D rb;

    private Animator anim;

    private GameManager gameManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        moveSpeed = GetComponent<PlayerStats>().GetPlayerMoveSpeed();

        anim = GetComponent<Animator>();

        gameManager = FindObjectOfType<GameManager>();
    }
    private void Update()
    {
        if (gameManager.isPauseGame)
        {
            return;
        }

        dirX = Input.GetAxisRaw("Horizontal");

        dirY = Input.GetAxisRaw("Vertical");

        moveDir = new Vector2(dirX, dirY);     
       

        if (dirX != 0 || dirY != 0)
        {
            anim.SetBool("isMoving", true);

            lastMoveDir = moveDir;

            anim.SetFloat("Horizontal", lastMoveDir.x);

            anim.SetFloat("Vertical", lastMoveDir.y);

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Movement);
        }
        else
        {
            anim.SetBool("isMoving", false);

            SoundManager.Instance.StopSFXSound(SoundManager.SFXSound.Movement);
        }
    }

    private void FixedUpdate()
    {
        Movement();
    }
    void Movement()
    {
        //rb.MovePosition(new Vector2(dirX, dirY) * moveSpeed * Time.deltaTime);

        //transform.Translate(new Vector3(dirX * moveSpeed * Time.deltaTime, dirY * moveSpeed * Time.deltaTime, 0));

        //rb.MovePosition((Vector2)transform.position + new Vector2(dirX, dirY).normalized * moveSpeed * Time.fixedDeltaTime);

        transform.position += (Vector3)moveDir.normalized * moveSpeed * Time.fixedDeltaTime;
    }
}
