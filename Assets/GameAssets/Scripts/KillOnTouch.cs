using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnTouch : MonoBehaviour
{

    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        eventBroker.Publish(this, new LevelEvents.EndLevel("", false));
    }
}
