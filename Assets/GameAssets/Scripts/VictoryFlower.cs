using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryFlower : MonoBehaviour
{
    [SerializeField] private string nextLevel;
	[SerializeField] private int levelNumber;

    private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated) { return; }

        activated = true;
        PlayerPrefs.SetInt("LevelReached", levelNumber + 1);
        eventBroker.Publish(this, new LevelEvents.EndLevel(nextLevel, true, 5f));
        return;
    }

}
