using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [SerializeField] Collider2D coll;

    [SerializeField] private LayerMask playerLayer;

    private EnemyStats stats;

    [SerializeField] private GameObject bulletPrefab;

    private GameObject player;

    private EnemyMovement em;

    private Transform aimPoint1;

    private Transform aimPoint2;

    [SerializeField] private GameObject effect;

    private Animator anim;
    private Vector3 targetPos;
    private Vector3 aimDir;
    private float angle;

    [SerializeField] private LayerMask layerEnviroment;

    private RaycastHit2D hitPoint1;

    private RaycastHit2D hitPoint2;

    private float timeCounter = 0f;

    private GameManager gameManager;

    private void Awake()
    {
        if (FindObjectOfType<PlayerMovement>())
        {
            player = FindObjectOfType<PlayerMovement>().gameObject;
        }

        anim = GetComponent<Animator>();

        em = GetComponent<EnemyMovement>();

        stats = GetComponent<EnemyStats>();

        aimPoint1 = transform.Find("Guns").Find("Firepoint 1");

        aimPoint2 = transform.Find("Guns").Find("Firepoint 2");

        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        timeCounter = stats.GetFireTimeCoolDown();
    }
    private void Update()
    {
        if (gameManager.isPauseGame)
        {
            return;
        }

        if (player == null) return;

        Rotate();

        if (DetectPlayer())
        {
            em.enabled = false;

            if(timeCounter >= stats.GetFireTimeCoolDown())
            {
                Shoot();

                timeCounter = 0f;
            }
            else
            {
                timeCounter += Time.deltaTime;
            }
        }
        else
        {
            Invoke(nameof(TurnOnMovement), 1f);

            timeCounter += Time.deltaTime;
        }
    }
    void Rotate()
    {
        targetPos = player.transform.position;

        aimDir = (player.transform.position - transform.position).normalized;

        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        transform.eulerAngles = new Vector3(0, 0, angle + 90);
    }

    void TurnOnMovement()
    {
        em.enabled = true;
    }

    void Shoot()
    {
        Instantiate(stats.GetEnemy().gun.bullet, aimPoint1.position, aimPoint1.rotation);

        Instantiate(stats.GetEnemy().gun.bullet, aimPoint2.position, aimPoint2.rotation);
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
        RaycastHit2D hit = Physics2D.CircleCast(coll.bounds.center, stats.GetRangeToShoot(), transform.right, 0, playerLayer);
        return hit;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if(stats != null)
            Gizmos.DrawWireSphere(transform.position, stats.GetRangeToShoot());
    }
    public void Attack()
    {
        if (DetectPlayer())
        {
            //player.GetComponent<PlayerHealth>().TakeDamage(stats.GetDamage());
        }
    }
}
