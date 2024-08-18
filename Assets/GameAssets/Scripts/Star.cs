using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        eventBroker.Publish(this, new StarEvents.AddStar());
        Destroy(gameObject, .1f);
    }
}
