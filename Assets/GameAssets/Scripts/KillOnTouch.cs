using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnTouch : MonoBehaviour
{

    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    private bool canKill = true;

    private void OnEnable()
    {
        eventBroker.Subscribe<LevelEvents.EndLevel>(EndLevelHander);
    }

    private void OnDisable()
    {
        eventBroker.Unsubscribe<LevelEvents.EndLevel>(EndLevelHander);
    }

    private void EndLevelHander(BrokerEvent<LevelEvents.EndLevel> @event)
    {
        if (@event.Payload.Victory)
        {
            canKill = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canKill) { return; }
        eventBroker.Publish(this, new LevelEvents.EndLevel("", false));
    }
}
