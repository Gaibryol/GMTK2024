using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryFlower : MonoBehaviour, IInteractable
{
    [SerializeField] private string nextLevel;

    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    private bool activated = false;

    public bool Interact(GameObject source)
    {
        if (activated) { return false; }

        activated = true;
        eventBroker.Publish(this, new LevelEvents.EndLevel(nextLevel, true));
        return false;
    }

    public bool StopInteract()
    {
        return true;
    }

}
