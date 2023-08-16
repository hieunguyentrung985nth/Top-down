using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Transform aimPoint1;

    private Transform aimPoint2;

    //[SerializeField] private float bulletSpeed = 2f;

    //[SerializeField] private float shootTimeCoolDown = 1f;

    //[SerializeField] private float shootTimeCounter;

    private float angle;

    Vector3 aimDir;

    Vector3 mousePos;

    Rigidbody2D rb;

    [SerializeField] private EnemyWithGuns enemyWeapon;

    private float recoil;

    Vector3 recoilVector;

    Vector3 targetPos;

    [SerializeField] private float distance;

    [SerializeField] private float radius;

    [SerializeField] private LayerMask layer;

    [SerializeField] private LayerMask layerEnviroment;

    Collider2D hits;

    private LineRenderer line;

    [SerializeField] private GameObject effect;

    private GameObject player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        recoil = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().GetPlayerRecoil();

        line = GetComponent<LineRenderer>();

        player = FindObjectOfType<PlayerMovement>().gameObject;
    }

    private void Start()
    {
        GetPlayerPosition();
    }

    private void Update()
    {
        Shoot();
    }
    void GetPlayerPosition()
    {
        targetPos = new Vector2(player.transform.position.x, player.transform.position.y);

        aimDir = (player.transform.position - transform.position).normalized;

        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        //transform.eulerAngles = new Vector3(0, 0, angle);
    }

    private void FixedUpdate()
    {

    }

    void Shoot()
    {
        //transform.Translate(Vector2.down * enemyWeapon.bulletSpeed * Time.smoothDeltaTime);

        transform.position = Vector2.MoveTowards(transform.position, targetPos, enemyWeapon.bulletSpeed * Time.deltaTime);

        hits = Physics2D.OverlapCircle(transform.position, radius, layerEnviroment);

        hitPoint1 = Physics2D.Linecast(transform.position, targetPos, layerEnviroment);

        hitPoint2 = Physics2D.Linecast(transform.position, targetPos, layerEnviroment);

        if (Vector2.Distance(transform.position, targetPos) < .1f)
        {
            //if (!passed)
            //{
            //    var effectins = Instantiate(effect, targetPos, Quaternion.identity);

            //    Destroy(effectins, .5f);
            //}

            CheckRayCast();
        }
    }

    private bool passed = false;

    private RaycastHit2D hitPoint1;

    private RaycastHit2D hitPoint2;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!passed)
        {
            //passed = true;

            //var effectins = Instantiate(effect, collision.ClosestPoint(transform.position), Quaternion.identity);

            //Destroy(effectins, .5f);

            CheckRayCast();
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CheckRayCast();
    }

    public void CheckRayCast()
    {
        if (hits != null)
        {
            hits.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(enemyWeapon.bulletDamage);

            Destroy(gameObject);
        }

        if (Vector2.Distance(transform.position, targetPos) < .1f)
        {
            Destroy(gameObject);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position + transform.right * distance, radius);

        if(aimPoint1 != null)
            Gizmos.DrawLine(aimPoint1.position, targetPos);

        if (aimPoint2 != null)
            Gizmos.DrawLine(aimPoint2.position, targetPos);
    }
}
