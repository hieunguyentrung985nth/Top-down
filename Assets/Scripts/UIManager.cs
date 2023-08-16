using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private TextMeshProUGUI popup;

    [Header("In Game UI")]
    [SerializeField] private GameObject inGameUI;

    [Header("Display Inventory Ingame")]
    [SerializeField] private RectTransform inventoryTemplate;

    [SerializeField] private RectTransform inventoryContainer;

    [SerializeField] private InventoryObject inventory;

    [SerializeField] private static readonly int maxWeaponSlots = 6;


    private Queue<string> popupTextQueue = new Queue<string>(5);

    private PickUpItem PickUpItem;

    private GameObject player;

    [Header("Player Weapon Events")]
    private PlayerWeapon PopUpText;

    private PlayerWeapon ChangeWeapon;

    private PlayerWeapon Shooting;

    private PlayerWeapon ReceivedNewWeapon;

    [Header("Player Money")]
    private PlayerMoney PopUpTextMoney;

    [Header("Stage Cleared")]
    [SerializeField] private TextMeshProUGUI stageClearedText;

    private PlayerHealth PlayerHealth;

    private int previousIndexWeapon;

    private Transform currentSlot;


    private PlayerStats playerStats;

    private GameManager gameManager;

    private void Awake()
    {
        //inventory.LoadPlayer();

        gameManager = FindObjectOfType<GameManager>();

        inGameUI.gameObject.SetActive(false);

        stageClearedText.enabled = false;
       
        //DisplayInventoryInGame();
    }

    private void Start()
    {
        ClearQueue();
    }

    private void OnEnable()
    {
        //inGameUI.gameObject.SetActive(true);

        //SetUp();
    }

    private void SetUp()
    {
        stageClearedText.enabled = false;

        player = GameObject.FindGameObjectWithTag("Player");

        PickUpItem = player.GetComponent<PickUpItem>();

        PopUpText = player.GetComponent<PlayerWeapon>();

        ChangeWeapon = player.GetComponent<PlayerWeapon>();

        Shooting = player.GetComponent<PlayerWeapon>();

        ReceivedNewWeapon = player.GetComponent<PlayerWeapon>();

        PlayerHealth = player.GetComponent<PlayerHealth>();

        PopUpTextMoney = player.GetComponent<PlayerMoney>();

        playerStats = player.GetComponent<PlayerStats>();

        inventoryTemplate.gameObject.SetActive(false);

        gameManager.OnStageClearedEvent += GameManager_OnStageClearedEvent;

        PopUpText.PopUpTextEvent += PopUpText_PopUpTextEvent;

        PlayerHealth.OnUpdateHealthEvent += PlayerHealth_OnUpdateHealthEvent;

        ChangeWeapon.ChangeWeaponEvent += ChangeWeapon_ChangeWeaponEvent;

        Shooting.ShootingEvent += Shooting_ShootingEvent;

        ReceivedNewWeapon.ReceivedNewWeaponEvent += ReceivedNewWeapon_ReceivedNewWeaponEvent;

        PlayerHealth.PopUpTextEvent += PlayerHealth_PopUpTextEvent;

        PopUpTextMoney.PopUpTextEvent += PopUpTextMoney_PopUpTextEvent;

        healthText.text = player.GetComponent<PlayerHealth>().GetHealth().ToString();

        previousIndexWeapon = 0;
    }

    private void GameManager_OnStageClearedEvent()
    {
        stageClearedText.enabled = true;
    }

    private void OnDisable()
    {       
        //gameManager.OnStageClearedEvent -= GameManager_OnStageClearedEvent;

        //PopUpText.PopUpTextEvent -= PopUpText_PopUpTextEvent;

        //PlayerHealth.OnUpdateHealthEvent -= PlayerHealth_OnUpdateHealthEvent;

        //ChangeWeapon.ChangeWeaponEvent -= ChangeWeapon_ChangeWeaponEvent;

        //Shooting.ShootingEvent -= Shooting_ShootingEvent;

        //ReceivedNewWeapon.ReceivedNewWeaponEvent -= ReceivedNewWeapon_ReceivedNewWeaponEvent;

        //PlayerHealth.PopUpTextEvent -= PlayerHealth_PopUpTextEvent;

        //PopUpTextMoney.PopUpTextEvent -= PopUpTextMoney_PopUpTextEvent;

        //inGameUI?.SetActive(false);
    }

    private void PopUpTextMoney_PopUpTextEvent(string obj)
    {
        PopUpText_PopUpTextEvent(obj);
    }

    private void PlayerHealth_PopUpTextEvent(string obj)
    {
        PopUpText_PopUpTextEvent(obj);
    }

    private void PlayerHealth_OnUpdateHealthEvent(float obj)
    {
        UpdateHealth(obj);
    }

    private void Shooting_ShootingEvent(WeaponAmmoAmount wp)
    {
        UpdateAmmo(grid[wp.item.index - 1], wp);
    }
    private void ReceivedNewWeapon_ReceivedNewWeaponEvent(WeaponAmmoAmount wp)
    {
        ShowWeapon(wp);
    }

    private void ChangeWeapon_ChangeWeaponEvent(WeaponAmmoAmount wp)
    {
        ChangeActiveWeapon(wp);

        if(previousIndexWeapon != wp.item.index)
            previousIndexWeapon = wp.item.index;
    }

    private void PopUpText_PopUpTextEvent(string obj)
    {
        StopAllCoroutines();

        StartCoroutine(DequeueText(obj));  
    }

    public void ClearQueue()
    {
        if(popupTextQueue.Count > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                popupTextQueue?.Dequeue();
            }
        }
        
        for (int i = 0; i < 5; i++)
        {
            popupTextQueue.Enqueue("");
        }
    }

    private IEnumerator DequeueText(string obj)
    {
        string tmp = "";

        popupTextQueue.Dequeue();

        popupTextQueue.Enqueue(obj);

        foreach (var text in popupTextQueue)
        {
            tmp += text + "\r\n";
        }

        popup.text = tmp;

        while(popup.text != "")
        {
            yield return new WaitForSeconds(3f);

            popupTextQueue.Dequeue();

            popupTextQueue.Enqueue("");

            tmp = "";

            foreach (var text in popupTextQueue)
            {
                tmp += text + "\r\n";
            }

            popup.text = tmp;
        }      
    }
    public void UpdateHealth(float obj)
    {
        healthText.text = obj.ToString();
    }

    private Transform[] grid = new Transform[maxWeaponSlots];

    public void DisplayInventoryInGame()
    {
        SetUp();

        Vector3 offset = new Vector3(0, -100, 0);

        for (int i = 0; i < maxWeaponSlots; i++)
        {
            Transform slotTransform = Instantiate(inventoryTemplate, inventoryContainer);

            grid[i] = slotTransform;

            slotTransform.gameObject.SetActive(true);

            var wp = inventory.Container.weapons.Where(w => w.item.index - 1 == i).FirstOrDefault();

            if (wp != null && wp.status == true)
            {
                UpdateAmmo(slotTransform, wp);

                DisplayInactiveWeapon(slotTransform);
            }
            else
            {
                DisplayEmptySlot(slotTransform);
            }

        }
    }
    void ChangeActiveWeapon(WeaponAmmoAmount wp)
    {
        DisplayActiveWeapon(grid[wp.item.index - 1], wp);

        if (previousIndexWeapon > 0)
            DisplayInactiveWeapon(grid[previousIndexWeapon - 1]);
    }

    void DisplayActiveWeapon(Transform instance, WeaponAmmoAmount wp)
    {
        instance.GetComponent<Image>().enabled = true;

        instance.Find("Weapon Image").GetComponent<Image>().enabled = true;

        instance.Find("Ammo Text").GetComponent<TextMeshProUGUI>().enabled = true;

        UpdateAmmo(instance, wp);

        instance.Find("Weapon Image").GetComponent<Image>().sprite = wp.item.image;     

        if (wp.item.id == 0)
            instance.Find("Ammo Text").GetComponent<TextMeshProUGUI>().text = "999999";
        else
            instance.Find("Ammo Text").GetComponent<TextMeshProUGUI>().text = wp.amount.ToString();
    }

    void UpdateAmmo(Transform instance, WeaponAmmoAmount wp)
    {
        if(wp.id != 0)
            instance.Find("Ammo Text").GetComponent<TextMeshProUGUI>().text = wp.amount.ToString();
        else
            instance.Find("Ammo Text").GetComponent<TextMeshProUGUI>().text = "999999";

        instance.Find("Ammo Slider").Find("Background").GetComponent<Image>().enabled = true;

        instance.Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().enabled = true;

        instance.Find("Ammo Slider").Find("BG Index").Find("Weapon Index").GetComponent<TextMeshProUGUI>().enabled = true;

        instance.Find("Ammo Slider").Find("BG Index").GetComponent<Image>().enabled = true;

        instance.Find("Ammo Slider").Find("BG Index").Find("Weapon Index").GetComponent<TextMeshProUGUI>().text = wp.item.index.ToString();

        instance.Find("Ammo Slider").GetComponent<Slider>().value = playerStats.CalculateNumberToPercentage(wp.amount, DataBaseTemp.Instance.maxAmmoList[wp.item.id]) / 100;

        var currentAmmo = playerStats.CalculateNumberToPercentage(wp.amount, DataBaseTemp.Instance.maxAmmoList[wp.item.id]);

        instance.Find("Ammo Slider").GetComponent<Slider>().value = Mathf.Lerp(0, 1, currentAmmo / 100);

        instance.Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, currentAmmo / 100);

        //if (currentAmmo >= 80 || wp.id == 0)
        //{
        //    instance.Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.green;
        //}
        //else if (60 <= currentAmmo && currentAmmo < 80)
        //{
        //    instance.Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color32(175, 255, 0, 255);
        //}
        //else if (40 <= currentAmmo && currentAmmo < 60)
        //{
        //    instance.Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.yellow;
        //}
        //else if (20 <= currentAmmo && currentAmmo < 40)
        //{
        //    instance.Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = new Color32(255, 98, 0, 255);
        //}
        //else
        //{
        //    instance.Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.red;
        //}
    }

    void ShowWeapon(WeaponAmmoAmount wp)
    {
        UpdateAmmo(grid[wp.item.index - 1], wp);
    }

    void DisplayEmptySlot(Transform instance)
    {
        instance.GetComponent<Image>().enabled = false;

        instance.Find("Ammo Slider").Find("Background").GetComponent<Image>().enabled = false;

        instance.Find("Ammo Slider").Find("Fill Area").Find("Fill").GetComponent<Image>().enabled = false;

        instance.Find("Ammo Slider").Find("BG Index").Find("Weapon Index").GetComponent<TextMeshProUGUI>().enabled = false;

        instance.Find("Ammo Slider").Find("BG Index").GetComponent<Image>().enabled = false;

        instance.Find("Weapon Image").GetComponent<Image>().enabled = false;

        instance.Find("Ammo Text").GetComponent<TextMeshProUGUI>().enabled = false;
    }

    void DisplayInactiveWeapon(Transform instance)
    {
        instance.GetComponent<Image>().enabled = false;

        instance.Find("Weapon Image").GetComponent<Image>().enabled = false;

        instance.Find("Ammo Text").GetComponent<TextMeshProUGUI>().enabled = false;

        
    }
}
