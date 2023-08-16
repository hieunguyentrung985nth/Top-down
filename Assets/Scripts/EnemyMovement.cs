using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyStats stats;

    private GameObject player;

    Rigidbody2D rb;

    SpriteRenderer spriteRenderer;

    Vector2 direction;

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>()?.gameObject;

        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        stats = GetComponent<EnemyStats>();
    }

    private void Update()
    {
        if (player == null) return;

        direction = player.transform.position - transform.position;
    }
    private void FixedUpdate()
    {
        if (player == null) return;

        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, stats.GetMoveSpeed() * Time.fixedDeltaTime);

        //rb.MovePosition((Vector2)transform.position + stats.GetMoveSpeed() * Time.fixedDeltaTime * direction.normalized);
    }
}
