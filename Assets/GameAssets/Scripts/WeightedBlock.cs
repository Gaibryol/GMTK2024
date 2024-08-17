using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedBlock : MonoBehaviour, IWeighted, IButtonInteractable, IInteractable, IGrassBendable
{
    [SerializeField] private float blockWeight;

    public float GetWeight()
    {
        return blockWeight;
    }

    public bool Interact(GameObject source)
    {
        throw new System.NotImplementedException();
    }

    public bool StopInteract()
    {
        throw new System.NotImplementedException();
    }
}
