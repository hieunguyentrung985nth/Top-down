using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    //[SerializeField] private Texture2D cursor;

    [SerializeField] private Canvas canvas;

    private Vector2 startPos;

    private float referenceResolution;

    private void Awake()
    {
        //Vector2 hotspot = new Vector2(cursor.width / 2, cursor.height / 2);
    }

    private void Start()
    {
        referenceResolution = (float)Screen.width / Screen.height;
    }

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        //float scaleFactor = 1920f / Screen.width;

        //startPos = Input.mousePosition.normalized / scaleFactor;

        //cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out startPos);

        transform.position = canvas.transform.TransformPoint(startPos);

        //transform.position = Input.mousePosition;
    }
}
