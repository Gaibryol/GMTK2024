using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField, Header("Credits")] private GameObject creditsPanel;

	[SerializeField, Header("Level Select")] private GameObject levelSelectPanel;
	[SerializeField] private UnityEngine.UI.Button level1Button;
	[SerializeField] private UnityEngine.UI.Button level2Button;
	[SerializeField] private UnityEngine.UI.Button level3Button;
	[SerializeField] private UnityEngine.UI.Button level4Button;
	[SerializeField] private UnityEngine.UI.Button level5Button;

	[SerializeField, Header("Powerpoint")] private GameObject powerpointPanel;
	[SerializeField] private Image ppScreen;
	[SerializeField] private List<Sprite> ppImages;

	[SerializeField, Header("Audio Toggle")] private GameObject audioToggle;
	[SerializeField] private Sprite audioOn;
	[SerializeField] private Sprite audioOff;

	private int ppIndex = 0;

	private Coroutine currentPPCoroutine;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private bool audioMuted;

	private void Start()
	{
		// Check if music volume is set to 0
		audioMuted = PlayerPrefs.GetFloat(Constants.Audio.MusicVolumePP, Constants.Audio.DefaultMusicVolume) == 0f && PlayerPrefs.GetFloat(Constants.Audio.SFXVolumePP, Constants.Audio.DefaultSFXVolume) == 0f;
		if (audioMuted)
		{
			audioToggle.GetComponent<Image>().sprite = audioOff;
		}
		else
		{
			audioToggle.GetComponent<Image>().sprite = audioOn;
		}
	}

	public void OnRunPowerPoint()
	{
		powerpointPanel.SetActive(true);
		ppScreen.sprite = ppImages[0];
		StartCoroutine(ppCoroutine());
	}

	public void NextPPImage()
	{
		if (currentPPCoroutine != null)
		{
			StopCoroutine(currentPPCoroutine);
		}

		if (ppIndex == ppImages.Count - 1)
		{
			eventBroker.Publish(this, new LevelEvents.EndLevel(Constants.Scenes.Level1, true));
			return;
		}

		ppIndex += 1;
		ppScreen.sprite = ppImages[ppIndex];
		currentPPCoroutine = StartCoroutine(ppCoroutine());
	}

	private IEnumerator ppCoroutine()
	{
		yield return new WaitForSeconds(3f);
		NextPPImage();
	}

	public void OnLevelPressed(int level)
	{
		if (PlayerPrefs.GetInt("HasWatchedPP", 0) == 0)
		{
			OnRunPowerPoint();
			PlayerPrefs.SetInt("HasWatchedPP", 1);
			PlayerPrefs.Save();
			return;
		}

		string levelString = "";
		switch (level)
		{
			case 1:
				levelString = Constants.Scenes.Level1;
				break;

			case 2:
				levelString = Constants.Scenes.Level2;
				break;

			case 3:
				levelString = Constants.Scenes.Level3;
				break;

			case 4:
				levelString = Constants.Scenes.Level4;
				break;

			case 5:
				levelString = Constants.Scenes.Level5;
				break;
		}
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ButtonPress));
		eventBroker.Publish(this, new LevelEvents.EndLevel(levelString, true));
	}

	public void OnSoundButtonPressed()
	{
		if (audioMuted)
		{
			// Audio was muted, unmute
			eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ButtonPress));
			eventBroker.Publish(this, new AudioEvents.ChangeMusicVolume(Constants.Audio.DefaultMusicVolume));
			eventBroker.Publish(this, new AudioEvents.ChangeSFXVolume(Constants.Audio.DefaultSFXVolume));
			audioMuted = false;
			audioToggle.GetComponent<Image>().sprite = audioOn;
		}
		else
		{
			// Audio unmuted, mute
			eventBroker.Publish(this, new AudioEvents.ChangeMusicVolume(0f));
			eventBroker.Publish(this, new AudioEvents.ChangeSFXVolume(0f));
			audioMuted = true;
			audioToggle.GetComponent<Image>().sprite = audioOff;
		}
	}

	public void OpenLevelSelectPanel()
	{
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ButtonPress));
		int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

		level1Button.gameObject.SetActive(levelReached >= 1);
		level2Button.gameObject.SetActive(levelReached >= 2);
		level3Button.gameObject.SetActive(levelReached >= 3);
		level4Button.gameObject.SetActive(levelReached >= 4);
		level5Button.gameObject.SetActive(levelReached >= 5);

		levelSelectPanel.SetActive(true);
	}

	public void CloseLevelSelectPanel()
	{
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ButtonPress));
		levelSelectPanel.SetActive(false);
	}

    public void OpenCreditsPanel()
	{
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ButtonPress));
		creditsPanel.SetActive(true);
	}

	public void CloseCreditsPanel()
	{
		eventBroker.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.ButtonPress));
		creditsPanel.SetActive(false);
	}
}
