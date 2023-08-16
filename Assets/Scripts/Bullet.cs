using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform aimPoint;

    private Transform firePoint;

    //[SerializeField] private float bulletSpeed = 2f;

    //[SerializeField] private float shootTimeCoolDown = 1f;

    //[SerializeField] private float shootTimeCounter;

    private float angle;

    Vector3 aimDir;

    Vector3 mousePos;

    Rigidbody2D rb;

    [SerializeField] private WeaponItem playerWeapon;

    private float recoil;

    Vector3 recoilVector;

    Vector3 targetPos;

    [SerializeField] private float distance;

    [SerializeField] private float radius;

    [SerializeField] private LayerMask layer;

    [SerializeField] private LayerMask layerEnviroment;

    RaycastHit2D[] hits;

    private LineRenderer line;

    [SerializeField] private GameObject effect;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        recoil = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>().GetPlayerRecoil();

        aimPoint = GameObject.FindGameObjectWithTag("Firepoint").transform;

        line = GetComponent<LineRenderer>();
    }

    private void Start()
    {

        GetMousePosition();

    }

    private void Update()
    {
        Shoot();
    }
    void GetMousePosition()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        aimDir = (mousePos - transform.position).normalized;

        recoilVector = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWeapon>().recoilVector;

        targetPos = mousePos + recoilVector;

        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        var s = Vector2.Distance(aimPoint.position, targetPos);

        //if(playerWeapon.itemName == "Laser")
        //{
        //    transform.localScale = new Vector2(s, transform.localScale.y);

        //    distance = s / 2;

        //    GetComponent<BoxCollider2D>().size = new Vector2(s, transform.localScale.y);
        //}

        transform.eulerAngles = new Vector3(0, 0, angle);

    }

    private void FixedUpdate()
    {
       

    }
    void Shoot()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, playerWeapon.bulletSpeed * Time.deltaTime);

        hits = Physics2D.CircleCastAll(transform.position, playerWeapon.radius, transform.right, distance, layerEnviroment);

        hitPoint = Physics2D.Linecast(aimPoint.position, targetPos, layerEnviroment);


        if (Vector2.Distance(transform.position, targetPos) < .1f)
        {
            if (!passed)
            {
                if (hitPoint != false)
                {
                    effectins = Instantiate(playerWeapon.hitVFX, new Vector3(hitPoint.point.x, hitPoint.point.y, 0), Quaternion.identity);
                }
                else
                {
                    effectins = Instantiate(playerWeapon.hitVFX, new Vector3(targetPos.x, targetPos.y, 0), Quaternion.identity);
                }

                Destroy(effectins, .5f);
            }

            Destroy(gameObject);
        }
        
        //rb.velocity = new Vector2(mousePos.x, mousePos.y) * bulletSpeed * Time.deltaTime;
    }

    private bool passed = false;

    private GameObject effectins = null;

    private RaycastHit2D hitPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!passed)
        {
            passed = true;

            effectins = Instantiate(playerWeapon.hitVFX, collision.ClosestPoint(transform.position), Quaternion.identity);

            Destroy(effectins, .5f);

            Rocket();
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rocket();
    }

    void CheckRayCast()
    {
        if(hits != null)
        {
            foreach (var raycast in hits)
            {
                raycast.collider.GetComponent<EnemyHealth>()?.TakeDamage(playerWeapon.damage);

                if (playerWeapon.itemName == "Rocket Launcher" || playerWeapon.itemName == "Nuclear")
                {
                    Destroy(gameObject);
                }
            }
            
        }

        if (Vector2.Distance(transform.position, targetPos) < .1f)
        {
            
        }
    }

    public void Rocket()
    {
        if (hits != null)
        {
            foreach (var raycast in hits)
            {
                if (raycast.collider.CompareTag("Enemy"))
                {
                    raycast.collider.GetComponent<EnemyHealth>()?.TakeDamage(playerWeapon.damage);
                }

                else if (raycast.collider.CompareTag("Box"))
                {
                    raycast.collider.GetComponent<Box>()?.DisplayItem();
                }
                    
            }
            if (playerWeapon.itemName == "Rocket Launcher" || playerWeapon.itemName == "Nuclear")
            {
                Destroy(gameObject);
            }

        }



        if (Vector2.Distance(transform.position, targetPos) < .1f)
        {
            //if (!passed)
            //{
            //    effectins = Instantiate(effect, new Vector3(targetPos.x, targetPos.y, 0), Quaternion.identity);

            //    Destroy(effectins, .5f);
            //}

            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position + transform.right * distance, radius);
        if(aimPoint != null)
            Gizmos.DrawLine(aimPoint.position, targetPos);
    }
}
