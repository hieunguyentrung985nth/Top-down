using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameTransition : MonoBehaviour
{
    public static InGameTransition Instance { get; private set; }

    private Animator anim;

    private GameObject currentSceneOff;

    private GameObject currentSceneOn;

    [SerializeField] private GameObject inGameUI;

    private GameManager gameManager;


    private void Awake()
    {
        Instance = this;

        gameManager = FindObjectOfType<GameManager>();

        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        //if (isTransitioning)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        StopAllCoroutines();

        //        EndTransition(currentSceneOff, currentSceneOn);
        //    }
        //}
    }

    public IEnumerator StartTransition(GameObject raycast)
    {
        raycast.SetActive(false);

        gameObject.SetActive(true);

        anim.ResetTrigger("startTransition");

        anim.SetTrigger("startTransition");

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

        inGameUI.SetActive(true);      

        gameObject.SetActive(false);
    }

    public IEnumerator EndTransition(GameObject sceneOn, bool isClearedCampaign = false)
    {
        gameObject.SetActive(true);

        anim.ResetTrigger("endTransition");

        anim.SetTrigger("endTransition");

        yield return new WaitForSecondsRealtime(anim.GetCurrentAnimatorStateInfo(0).length);

        gameManager.GetCurrentLevelObject().gameObject.SetActive(false);

        StartCoroutine(TransitionAnimation.Instance.StartTransition2(sceneOn, isClearedCampaign));

        //inGameUI.SetActive(false);
    }
}
