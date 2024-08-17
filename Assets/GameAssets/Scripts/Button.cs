using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Triggers a button press when an object enters it's trigger.
* The object must implement IButtonInteractable for it to interact with the button
* The responding object implementing IButtonInteractableListener will be notified
*/
public class Button : MonoBehaviour
{
    [SerializeField] private GameObject buttonListener;
    private IButtonInteractableListener interactableListener;

    private bool pressed;

    void Start()
    {
        interactableListener = buttonListener.GetComponent<IButtonInteractableListener>();
        if (interactableListener == null) 
        {
            Debug.LogError("Button listner is not implementing IButtonInteractableListener", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (pressed) { return; }

        IButtonInteractable buttonInteractable = collision.GetComponent<IButtonInteractable>();

        if (buttonInteractable == null) { return; }
        Debug.Log("here");

        pressed = true;
        interactableListener.OnButtonPressed();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!pressed) { return; }

        IButtonInteractable buttonInteractable = collision.GetComponent<IButtonInteractable>();

        if (buttonInteractable == null) { return; }

        pressed = false;
        interactableListener.OnButtonReleased();
    }
}
