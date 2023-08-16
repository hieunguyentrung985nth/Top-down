using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IconPopUp : MonoBehaviour
{
    private TextMeshPro textPopUp;

    private float disappearTimer;

    private Color textColor;

    private void Awake()
    {
        textPopUp = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        transform.position += new Vector3(0, 3f) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;

        if(disappearTimer > 1)
        {
            transform.localScale += 1f * Time.deltaTime * Vector3.one;
        }
        else
        {
            transform.localScale -= 1f * Time.deltaTime * Vector3.one;
        }

        if(disappearTimer <= 0)
        {
            float disappearSpeed = 2f;

            textColor.a -= disappearSpeed * Time.deltaTime;

            textPopUp.color = textColor;

            if(textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public static IconPopUp Create(Vector3 position, int value)
    {
        GameObject popUp = Instantiate(GameAssets.i.PopUp, position, Quaternion.identity);

        IconPopUp popUpComp = popUp.GetComponent<IconPopUp>();

        popUpComp.SetUpValue(value);

        return popUpComp;
    }

    private void SetUpValue(int value)
    {
        textPopUp.text = value.ToString();

        disappearTimer = 1f;

        textColor = textPopUp.color;
    }
}
