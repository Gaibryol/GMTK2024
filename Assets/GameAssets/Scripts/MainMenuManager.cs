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

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	public void OnLevelPressed(int level)
	{
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
		eventBroker.Publish(this, new LevelEvents.EndLevel(levelString, true));
	}
	
	public void OpenLevelSelectPanel()
	{
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
		levelSelectPanel.SetActive(false);
	}

    public void OpenCreditsPanel()
	{
		creditsPanel.SetActive(true);
	}

	public void CloseCreditsPanel()
	{
		creditsPanel.SetActive(false);
	}
}
