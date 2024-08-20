using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CircleWipe : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Camera mainCam;

    private Material material;
    private int fillAmountID = Shader.PropertyToID("_fillAmount");
    private int offsetID = Shader.PropertyToID("_offset");

    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    private bool active = false;
    private float fillAmount = 1f;
    public float targetAlpha = 0.07f;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Image>().material;
        material.SetFloat(fillAmountID, fillAmount);
    }

    // Update is called once per frame
    void Update()
    {
        if (!active) return;
        TrackTarget();

        fillAmount = Mathf.MoveTowards(fillAmount, targetAlpha, Time.deltaTime * 1f);
        material.SetFloat(fillAmountID, fillAmount);

    }

    private void OnEnable()
    {
        eventBroker.Subscribe<LevelEvents.EndLevel>(OnLevelEnd);
    }


    private void OnDisable()
    {
        eventBroker.Unsubscribe<LevelEvents.EndLevel>(OnLevelEnd);
    }

    private void OnLevelEnd(BrokerEvent<LevelEvents.EndLevel> @event)
    {
        if (@event.Payload.Victory)
        {
            active = true;
            targetAlpha = 0.07f;
        }
    }

    private void TrackTarget()
    {

        Vector3 targetPosition = target.transform.position;
        Vector3 cameraPoint = mainCam.WorldToScreenPoint(targetPosition);
        Vector2 uiPoint = cameraPoint / new Vector2(Screen.width, Screen.height);
        material.SetVector(offsetID, uiPoint);
    }

}
