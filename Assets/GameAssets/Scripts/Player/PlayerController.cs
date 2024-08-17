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

	[SerializeField, Header("Stats")] private float movespeed;

	private Rigidbody2D rbody;
	private CircleCollider2D coll;

	private Collision2D currentWall;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    void Start()
    {
		rbody = GetComponent<Rigidbody2D>();
		coll = GetComponent<CircleCollider2D>();
    }

    void FixedUpdate()
    {
		//rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;

		if (currentWall == null)
		{
			rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
			rbody.gravityScale = 1f;
		}
		else if (Vector2.Dot(currentWall.transform.position - transform.position, transform.right) > 0)
		{
			rbody.gravityScale = 0f;

			if (move.ReadValue<float>() < 0)
			{
				// Climb down wall
				rbody.velocityY = Mathf.Abs(move.ReadValue<float>()) * movespeed * Time.deltaTime;
			}
			else if (move.ReadValue<float>() > 0)
			{
				// Climb up wall
				rbody.velocityY = Mathf.Abs(move.ReadValue<float>()) * -movespeed * Time.deltaTime;
			}
			else
			{
				// Stop
				rbody.velocityY = 0f;
			}
		}
		else if (Vector2.Dot(currentWall.transform.position - transform.position, transform.right) < 0)
		{
			rbody.gravityScale = 0f;

			if (move.ReadValue<float>() < 0)
			{
				// Climb up wall
				rbody.velocityY = Mathf.Abs(move.ReadValue<float>()) * -movespeed * Time.deltaTime;
			}
			else if (move.ReadValue<float>() > 0)
			{
				// Climb down wall
				rbody.velocityY = Mathf.Abs(move.ReadValue<float>()) * movespeed * Time.deltaTime;
			}
			else
			{
				// Stop
				rbody.velocityY = 0f;
			}
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		Debug.Log("Move: " + context.ReadValue<float>());
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

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Wall")
		{
			currentWall = collision;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "Wall")
		{
			currentWall = null;
		}
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
