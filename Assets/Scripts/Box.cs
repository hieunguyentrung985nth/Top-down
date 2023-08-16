using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private GameObject itemToDrop;

    [SerializeField] private RandomSpawnRate[] itemsInBox;

    [SerializeField] private GameObject[] itemInSurvivalMode;

    private float totalWeights;

    private System.Random rand = new System.Random();

    private GameObject itemCollection;

    private GameManager gameManager;

    [SerializeField] private bool canBeChanged = true;

    private SurvivalModeBattle survivalModeBattle;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        survivalModeBattle = FindObjectOfType<SurvivalModeBattle>();
    }

    private void Start()
    {
        itemCollection = gameManager.GetCurrentLevelObject().transform.Find("Item Collection").gameObject;
    }

    private void OnEnable()
    {
        
    }

    public void DisplayItem()
    {
        if (canBeChanged)
        {
            if (gameManager.IsSurvivalMode())
            {
                if(survivalModeBattle.ChangeItemsOverTime() != null)
                {
                    itemsInBox = survivalModeBattle.ChangeItemsOverTime();
                }
                else
                {
                    itemsInBox = null;
                }
            }
            else
            {
                itemsInBox = gameManager.ChangeItemsOverTime();
            }          
        }

        totalWeights = CalculateWeights();

        itemToDrop = SpawnRandomItem();

        if(itemInSurvivalMode.Length > 0)
        {
            Instantiate(itemInSurvivalMode[0], transform.position, Quaternion.identity, itemCollection.transform);                 
        }
        else if(itemToDrop != null)
        {
            Instantiate(itemToDrop, transform.position, Quaternion.identity, itemCollection.transform);
        }

        GetComponent<BoxCollider2D>().enabled = false;

        transform.Find("Piece 1")?.gameObject.SetActive(false);

        transform.Find("Piece 2")?.gameObject.SetActive(false);

        //transform.Find("Broken").gameObject.SetActive(true);


    }

    private int GetRandomItemIndex()
    {
        float r = (float)rand.NextDouble();

        float adding = 0f;

        for (int i = 0; i < itemsInBox.Length; i++)
        {
            if (itemsInBox[i].rate / totalWeights + adding >= r)
            {
                return i;
            }
            else
            {
                adding += itemsInBox[i].rate / totalWeights;
            }
        }
        return -1;
    }

    private float CalculateWeights()
    {
        float total = 0;

        if(itemsInBox != null)
        {
            for (int i = 0; i < itemsInBox.Length; i++)
            {
                total += itemsInBox[i].rate;
            }
        }       

        return total;
    }

    public GameObject SpawnRandomItem()
    {
        if(itemsInBox != null)
        {
            RandomSpawnRate item = itemsInBox[GetRandomItemIndex()];

            return item.prefab;
        }

        return null;
    }

    public void SpawnFirstItem()
    {
        if(itemInSurvivalMode.Length > 1)
        {
            Instantiate(itemInSurvivalMode[1], transform.position, Quaternion.identity, itemCollection.transform);
        }       
    }
}
