using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth { get; private set; }

    private PlayerStats stats;

    public event Action<float> OnUpdateHealthEvent;

    private PickUpItem PickUpItem;

    private bool isShield = false;

    private float shieldTime = 0;

    public event Action<string> PopUpTextEvent;

    private SpriteRenderer sprite;

    private GameManager gameManager;

    private Color oldColor;

    [SerializeField] private GameObject prefPopUp;

    private Transform popUpPos;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();

        PickUpItem = GetComponent<PickUpItem>();

        sprite = GetComponent<SpriteRenderer>();

        gameManager = FindObjectOfType<GameManager>();

        popUpPos = transform.Find("Popup");
    }

    private void Start()
    {
        oldColor = sprite.color;
    }

    private void Update()
    {
        if (isShield)
        {
            shieldTime -= Time.deltaTime;

            if(shieldTime <= 0)
            {
                isShield = false;

                DisableAnimationInvincible();
            }
        }
    }

    private void OnEnable()
    {
        PickUpItem.OnPickUpHealthEvent += PickUpHealth_OnPickUpHealthEvent;

        PickUpItem.OnPickUpShieldEvent += PickUpShield_OnPickUpShieldEvent;

        PickUpItem.OnPickUpPowerEvent += PickUpItem_OnPickUpPowerEvent;
    }

    private void OnDisable()
    {
        PickUpItem.OnPickUpHealthEvent -= PickUpHealth_OnPickUpHealthEvent;

        PickUpItem.OnPickUpShieldEvent -= PickUpShield_OnPickUpShieldEvent;

        PickUpItem.OnPickUpPowerEvent -= PickUpItem_OnPickUpPowerEvent;
    }

    private void PickUpHealth_OnPickUpHealthEvent(HealthItem obj, Collider2D other)
    {
        Heal(obj, other);
    }

    private void PickUpShield_OnPickUpShieldEvent(float obj)
    {
        Invincible(obj);
    }

    public float GetHealth()
    {
        currentHealth = stats.GetPlayerHealth();

        return currentHealth;
    }

    void Invincible(float time)
    {
        isShield = true;

        shieldTime = time;

        PlayAnimationInvincible();

        PopUpTextEvent?.Invoke("+"+time+"s shield");
    }

    void PlayAnimationInvincible()
    {
        sprite.color = Color.green;
    }

    void DisableAnimationInvincible()
    {
        sprite.color = oldColor;
    }

    private void PickUpItem_OnPickUpPowerEvent(PowerItem arg1, Collider2D arg2)
    {
        float newHealth;

        var hpPower = stats.GetPlayerPowers().Where(p => p.typePower == PowerType.HP).FirstOrDefault();

        if (hpPower?.amount < 3 || hpPower == null)
        {
            newHealth = DataBaseTemp.Instance.maxHealth + stats.CalculatePercentageToNumber(arg1.effectAmount, DataBaseTemp.Instance.maxHealth);

            if (stats.SetPlayerHealth(newHealth, currentHealth))
            {
                if (currentHealth < DataBaseTemp.Instance.maxHealth)
                {
                    currentHealth = Mathf.Clamp(currentHealth + stats.CalculatePercentageToNumber(50, DataBaseTemp.Instance.maxHealth), 0, DataBaseTemp.Instance.maxHealth);

                    OnUpdateHealthEvent?.Invoke(currentHealth);

                    PopUpTextEvent?.Invoke("+Max HP");

                    SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Heal);

                    Destroy(arg2.gameObject);
                }
            }
        }
        else
        {
            if (currentHealth < DataBaseTemp.Instance.maxHealth)
            {
                currentHealth = Mathf.Clamp(currentHealth + stats.CalculatePercentageToNumber(50, DataBaseTemp.Instance.maxHealth), 0, DataBaseTemp.Instance.maxHealth);

                OnUpdateHealthEvent?.Invoke(currentHealth);

                PopUpTextEvent?.Invoke("+Max HP");

                SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Heal);

                Destroy(arg2.gameObject);
            }
        }
    }

    public void TakeDamage(float receiveDamage)
    {
        if(isShield)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - receiveDamage, 0, DataBaseTemp.Instance.maxHealth);

        OnUpdateHealthEvent?.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Dead);

            if (gameManager.IsSurvivalMode())
            {
                EnemyCounter.Instance.PlayerDead();
            }
            else
            {
                gameManager.PlayerDead();

                SoundManager.Instance.StopSFXSound(SoundManager.SFXSound.Movement);
            }           
        }

    }


    private void Heal(HealthItem item, Collider2D other)
    {
        if(currentHealth < DataBaseTemp.Instance.maxHealth)
        {
            currentHealth = Mathf.Clamp(currentHealth + item.health, 0, DataBaseTemp.Instance.maxHealth);

            OnUpdateHealthEvent?.Invoke(currentHealth);

            SoundManager.Instance.PlaySFXSound(SoundManager.SFXSound.Heal);

            IconPopUp.Create(other.transform.position, (int)item.health);

            Destroy(other.gameObject);

            PopUpTextEvent?.Invoke("+" + item.health +" health");
        }       
    }
}
