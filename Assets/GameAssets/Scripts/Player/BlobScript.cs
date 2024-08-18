using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobScript : MonoBehaviour, IInteractable, IBounceable, IButtonInteractable, IGrassBendable, IWeighted
{
	
	public float forceMultiplier { get { return 0.5f; } }

    public float GetWeight()
    {
		return 1;
    }

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
