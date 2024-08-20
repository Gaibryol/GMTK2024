using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioSystem : MonoBehaviour
{
	[SerializeField, Header("Sources")] private AudioSource musicSource;
	[SerializeField] private AudioSource sfxSource;

	[SerializeField, Header("Music")] private AudioClip mainMenu;
	[SerializeField] private AudioClip theme1;
	[SerializeField] private AudioClip theme2;
	[SerializeField] private AudioClip theme3;
	[SerializeField] private AudioClip theme4;
	[SerializeField] private AudioClip theme5;
	[SerializeField] private AudioClip victory;

    [SerializeField, Header("SFX")] private AudioClip buttonPress;
	[SerializeField] private AudioClip dandelion;
	[SerializeField] private AudioClip invalid;
	[SerializeField] private AudioClip mushroomJump;
	[SerializeField] private AudioClip platformDoorMove;
	[SerializeField] private AudioClip UIClick;
	[SerializeField] private AudioClip WormFall;
	[SerializeField] private AudioClip WormJoin;
	[SerializeField] private AudioClip WormSplit;
	[SerializeField] private AudioClip Star;

    private float musicVolume;
	private float sfxVolume;

	private AudioClip oldMusic;
	private float oldTime;

	private Dictionary<string, AudioClip> music = new Dictionary<string, AudioClip>();
	private Dictionary<string, AudioClip> sfx = new Dictionary<string, AudioClip>();

	private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

	private Coroutine playMusicFadeCoroutine;

	private void Awake()
	{
		// Set up music and sfx dictionaries
		music.Add(Constants.Audio.Music.MainMenuTheme, mainMenu);
		music.Add(Constants.Audio.Music.Level1Theme, theme1);
		music.Add(Constants.Audio.Music.Level2Theme, theme2);
		music.Add(Constants.Audio.Music.Level3Theme, theme3);
		music.Add(Constants.Audio.Music.Level4Theme, theme4);
		music.Add(Constants.Audio.Music.Level5Theme, theme5);
		music.Add(Constants.Audio.Music.Victory, victory);

        sfx.Add(Constants.Audio.SFX.ButtonPress, buttonPress);
		sfx.Add(Constants.Audio.SFX.Dandelion, dandelion);
		sfx.Add(Constants.Audio.SFX.Invalid, invalid);
		sfx.Add(Constants.Audio.SFX.MushroomJump, mushroomJump);
		sfx.Add(Constants.Audio.SFX.PlatformDoorMove, platformDoorMove);
		sfx.Add(Constants.Audio.SFX.UIClick, UIClick);
		sfx.Add(Constants.Audio.SFX.WormFall, WormFall);
		sfx.Add(Constants.Audio.SFX.WormJoin, WormJoin);
		sfx.Add(Constants.Audio.SFX.WormSplit, WormSplit);
		sfx.Add(Constants.Audio.SFX.Star, Star);
    }

	private void ChangeMusicVolumeHandler(BrokerEvent<AudioEvents.ChangeMusicVolume> inEvent)
	{
		musicVolume = inEvent.Payload.NewVolume;
		musicSource.volume = musicVolume;

		PlayerPrefs.SetFloat(Constants.Audio.MusicVolumePP, musicVolume);
	}

	private void ChangeSFXVolumeHandler(BrokerEvent<AudioEvents.ChangeSFXVolume> inEvent)
	{
		sfxVolume = inEvent.Payload.NewVolume;
		sfxSource.volume = sfxVolume;

		PlayerPrefs.SetFloat(Constants.Audio.SFXVolumePP, sfxVolume);
	}

	private void PlayMusicHandler(BrokerEvent<AudioEvents.PlayMusic> inEvent)
	{
		if (playMusicFadeCoroutine != null)
		{
			StopCoroutine(playMusicFadeCoroutine);
		}

		if (inEvent.Payload.Transition)
		{
			playMusicFadeCoroutine = StartCoroutine(FadeToSong(inEvent.Payload.MusicName));
		}
		else
		{
			PlayMusic(inEvent.Payload.MusicName);
		}
	}

	private void PlaySFXHandler(BrokerEvent<AudioEvents.PlaySFX> inEvent)
	{
		if (sfx.ContainsKey(inEvent.Payload.SFXName))
		{
			sfxSource.PlayOneShot(sfx[inEvent.Payload.SFXName]);
		}
		else
		{
			Debug.LogError("Cannot find sfx named " + inEvent.Payload.SFXName);
		}
	}

	private void PlayTemporaryMusicHandler(BrokerEvent<AudioEvents.PlayTemporaryMusic> inEvent)
	{
		oldMusic = musicSource.clip;
		oldTime = musicSource.time;
		StartCoroutine(FadeToSong(inEvent.Payload.MusicName));
	}

	private void StopTemporaryMusicHandler(BrokerEvent<AudioEvents.StopTemporaryMusic> inEvent)
	{
		StartCoroutine(FadeToSong(oldMusic, oldTime));
	}

	private void GetSongLengthHandler(BrokerEvent<AudioEvents.GetSongLength> inEvent)
	{
		if (music.ContainsKey(inEvent.Payload.Title))
		{
			inEvent.Payload.ProcessData.DynamicInvoke(music[inEvent.Payload.Title].length);
		}
		else
		{
			Debug.LogError("Cannot find music named " + inEvent.Payload.Title);
		}
	}

	private void StopMusicHandler(BrokerEvent<AudioEvents.StopMusic> inEvent)
	{
		musicSource.Stop();
	}

	private void PlayMusic(string song, float time = 0f)
	{
		if (music.ContainsKey(song))
		{
			musicSource.Stop();
			musicSource.clip = music[song];
			musicSource.loop = true;
			musicSource.Play();
			musicSource.time = time;
		}
		else
		{
			Debug.LogError("Cannot find music named " + song);
		}
	}

	private void PlayMusic(AudioClip song, float time = 0f)
	{
		musicSource.Stop();
		musicSource.clip = song;
		musicSource.loop = true;
		musicSource.Play();
		musicSource.time = time;
	}

	private IEnumerator FadeToSong(string song, float time = 0f)
	{
		while (musicSource.volume > 0)
		{
			musicSource.volume -= Constants.Audio.MusicFadeSpeed * Time.deltaTime;
			yield return null;
		}

		PlayMusic(song, time);

		while (musicSource.volume < musicVolume)
		{
			musicSource.volume += Constants.Audio.MusicFadeSpeed * Time.deltaTime;
			yield return null;
		}
	}

	private IEnumerator FadeToSong(AudioClip song, float time = 0f)
	{
		while (musicSource.volume > 0)
		{
			musicSource.volume -= Constants.Audio.MusicFadeSpeed * Time.deltaTime;
			yield return null;
		}

		PlayMusic(song, time);

		while (musicSource.volume < musicVolume)
		{
			musicSource.volume += Constants.Audio.MusicFadeSpeed * Time.deltaTime;
			yield return null;
		}
	}

	private void OnEnable()
	{
		eventBrokerComponent.Subscribe<AudioEvents.PlayMusic>(PlayMusicHandler);
		eventBrokerComponent.Subscribe<AudioEvents.PlaySFX>(PlaySFXHandler);
		eventBrokerComponent.Subscribe<AudioEvents.ChangeMusicVolume>(ChangeMusicVolumeHandler);
		eventBrokerComponent.Subscribe<AudioEvents.ChangeSFXVolume>(ChangeSFXVolumeHandler);
		eventBrokerComponent.Subscribe<AudioEvents.PlayTemporaryMusic>(PlayTemporaryMusicHandler);
		eventBrokerComponent.Subscribe<AudioEvents.StopTemporaryMusic>(StopTemporaryMusicHandler);
		eventBrokerComponent.Subscribe<AudioEvents.GetSongLength>(GetSongLengthHandler);
		eventBrokerComponent.Subscribe<AudioEvents.StopMusic>(StopMusicHandler);

		float musicLevel = PlayerPrefs.GetFloat(Constants.Audio.MusicVolumePP, Constants.Audio.DefaultMusicVolume);
		float sfxLevel = PlayerPrefs.GetFloat(Constants.Audio.SFXVolumePP, Constants.Audio.DefaultSFXVolume);

		musicVolume = musicLevel;
		sfxVolume = sfxLevel;
		musicSource.volume = musicLevel;
		sfxSource.volume = sfxLevel;
	}

	private void OnDisable()
	{
		eventBrokerComponent.Unsubscribe<AudioEvents.PlayMusic>(PlayMusicHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.PlaySFX>(PlaySFXHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.ChangeMusicVolume>(ChangeMusicVolumeHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.ChangeSFXVolume>(ChangeSFXVolumeHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.PlayTemporaryMusic>(PlayTemporaryMusicHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.StopTemporaryMusic>(StopTemporaryMusicHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.GetSongLength>(GetSongLengthHandler);
		eventBrokerComponent.Unsubscribe<AudioEvents.StopMusic>(StopMusicHandler);
	}
}