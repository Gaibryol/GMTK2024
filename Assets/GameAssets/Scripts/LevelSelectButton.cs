using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelectButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData eventData)
	{
		transform.localScale = new Vector3(1.25f, 1.25f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		transform.localScale = new Vector3(1f, 1f);
	}
}
