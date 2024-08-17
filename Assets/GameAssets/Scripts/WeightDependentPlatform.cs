using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightDependentPlatform : MonoBehaviour
{
    [SerializeField] private float maxWeight;
    private List<IWeighted> weighteds = new List<IWeighted>();

    private bool broken = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (broken) return;
        if (WeightExceeded())
        {
            DestroyPlatform();
        }
    }

    private void DestroyPlatform()
    {
        Destroy(gameObject);
    }

    private bool WeightExceeded()
    {
        float total = 0;
        foreach (IWeighted weighted in weighteds)
        {
            total += weighted.GetWeight();
        }
        return total >= maxWeight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IWeighted weighted = collision.gameObject.GetComponent<IWeighted>();
        if (weighted == null) return;
        weighteds.Add(weighted);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IWeighted weighted = collision.gameObject.GetComponent<IWeighted>();
        if (weighted == null) return;
        weighteds.Remove(weighted);
    }
}
