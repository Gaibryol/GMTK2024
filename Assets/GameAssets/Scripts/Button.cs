using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Triggers a button press when an object enters it's trigger.
* The object must implement IButtonInteractable for it to interact with the button
* The responding object implementing IButtonInteractableListener will be notified
*/
[RequireComponent(typeof(Collider2D))]
public class Button : MonoBehaviour
{
    [SerializeField] private GameObject buttonListener;
    private IButtonInteractableListener interactableListener;

    private bool pressed;
    private List<IButtonInteractable> pressedObjects = new List<IButtonInteractable>();

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private Sprite onSprite;

    void Start()
    {
        interactableListener = buttonListener.GetComponent<IButtonInteractableListener>();
        if (interactableListener == null) 
        {
            Debug.LogError("Button listner is not implementing IButtonInteractableListener", this);
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IButtonInteractable buttonInteractable = collision.GetComponent<IButtonInteractable>();

        if (buttonInteractable == null) { return; }
        
        pressedObjects.Add(buttonInteractable);

        pressed = pressedObjects.Count > 0;
        if (pressed)
        {
            interactableListener.OnButtonPressed();
            spriteRenderer.sprite = onSprite;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IButtonInteractable buttonInteractable = collision.GetComponent<IButtonInteractable>();

        if (buttonInteractable == null) { return; }

        pressedObjects.Remove(buttonInteractable);
        pressed = pressedObjects.Count > 0;
        
        if (!pressed)
        {
            interactableListener.OnButtonReleased();
            spriteRenderer.sprite = offSprite;
        }
    }
}
