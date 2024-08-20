using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
	[SerializeField] private UnityEngine.UI.Button homeButton;
	[SerializeField] private UnityEngine.UI.Button soundButton;
	[SerializeField] private UnityEngine.UI.Button restartButton;
	[SerializeField] private Image uiBackground;
	[SerializeField] private Sprite soundOnImage;
	[SerializeField] private Sprite soundOffImage;

	[SerializeField, Header("Stars")] private int numStars;
	[SerializeField] private Transform starParent;
	[SerializeField] private GameObject starPrefab;
	[SerializeField] private Sprite starOn;
	[SerializeField] private Sprite starOff;
	private List<GameObject> spawnedStars;

	[SerializeField, Header("Ending")] private GameObject endPanel;
	[SerializeField] CircleWipe circleWipe;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private bool audioMuted;

	private Coroutine endingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
		// Check if music volume is set to 0
		audioMuted = PlayerPrefs.GetFloat(Constants.Audio.MusicVolumePP, Constants.Audio.DefaultMusicVolume) == 0f && PlayerPrefs.GetFloat(Constants.Audio.SFXVolumePP, Constants.Audio.DefaultSFXVolume) == 0f;
		if (audioMuted)
		{
			uiBackground.sprite = soundOffImage;
		}
		else
		{
			uiBackground.sprite = soundOnImage;
		}

		spawnedStars = new List<GameObject>();
		for (int i = 0; i < numStars; i++)
		{
			GameObject star = Instantiate(starPrefab, starParent);
			star.GetComponent<Image>().sprite = starOff;
			spawnedStars.Add(star);
		}
    }

	private void AddStarHandler(BrokerEvent<StarEvents.AddStar> inEvent)
	{
		// Find star that isn't turned on yet
		for (int i = 0; i < spawnedStars.Count; i++)
		{
			if (spawnedStars[i].GetComponent<Image>().sprite == starOff)
			{
				spawnedStars[i].GetComponent<Image>().sprite = starOn;
				return;
			}
		}
	}

	public void OnHomeButtonPressed()
	{
		if (endingCoroutine != null)
		{
			StopCoroutine(endingCoroutine);
		}

		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ButtonPress));
		SceneManager.LoadScene("MainMenu");
	}

	private void OnSoundButtonPressed()
	{
		if (audioMuted)
		{
			// Audio was muted, unmute
			eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ButtonPress));
			eventBroker.Publish(this, new AudioEvents.ChangeMusicVolume(Constants.Audio.DefaultMusicVolume));
			eventBroker.Publish(this, new AudioEvents.ChangeSFXVolume(Constants.Audio.DefaultSFXVolume));
			audioMuted = false;
			uiBackground.sprite = soundOnImage;
		}
		else
		{
			// Audio unmuted, mute
			eventBroker.Publish(this, new AudioEvents.ChangeMusicVolume(0f));
			eventBroker.Publish(this, new AudioEvents.ChangeSFXVolume(0f));
			audioMuted = true;
			uiBackground.sprite = soundOffImage;
		}
	}

	private void OnRestartButtonPressed()
	{
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ButtonPress));
		eventBroker.Publish(this, new LevelEvents.EndLevel("", false));
	}

	private void EndLevelHandler(BrokerEvent<LevelEvents.EndLevel> inEvent)
	{
		if (inEvent.Payload.NextLevel == "Ending")
		{
			StartCoroutine(EndingBeginCoroutine());
		}
	}

	private IEnumerator EndingBeginCoroutine()
	{
		yield return new WaitForSeconds(5f);
		circleWipe.targetAlpha = 1f;
		endPanel.SetActive(true);
		endingCoroutine = StartCoroutine(EndingCoroutine());
	}

	private IEnumerator EndingCoroutine()
	{
		yield return new WaitForSeconds(10f);
		OnHomeButtonPressed();
	}

	private void OnEnable()
	{
		homeButton.onClick.AddListener(OnHomeButtonPressed);
		soundButton.onClick.AddListener(OnSoundButtonPressed);
		restartButton.onClick.AddListener(OnRestartButtonPressed);

		eventBroker.Subscribe<StarEvents.AddStar>(AddStarHandler);
		eventBroker.Subscribe<LevelEvents.EndLevel>(EndLevelHandler);
	}

	private void OnDisable()
	{
		homeButton.onClick.RemoveListener(OnHomeButtonPressed);
		soundButton.onClick.RemoveListener(OnSoundButtonPressed);
		restartButton.onClick.RemoveListener(OnRestartButtonPressed);

		eventBroker.Unsubscribe<StarEvents.AddStar>(AddStarHandler);
		eventBroker.Unsubscribe<LevelEvents.EndLevel>(EndLevelHandler);
	}
}
