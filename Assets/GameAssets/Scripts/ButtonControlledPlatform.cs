using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControlledPlatform : MonoBehaviour, IButtonInteractableListener
{
    [SerializeField] private Vector2 positionOffset;
    [SerializeField] private float moveSpeed;

    private Vector2 startingPosition;

    private bool active;
    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

    private void Start()
    {
        startingPosition = transform.position;    
    }

    private void Update()
    {
        Vector2 target = startingPosition;
        if (active)
        {
            target += positionOffset;
        }
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    public void OnButtonPressed()
    {
        active = true;
        Vector2 target = startingPosition;
        if (active)
        {
            target += positionOffset;
        }
        if (Vector2.Distance(transform.position, target) > .1f)
        {
            eventBrokerComponent.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.PlatformDoorMove));
        }

    }

    public void OnButtonReleased()
    {
        active = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IButtonInteractable buttonInteractable = collision.GetComponent<IButtonInteractable>();
        if (buttonInteractable == null) { return; }

        collision.transform.SetParent(transform, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IButtonInteractable buttonInteractable = collision.GetComponent<IButtonInteractable>();
        if (buttonInteractable == null) { return; }

        collision.transform.SetParent(null, true);

    }
}
