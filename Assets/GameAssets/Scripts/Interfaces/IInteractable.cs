using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool Interact(GameObject source);

    public bool StopInteract();
}
