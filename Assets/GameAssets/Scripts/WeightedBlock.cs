using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedBlock : MonoBehaviour, IWeighted, IButtonInteractable
{
    [SerializeField] private float blockWeight;

    public float GetWeight()
    {
        return blockWeight;
    }
}
