using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
	[SerializeField] private UnityEngine.UI.Button homeButton;
	[SerializeField] private UnityEngine.UI.Button soundButton;
	[SerializeField] private UnityEngine.UI.Button restartButton;
	[SerializeField] private Image uiBackground;
	[SerializeField] private Sprite soundOnImage;
	[SerializeField] private Sprite soundOffImage;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private bool audioMuted;

    // Start is called before the first frame update
    void Start()
    {
		// Check if music volume is set to 0
		audioMuted = PlayerPrefs.GetFloat(Constants.Audio.MusicVolumePP, Constants.Audio.DefaultMusicVolume) == 0f && PlayerPrefs.GetFloat(Constants.Audio.SFXVolumePP, Constants.Audio.DefaultSFXVolume) == 0f;
    }

	private void OnHomeButtonPressed()
	{
		SceneManager.LoadScene("MainMenu");
	}

	private void OnSoundButtonPressed()
	{
		if (audioMuted)
		{
			// Audio was muted, unmute
			eventBroker.Publish(this, new AudioEvents.ChangeMusicVolume(PlayerPrefs.GetFloat(Constants.Audio.MusicVolumePP)));
			eventBroker.Publish(this, new AudioEvents.ChangeSFXVolume(PlayerPrefs.GetFloat(Constants.Audio.MusicVolumePP)));
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
		eventBroker.Publish(this, new LevelEvents.EndLevel("", false));
	}

	private void OnEnable()
	{
		homeButton.onClick.AddListener(OnHomeButtonPressed);
		soundButton.onClick.AddListener(OnSoundButtonPressed);
		restartButton.onClick.AddListener(OnRestartButtonPressed);
	}

	private void OnDisable()
	{
		homeButton.onClick.RemoveListener(OnHomeButtonPressed);
		soundButton.onClick.RemoveListener(OnSoundButtonPressed);
		restartButton.onClick.RemoveListener(OnRestartButtonPressed);
	}
}
