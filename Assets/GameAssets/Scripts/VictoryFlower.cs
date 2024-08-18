using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryFlower : MonoBehaviour
{
    [SerializeField] private string nextLevel;

    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) { return; }

        activated = true;
        eventBroker.Publish(this, new LevelEvents.EndLevel(nextLevel, true));
        return;
    }

}
