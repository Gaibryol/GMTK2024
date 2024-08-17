using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField, Header("Inputs")] private InputAction move;
	[SerializeField] private InputAction drop;
	[SerializeField] private InputAction split;
	[SerializeField] private InputAction interact;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnMove(InputAction.CallbackContext context)
	{
		Debug.Log("Move");
	}

	public void OnDrop(InputAction.CallbackContext context)
	{
		Debug.Log("Drop");
	}

	public void OnSplit(InputAction.CallbackContext context)
	{
		Debug.Log("Split");
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		Debug.Log("Interact");
	}

	private void OnEnable()
	{
		move.performed += OnMove;
		drop.performed += OnDrop;
		split.performed += OnSplit;
		interact.performed += OnInteract;

		move.Enable();
		drop.Enable();
		split.Enable();
		interact.Enable();
	}

	private void OnDisable()
	{
		move.performed -= OnMove;
		drop.performed -= OnDrop;
		split.performed -= OnSplit;
		interact.performed -= OnInteract;

		move.Disable();
		drop.Disable();
		split.Disable();
		interact.Disable();
	}
}
