using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private readonly EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();
    private bool triggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        triggered = true;
        eventBrokerComponent.Publish(this, new StarEvents.AddStar());
        eventBrokerComponent.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.Star));
        Destroy(gameObject, .1f);
    }
}
