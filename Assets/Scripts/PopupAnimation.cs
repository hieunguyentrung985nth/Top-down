using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupAnimation : MonoBehaviour
{
    public static PopupAnimation Instance { get ; private set; }

    private Animator anim;

    private TextMeshProUGUI popupText;

    private void Awake()
    {
        Instance = this;

        anim = GetComponent<Animator>();

        popupText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator StartPopUp(string valueText)
    {
        popupText.text = valueText;

        gameObject.SetActive(true);

        anim.ResetTrigger("startPopup");

        anim.SetTrigger("startPopup");

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.ResetTrigger("endPopup");

        anim.SetTrigger("endPopup");

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

        gameObject.SetActive(false);
    } 
}
