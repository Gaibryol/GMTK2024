using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Objects implementing this can perform an interaction when the player presses the interact button
 */
public interface IInteractable
{
    public bool Interact(GameObject source);

    public bool StopInteract();
}
