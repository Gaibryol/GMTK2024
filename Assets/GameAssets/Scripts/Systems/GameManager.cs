using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    private string currentSceneName;

	private int starsCollected;

    private void Start()
    {
        eventBroker.Publish(this, new AudioEvents.PlayMusic(currentSceneName));
		starsCollected = 0;
    }

    private void OnEnable()
    {
        eventBroker.Subscribe<StarEvents.AddStar>(OnAddStar);
        eventBroker.Subscribe<LevelEvents.EndLevel>(OnLevelEnd);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDisable()
    {
        eventBroker.Unsubscribe<LevelEvents.EndLevel>(OnLevelEnd);
        eventBroker.Unsubscribe<StarEvents.AddStar>(OnAddStar);
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnAddStar(BrokerEvent<StarEvents.AddStar> @event)
    {
		starsCollected += 1;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneName = scene.name;
    }

    private void OnLevelEnd(BrokerEvent<LevelEvents.EndLevel> @event)
    {
        if (@event.Payload.Victory)
        {
            eventBroker.Publish(this, new AudioEvents.PlayTemporaryMusic(Constants.Audio.Music.Victory));
        }

		int highestStars = PlayerPrefs.GetInt(currentSceneName, 0);

		if (starsCollected > highestStars)
		{
			PlayerPrefs.SetInt(currentSceneName, starsCollected);
			PlayerPrefs.Save();
		}
        StartCoroutine(HandleLevelEnd(@event));
    }

    private IEnumerator HandleLevelEnd(BrokerEvent<LevelEvents.EndLevel> @event)
    {
        yield return new WaitForSeconds(@event.Payload.TransitionDelay);
        if (@event.Payload.Victory)
        {
            SceneManager.LoadScene(@event.Payload.NextLevel);

        }
        else
        {
            SceneManager.LoadScene(currentSceneName);

        }
    }
}
