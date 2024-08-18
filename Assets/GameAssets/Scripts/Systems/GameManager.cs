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
        if (!PlayerPrefs.HasKey(currentSceneName))
        {
            PlayerPrefs.SetInt(currentSceneName, 0);
        }
        PlayerPrefs.SetInt(currentSceneName, PlayerPrefs.GetInt(currentSceneName) + 1);
        PlayerPrefs.Save();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerPrefs.SetInt(currentSceneName, 0);

        currentSceneName = scene.name;
    }


    private void OnLevelEnd(BrokerEvent<LevelEvents.EndLevel> @event)
    {
        SceneManager.LoadScene(@event.Payload.NextLevel);
    }
}
