using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonControlledPlatform : MonoBehaviour, IButtonInteractableListener
{
    [SerializeField] private Vector2 positionOffset;
    [SerializeField] private float moveSpeed;

    private Vector2 startingPosition;

    private bool active;

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
        Debug.Log("pressed");
    }

    public void OnButtonReleased()
    {
        active = false;
        Debug.Log("relessed");


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
