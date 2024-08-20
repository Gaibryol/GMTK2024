using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobScript : MonoBehaviour, IBounceable, IButtonInteractable, IGrassBendable, IWeighted
{
	
	public float forceMultiplier { get { return 0.5f; } }

    public float GetWeight()
    {
		return 1;
    }
}
