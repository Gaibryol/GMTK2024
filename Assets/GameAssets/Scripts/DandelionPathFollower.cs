using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionPathFollower : MonoBehaviour
{
    [SerializeField] private Dandylion dandylion;
    [SerializeField] private bool stopOnWall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!stopOnWall) return;
        dandylion.StopInteract();
    }
}
