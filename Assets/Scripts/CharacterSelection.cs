using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public static CharacterSelection Instance { get; private set; }

    private GameManager gameManager;

    private int currentCharacterIndex;

    private Transform characters;

    private PlayerData[] playerDatas;

    private void Awake()
    {
        Instance = this;

        gameManager = FindObjectOfType<GameManager>();

        currentCharacterIndex = 0;

        characters = transform.Find("Characters");
    }

    private void Start()
    {
        playerDatas = gameManager.GetPlayerDatas();

        DisplayPlayerDatas();
    }

    private void OnEnable()
    {
        ChooseCharacter(0);
    }

    public void DisplayPlayerDatas()
    {
        Transform maleCharacterTransform = characters.Find("Male Character");

        Transform femaleCharacterTransform = characters.Find("Female Character");

        GetAllStats(maleCharacterTransform, 0);

        GetAllStats(femaleCharacterTransform, 1);
    }

    private void GetAllStats(Transform current, int gender)
    {
        Transform maleStatsTransform = current.Find("Stats");

        Dictionary<string, Transform> stats = new Dictionary<string, Transform>();

        Transform hp = maleStatsTransform.Find("HP");

        Transform spd = maleStatsTransform.Find("SPD");

        Transform str = maleStatsTransform.Find("STR");

        Transform acc = maleStatsTransform.Find("ACC");

        stats.Add("HP", hp);

        stats.Add("SPD", spd);

        stats.Add("STR", str);

        stats.Add("ACC", acc);

        DisplayStats(stats, gender);      
    }

    private void DisplayStats(Dictionary<string, Transform> stats, int gender)
    {
        Slider hp = stats["HP"].Find("Slider").GetComponent<Slider>();

        Slider spd = stats["SPD"].Find("Slider").GetComponent<Slider>();

        Slider str = stats["STR"].Find("Slider").GetComponent<Slider>();

        Slider acc = stats["ACC"].Find("Slider").GetComponent<Slider>();

        hp.value = playerDatas[gender].maxHealth;

        spd.value = playerDatas[gender].moveSpeed;

        str.value = playerDatas[gender].strength;

        acc.value = acc.maxValue - playerDatas[gender].recoil;

        hp.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, hp.value / hp.maxValue);

        spd.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, spd.value / spd.maxValue);

        str.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, str.value / str.maxValue);

        acc.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, acc.value / acc.maxValue);
    }

    public void ChooseCharacter(int index)
    {
        currentCharacterIndex = index;

        gameManager.SetCharacterIndex(currentCharacterIndex);

        for (int i = 0; i < characters.transform.childCount; i++)
        {
            if(i == currentCharacterIndex)
            {
                characters.transform.GetChild(i).Find("Selected").GetComponent<Image>().enabled = true;
            }
            else
            {
                characters.transform.GetChild(i).Find("Selected").GetComponent<Image>().enabled = false;
            }
        }
    }
}
