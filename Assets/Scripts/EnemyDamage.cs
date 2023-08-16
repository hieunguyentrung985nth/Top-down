using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] Collider2D coll;

    [SerializeField] private LayerMask playerLayer;

    private EnemyStats stats;

    private GameObject player;

    private EnemyMovement em;

    private Animator anim;

    private GameManager gameManager;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>()?.gameObject;

        anim = GetComponent<Animator>();

        em = GetComponent<EnemyMovement>();

        stats = GetComponent<EnemyStats>();

        anim.ResetTrigger("attack");

        gameManager = FindObjectOfType<GameManager>();
    }
    private void Update()
    {
        if (player == null) return;

        if (gameManager.isPauseGame)
        {
            return;
        }

        if (DetectPlayer())
        {
            em.enabled = false;

            anim.SetTrigger("attack");

            StartCoroutine(PlayAttackSound());
        }
        else
        {
            Invoke(nameof(TurnOnMovement), 1f);

            anim.ResetTrigger("attack");
        }
    }

    private bool canPlay = true;

    private IEnumerator PlayAttackSound()
    {
        if (canPlay)
        {
            canPlay = false;

            SoundManager.Instance.PlaySFXSound(SoundManager.Instance.GetEnemyAtkSound(stats.GetEnemy()));

            yield return new WaitForSeconds(1f);

            canPlay = true;
        }       
    }


    void TurnOnMovement()
    {
        em.enabled = true;
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        collision.gameObject.GetComponent<Health>().TakeDamage(1);
    //    }
    //}
    bool DetectPlayer()
    {
        RaycastHit2D hit = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size * stats.GetRange(), 0, Vector2.left, .1f, playerLayer);

        return hit ? true : false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if(stats != null)
            Gizmos.DrawWireCube(coll.bounds.center, new Vector3(coll.bounds.size.x * stats.GetRange(), coll.bounds.size.y * stats.GetRange(), coll.bounds.size.z));
    }
    public void Attack()
    {
        if(this.enabled == true)
        {
            if (DetectPlayer())
            {
                player.GetComponent<PlayerHealth>().TakeDamage(stats.GetDamage());
            }
        }
        
    }
}
