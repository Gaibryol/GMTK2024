using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobScript : MonoBehaviour, IInteractable
{
	public bool Interact(GameObject source)
	{
		source.GetComponent<PlayerController>()?.PickupBlob(gameObject);
		return true;
	}

	public bool StopInteract()
	{
		return true;
	}
}
