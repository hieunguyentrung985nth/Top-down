using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class PlayerWeapon : MonoBehaviour
{
    private Transform aimPoint;

    private List<WeaponAmmoAmount> weapons = new List<WeaponAmmoAmount>();

    private List<WeaponAmmoAmount> activeWeapons = new List<WeaponAmmoAmount>();

    [SerializeField] private WeaponItem defaultWeapon;

    [SerializeField] private WeaponAmmoAmount currentWeapon;

    //[SerializeField] private InventoryObject inventory;

    private int currentWeaponIndex = 0;

    private Sprite sprite;

    private Transform firePoint;

    [SerializeField] private float shotgunFire = 5f;

    //[SerializeField] private float shootTimeCoolDown = 1f;

    private float shootTimeCounter;

    private float angle;

    private Vector3 aimDir;

    private PickUpItem PickUpWeapon;

    private PickUpItem PickUpAmmo;

    public event Action<string> PopUpTextEvent;

    public event Action<WeaponAmmoAmount> ChangeWeaponEvent;

    public event Action<WeaponAmmoAmount> ShootingEvent;

    public event Action<WeaponAmmoAmount> ReceivedNewWeaponEvent;

    private int highestWeaponIndex;

    private UIManager UI;

    private PlayerStats playerStats;

    [SerializeField] private GameObject effect;

    private Transform gun;

    private GameManager gameManager;

    [SerializeField] private GameObject prefVFX1;

    [SerializeField] private GameObject prefVFX2;

    private void Awake()
    {
        PickUpWeapon = GetComponent<PickUpItem>();

        PickUpAmmo = GetComponent<PickUpItem>();

        playerStats = GetComponent<PlayerStats>();

        line = GetComponent<LineRenderer>();

        gameManager = FindObjectOfType<GameManager>();

        soundManager = FindObjectOfType<SoundManager>();
    }
    private void Start()
    {
        //inventory.DeleteSave();

        //inventory.LoadPlayer();

        UI = FindObjectOfType<UIManager>();

        weapons = InventoryObject.Instance.Container.weapons;

        if (weapons.Where(w=>w.id == 0).FirstOrDefault() == null)
        {
            weapons.Add(new WeaponAmmoAmount
            {
                id = defaultWeapon.id,

                item = defaultWeapon,

                amount = float.MaxValue,

                status = true
            });

        }

        activeWeapons = weapons.Where(w => w.status == true).OrderBy(w => w.item.index).ToList();

        weapons = weapons.OrderBy(w => w.item.index).ToList();

        currentWeaponIndex = activeWeapons.Count - 1;

        //if (playerStats.GetItems().weaponAmount.Count > 1)
        //{
        //    weapons = playerStats.GetItems().weaponAmount;

        //    activeWeapons = weapons.Where(w => w.status == true).ToList();

        //    weapons = weapons.OrderBy(w => w.item.index).ToList();

        //    currentWeaponIndex = activeWeapons.Count - 1;
        //}
        //else
        //{
        //    weapons.Add(new WeaponAmmoAmount
        //    {
        //        item = defaultWeapon,

        //        amount = float.MaxValue,

        //        status = true
        //    });
        //}

        aimPoint = transform.Find("Aim");

        gun = aimPoint.Find("Gun");

        firePoint = aimPoint.transform.Find("Firepoint");

        highestWeaponIndex = activeWeapons[currentWeaponIndex].item.index;

        currentWeapon = activeWeapons[currentWeaponIndex];

        UI.DisplayInventoryInGame();

        ChangeWeaponEvent?.Invoke(currentWeapon);

        ShootingEvent?.Invoke(currentWeapon);

        shootTimeCounter = currentWeapon.item.fireTimeCooldown;

        gun.GetComponent<SpriteRenderer>().sprite = currentWeapon.item.image;
    }



    private void OnEnable()
    {
        PickUpWeapon.OnPickUpWeaponEvent += PickUpItem_OnPickUpEvent;

        PickUpAmmo.OnPickUpAmmoEvent += PickUpAmmo_OnPickUpAmmoEvent;
    }

    private void OnDisable()
    {
        PickUpWeapon.OnPickUpWeaponEvent -= PickUpItem_OnPickUpEvent;

        PickUpAmmo.OnPickUpAmmoEvent -= PickUpAmmo_OnPickUpAmmoEvent;
    }

    private void Update()
    {
        if (gameManager.isPauseGame)
        {
            return;
        }

        HandleAiming();

        HandleShooting();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeWeapon(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeWeapon(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeWeapon(6);
        }

    }

    public List<WeaponAmmoAmount> GetWeaponAmmoAmount()
    {
        return weapons;
    }

    private void OnDestroy()
    {
        InventoryObject.Instance.Container.weapons = weapons;
    }

    private void PickUpItem_OnPickUpEvent(WeaponItem obj, Collider2D other)
    {
        ReceivedNewWeapon(obj, other);
    }

    private void PickUpAmmo_OnPickUpAmmoEvent(WeaponItem obj, Collider2D other)
    {
        ReceivedAmmo(obj, other);
    }
    //public string PopupText(WeaponItem obj)
    //{
    //    if (ReceivedNewWeapon(obj))
    //    {
    //        return "Received New Weapon: " + obj.weaponName;
    //    }
    //    else
    //    {
    //        return $"Received {playerStats.CalculatePercentage(10, obj.maxAmmo) +" " + obj.weaponName} Ammo";
    //    }
    //}
    void ReceivedAmmo(WeaponItem obj, Collider2D other)
    {
        var weaponExists = weapons.Find(w => w.item.id == obj.id);

        if (weaponExists != null)
        {
            if (weaponExists.amount == DataBaseTemp.Instance.maxAmmoList[obj.id])
            {
                return;
            }

            weaponExists.amount += playerStats.CalculatePercentageToNumber(10, DataBaseTemp.Instance.maxAmmoList[obj.id]);

            weaponExists.amount = Mathf.Clamp(weaponExists.amount, 0, DataBaseTemp.Instance.maxAmmoList[obj.id]);

            weapons = weapons.OrderBy(w => w.item.index).ToList();

            PopUpTextEvent?.Invoke($"+{playerStats.CalculatePercentageToNumber(10, DataBaseTemp.Instance.maxAmmoList[obj.id]) + " " + obj.weaponName} Ammo");

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Ammo);

            if (weaponExists.status == true)
                ShootingEvent?.Invoke(weaponExists);
        }
        else
        {
            WeaponAmmoAmount newWp = new WeaponAmmoAmount
            {
                id = obj.id,

                item = obj,

                amount = playerStats.CalculatePercentageToNumber(10, DataBaseTemp.Instance.maxAmmoList[obj.id]),

                status = false
            };

            weapons.Add(newWp);

            weapons = weapons.OrderBy(w => w.item.index).ToList();

            PopUpTextEvent?.Invoke($"+{playerStats.CalculatePercentageToNumber(10, DataBaseTemp.Instance.maxAmmoList[obj.id]) + " " + obj.weaponName} Ammo");

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Ammo);
        }

        Destroy(other.gameObject);
    }

    void ReceivedNewWeapon(WeaponItem obj, Collider2D other)
    {
        var weaponExists = weapons.Find(w => w.item.id == obj.id);

        if (weaponExists != null)
        {
            if (weaponExists.status == false)
            {
                weaponExists.status = true;

                PopUpTextEvent?.Invoke($"+1 {obj.weaponName}");

                weapons = weapons.OrderBy(w => w.item.index).ToList();

                activeWeapons.Add(weaponExists);

                activeWeapons = activeWeapons.OrderBy(w => w.item.index).ToList();

                highestWeaponIndex = activeWeapons[activeWeapons.Count - 1].item.index;

                if (obj.index >= highestWeaponIndex)
                {
                    currentWeaponIndex = activeWeapons.Count - 1;

                    UpdateWeapon();

                    ChangeWeaponEvent?.Invoke(currentWeapon);
                }

                currentWeaponIndex = activeWeapons.IndexOf(currentWeapon);

                ReceivedNewWeaponEvent?.Invoke(weaponExists);

                SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Ammo);

                Destroy(other.gameObject);
            }
            else
            {
                ReceivedAmmo(obj, other);
            }
        }
        else
        {
            WeaponAmmoAmount newWp = new WeaponAmmoAmount
            {
                id = obj.id,

                item = obj,

                amount = playerStats.CalculatePercentageToNumber(20, DataBaseTemp.Instance.maxAmmoList[obj.id]),

                status = true
            };

            weapons.Add(newWp);

            PopUpTextEvent?.Invoke($"+1 {obj.weaponName}");

            weapons = weapons.OrderBy(w => w.item.index).ToList();

            activeWeapons.Add(newWp);

            activeWeapons = activeWeapons.OrderBy(w => w.item.index).ToList();

            highestWeaponIndex = activeWeapons[activeWeapons.Count - 1].item.index;

            if (obj.index >= highestWeaponIndex)
            {
                currentWeaponIndex = activeWeapons.Count - 1;

                UpdateWeapon();

                ChangeWeaponEvent?.Invoke(currentWeapon);
            }

            currentWeaponIndex = activeWeapons.IndexOf(currentWeapon);

            ReceivedNewWeaponEvent?.Invoke(newWp);

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Ammo);

            Destroy(other.gameObject);
        }


    }

    void ChangeWeapon(int inputKey)
    {
        SoundManager.Instance.StopSFXSound(GetGunSound());

        if (inputKey == currentWeapon.item.index)
            return;
        for (int i = 0; i < activeWeapons.Count; i++)
        {
            if (activeWeapons[i].item.index == inputKey && activeWeapons[i].status == true)
            {
                currentWeaponIndex = i;

                UpdateWeapon();

                ChangeWeaponEvent?.Invoke(activeWeapons[i]);

                break;
            }
        }
    }

    void UpdateWeapon()
    {
        SoundManager.Instance.StopSFXSound(GetGunSound());

        currentWeapon = activeWeapons[currentWeaponIndex];

        shootTimeCounter = currentWeapon.item.fireTimeCooldown;

        aimPoint.Find("Gun").GetComponent<SpriteRenderer>().sprite = currentWeapon.item.image;
    }

    Vector3 distance;

    void HandleAiming()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        aimDir = (mousePos - transform.position).normalized;

        distance = (mousePos - transform.position) / 2f;

        angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;

        aimPoint.eulerAngles = new Vector3(0, 0, angle);

        if(angle > 90 || angle < -90)
        {
            aimPoint.localScale = new Vector3(gun.localScale.x, -1f, gun.localScale.z);
        }
        else
        {
            aimPoint.localScale = new Vector3(gun.localScale.x, 1f, gun.localScale.z);
        }
    }

    RaycastHit2D[] hits;

    Collider2D[] ray;
    private Vector3 mousePos;
    public Vector3 recoilVector;
    private Vector3 targetPos;
    private LineRenderer line;

    [SerializeField] private float radius;
    [SerializeField] private float d;
    [SerializeField] private LayerMask enviromentLayer;

    private SoundManager soundManager;

    private SoundManager.SFXSound GetGunSound()
    {
        switch (currentWeapon.item.id)
        {
            case 0:
                return SoundManager.SFXSound.Pistols;
            case 1:
                return SoundManager.SFXSound.Shotgun;
            case 2:
                return SoundManager.SFXSound.RocketLauncher;
            case 3:
                return SoundManager.SFXSound.AK47;
            case 4:
                return SoundManager.SFXSound.Laser;
            case 5:
                return SoundManager.SFXSound.Nuclear;
            default:
                return SoundManager.SFXSound.NULL;
        }
    }

    //private float muzzleTimer = 0.3f;

    //private float muzzleCounter = 0f;

    private void TurnOnFireVFX()
    {
        if (!prefVFX1.activeInHierarchy)
        {
            prefVFX2.SetActive(true);
        }
        else
        {
            //prefVFX2.SetActive(false);

            prefVFX2.SetActive(true);
        }

        StartCoroutine(TurnOffFireVFXWhileHolding());

        return;

        //if (currentWeapon.item.id == 0 || currentWeapon.item.id == 1)
        //{
            
        //}

        //if(muzzleCounter < muzzleTimer)
        //{
        //    prefVFX1.SetActive(true);

        //    muzzleCounter = muzzleTimer;
        //}
        //else
        //{
        //    prefVFX2.SetActive(true);

        //    prefVFX1.SetActive(false);
        //}
    }

    private IEnumerator TurnOffFireVFXWhileHolding()
    {
        if(currentWeapon.item.id == 1 || currentWeapon.item.id == 4 || currentWeapon.item.id == 2)
        {
            yield return new WaitForSeconds(0.3f);

            prefVFX2.SetActive(false);
        }
    }

    private void TurnOffFireVFX()
    {
        //yield return new WaitForSeconds(prefVFX1.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
       

        prefVFX1.SetActive(false);

        prefVFX2.SetActive(false);

        return;

        //if (currentWeapon.item.id == 0 || currentWeapon.item.id == 1)
        //{
            
        //}

        //muzzleCounter = 0f;

        //prefVFX1.SetActive(true);

        //prefVFX2.SetActive(false);

        ////yield return new WaitForSeconds(prefVFX1.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

        //prefVFX1.SetActive(false);
    }

    private void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            if (shootTimeCounter >= currentWeapon.item.fireTimeCooldown)
            {
                if (currentWeapon.amount > 0)
                {
                    soundManager.PlaySFXSound(GetGunSound());

                    TurnOnFireVFX();

                    mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    recoilVector = new Vector3(UnityEngine.Random.Range(-playerStats.GetPlayerRecoil(), playerStats.GetPlayerRecoil()), UnityEngine.Random.Range(-playerStats.GetPlayerRecoil(), playerStats.GetPlayerRecoil()), 0);

                    targetPos = mousePos + recoilVector;

                    RaycastHit2D laserHit = Physics2D.Linecast(firePoint.position, targetPos, enviromentLayer);

                    if (currentWeapon.item.weaponName == "Rocket Launcher" || currentWeapon.item.weaponName == "Nuclear")
                    {
                        Rocket();
                    }

                    else if (currentWeapon.item.weaponName == "Laser")
                    {

                        line.enabled = true;

                        line.SetPosition(0, firePoint.position);

                        line.SetPosition(1, targetPos);

                        if (laserHit)
                        {
                            line.SetPosition(1, laserHit.point);
                        }

                        ray = Physics2D.OverlapCircleAll(line.GetPosition(1), currentWeapon.item.radius);

                        Laser();

                        var effectins = Instantiate(currentWeapon.item.hitVFX, new Vector3(line.GetPosition(1).x, line.GetPosition(1).y, 0), Quaternion.identity);

                        Destroy(effectins, .5f);

                        CameraShake.Instance.ShakeCamera(1.5f, 0.2f);

                        Invoke(nameof(DisableLaser), 0.2f);
                    }

                    else if (currentWeapon.item.weaponName == "AK 47")
                    {
                        if (laserHit)
                        {
                            ray = Physics2D.OverlapCircleAll(laserHit.point, 0.3f);

                            var effectins = Instantiate(effect, new Vector3(laserHit.point.x, laserHit.point.y, 0), Quaternion.identity);

                            Destroy(effectins, .5f);
                        }
                        else
                        {
                            ray = Physics2D.OverlapCircleAll(targetPos, 0.3f);

                            var effectins = Instantiate(effect, new Vector3(targetPos.x, targetPos.y, 0), Quaternion.identity);

                            Destroy(effectins, .5f);
                        }

                        CheckRayCast2();
                    }

                    else if (currentWeapon.item.weaponName == "Shotgun")
                    {
                        for (int i = 0; i < shotgunFire; i++)
                        {
                            Vector3 random = new Vector3(UnityEngine.Random.Range(-playerStats.GetPlayerRecoil(), playerStats.GetPlayerRecoil()), UnityEngine.Random.Range(-playerStats.GetPlayerRecoil(), playerStats.GetPlayerRecoil()), 0);

                            RaycastHit2D laserHitt = Physics2D.Linecast(firePoint.position, targetPos + random, enviromentLayer);

                            if (laserHitt)
                            {
                                hits = Physics2D.LinecastAll(aimPoint.position, laserHit.point);

                                var effectins = Instantiate(effect, new Vector3(laserHit.point.x, laserHit.point.y, 0), Quaternion.identity);

                                Destroy(effectins, .5f);
                            }
                            else
                            {
                                hits = Physics2D.LinecastAll(aimPoint.position, targetPos + random);

                                var effectins = Instantiate(effect, new Vector3(targetPos.x + random.x, targetPos.y + random.y, 0), Quaternion.identity);

                                Destroy(effectins, .5f);
                            }
                            CheckRayCast();
                        }
                    }

                    else
                    {
                        if (laserHit)
                        {
                            hits = Physics2D.LinecastAll(aimPoint.position, laserHit.point);

                            var effectins = Instantiate(effect, new Vector3(laserHit.point.x, laserHit.point.y, 0), Quaternion.identity);

                            Destroy(effectins, .5f);
                        }
                        else
                        {
                            hits = Physics2D.LinecastAll(aimPoint.position, targetPos);

                            var effectins = Instantiate(effect, new Vector3(targetPos.x, targetPos.y, 0), Quaternion.identity);

                            Destroy(effectins, .5f);
                        }

                        CheckRayCast();
                    }

                    if (hits != null)
                    {

                    }

                    if (currentWeapon.item.weaponName != "Duo Pistols")
                    {
                        currentWeapon.amount--;

                        ShootingEvent?.Invoke(currentWeapon);

                        //weapons[currentWeaponIndex].amount = currentWeapon.amount;

                        if (currentWeapon.amount <= 0)
                        {

                            currentWeaponIndex--;

                            UpdateWeapon();

                            ChangeWeaponEvent?.Invoke(currentWeapon);
                        }
                    }
                }
                else
                {


                    currentWeaponIndex--;

                    UpdateWeapon();

                    ChangeWeaponEvent?.Invoke(currentWeapon);
                }

                shootTimeCounter = 0f;
            }
            else
            {
                shootTimeCounter += Time.deltaTime;

            }
        }
        else
        {
            shootTimeCounter += Time.deltaTime;

            
        }

        if ((Input.GetMouseButtonUp(0)) && currentWeapon.item.itemName == "Laser")
        {
            DisableLaser();
        }

        if (Input.GetMouseButtonUp(0))
        {
            soundManager.StopSFXSound(GetGunSound());

            TurnOffFireVFX();
        }
    }
    void DisableLaser()
    {
        line.enabled = false;
    }

    void CheckRayCast()
    {
        int count = 0;

        if (hits != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.CompareTag("Enemy"))
                {
                    count++;

                    if (count >= 2)
                    {
                        hits[i].collider.GetComponent<EnemyHealth>()?.IsSimulation(true);
                    }
                    else
                    {
                        hits[i].collider.GetComponent<EnemyHealth>()?.IsSimulation(false);
                    }

                    hits[i].collider.GetComponent<EnemyHealth>()?.TakeDamage(currentWeapon.item.damage);
                }

                else if (hits[i].collider.CompareTag("Box"))
                {
                    hits[i].collider.GetComponent<Box>()?.DisplayItem();
                }
            }

            foreach (var hit in hits)
            {
                
            }
        }


    }
    void CheckRayCast2()
    {
        if (ray != null)
        {
            foreach (var hit in ray)
            {
                if (hit.gameObject.CompareTag("Enemy"))
                {
                    hit.gameObject.GetComponent<EnemyHealth>()?.TakeDamage(currentWeapon.item.damage);
                }

                else if (hit.gameObject.CompareTag("Box"))
                {
                    hit.gameObject.GetComponent<Box>()?.DisplayItem();
                }
            }
        }

    }
    void Rocket()
    {
        Instantiate(currentWeapon.item.prefab, firePoint.position, firePoint.rotation);


    }

    void Laser()
    {
        if (ray != null)
        {
            foreach (var hit in ray)
            {
                if (hit.gameObject.CompareTag("Enemy"))
                {
                    hit.gameObject.GetComponent<EnemyHealth>()?.TakeDamage(currentWeapon.item.damage);
                }

                else if (hit.gameObject.CompareTag("Box"))
                {
                    hit.gameObject.GetComponent<Box>()?.DisplayItem();
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (aimPoint != null)
            Gizmos.DrawWireSphere(line.GetPosition(1), currentWeapon.item.radius);
    }
}
