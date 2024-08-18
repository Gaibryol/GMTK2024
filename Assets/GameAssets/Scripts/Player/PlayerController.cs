using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class PlayerController : MonoBehaviour, IBounceable, IButtonInteractable, IGrassBendable, IDandylion, IWeighted
{
	[SerializeField, Header("Inputs")] private InputAction move;
	[SerializeField] private InputAction drop;
	[SerializeField] private InputAction split;
	[SerializeField] private InputAction interact;

	[SerializeField] private List<Vector3> boneStartEndRotation3;
    [SerializeField, Range(-1, 1)] private float boneBlend = 0;

	[SerializeField, Header("Stats")] private float movespeed;

	[SerializeField, Header("Splitting")] private GameObject blobPrefab;

	private Rigidbody2D rbody;
	private Collider2D coll;
	private Constants.Player.Inputs lastInput;

	private bool dropping;

	private bool canMove;
	private IInteractable interactable;

	[SerializeField] private SpriteSkin skin;
	private Transform[] boneTransforms;

	private bool facingRight;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

    void Start()
    {
		rbody = GetComponent<Rigidbody2D>();
		coll = GetComponent<Collider2D>();
		lastInput = Constants.Player.Inputs.None;
		dropping = false;
		canMove = true;
		facingRight = true;

		boneTransforms = skin.boneTransforms;
    }

    private void LateUpdate()
    {
        for (int i = 0; i < boneTransforms.Length; i++)
        {
            Transform boneTransform = boneTransforms[i];
			float zRot = boneStartEndRotation3[i].x;

            if (boneBlend >= 0)
			{
                zRot = Mathf.Lerp(boneStartEndRotation3[i].x, boneStartEndRotation3[i].y, boneBlend);
            } else
			{
                zRot = Mathf.Lerp(boneStartEndRotation3[i].x, boneStartEndRotation3[i].z, boneBlend * -1f);
            }
            skin.boneTransforms[i].localEulerAngles = new Vector3(boneTransform.eulerAngles.x, boneTransform.eulerAngles.y, zRot);
        }
    }

    void FixedUpdate()
    {
		if (!canMove)
		{
			return;
		}

		if (dropping)
		{
			Debug.Log("dropping");
			rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
			return;
		}

		HandleBones();

		Transform headBone = boneTransforms[boneTransforms.Length - 1];
		RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, Constants.Player.RaycastDistance.x, 1 << LayerMask.NameToLayer("Wall"));
		RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, Constants.Player.RaycastDistance.x, 1 << LayerMask.NameToLayer("Wall"));
		//RaycastHit2D topHit = Physics2D.Raycast(transform.position, Vector2.up, Constants.Player.RaycastDistance, 1 << LayerMask.NameToLayer("Wall"));
		RaycastHit2D bottomHit = Physics2D.Raycast(transform.position, Vector2.down, Constants.Player.RaycastDistance.y, 1 << LayerMask.NameToLayer("Wall"));
		RaycastHit2D bottomRightHit = Physics2D.Raycast(transform.position, new Vector2(1f, -1f), Constants.Player.DiagonalRaycastDistance, 1 << LayerMask.NameToLayer("Wall"));
		RaycastHit2D bottomLeftHit = Physics2D.Raycast(transform.position, new Vector2(-1f, -1f), Constants.Player.DiagonalRaycastDistance, 1 << LayerMask.NameToLayer("Wall"));
		//RaycastHit2D topRightHit = Physics2D.Raycast(transform.position, new Vector2(1f, 1f), Constants.Player.DiagonalRaycastDistance, 1 << LayerMask.NameToLayer("Wall"));
		//RaycastHit2D topLeftHit = Physics2D.Raycast(transform.position, new Vector2(-1f, 1f), Constants.Player.DiagonalRaycastDistance, 1 << LayerMask.NameToLayer("Wall"));

		//Debug.Log("Right: " + rightHit.collider);
		//Debug.Log("Left: " + leftHit.collider);
		//Debug.Log("Top: " + topHit.collider);
		//Debug.Log("Bottom: " + bottomHit.collider);
		//Debug.Log("Bottom Right: " + bottomRightHit.collider);
		//Debug.Log("Bottom Left: " + bottomLeftHit.collider);

		if (rightHit.collider != null && leftHit.collider == null && bottomHit.collider == null)
		{
			// Wall on right
			rbody.velocityX = 0f;
			rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
			rbody.gravityScale = 0f;
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
		}
		else if (rightHit.collider == null && leftHit.collider != null && bottomHit.collider == null)
		{
			// Wall on left
			rbody.velocityX = 0f;
			rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
			rbody.gravityScale = 0f;
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
		}
		//else if (rightHit.collider == null && leftHit.collider == null && topHit.collider == null && bottomHit.collider == null)
		//{
		//	// Wall on top
		//	rbody.velocityX = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//	rbody.gravityScale = 0f;
		//}
		else if (rightHit.collider == null && leftHit.collider == null && bottomHit.collider != null)
		{
			// Wall on bottom
			rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
			rbody.gravityScale = 1f;
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		}
		else if (rightHit.collider != null && bottomHit.collider != null)
		{
			// Wall on right and bottom
			if (lastInput == Constants.Player.Inputs.A)
			{
				// Was coming down wall, move off wall
				rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
				rbody.gravityScale = 1f;
				transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			}
			else if (lastInput == Constants.Player.Inputs.D)
			{
				// Was moving towards wall, go up wall
				rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
				rbody.gravityScale = 0f;
				transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
			}
		}
		else if (leftHit.collider != null && bottomHit.collider != null)
		{
			// Wall on left and bottom
			if (lastInput == Constants.Player.Inputs.A)
			{
				// Was moving towards wall, go up wall
				rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
				rbody.gravityScale = 0f;
				transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
			}
			else if (lastInput == Constants.Player.Inputs.D)
			{
				// Was coming down wall, move off wall
				rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
				rbody.gravityScale = 1f;
				transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
			}
		}
		//else if (leftHit.collider != null && topHit.collider != null)
		//{
		//	// Wall on left and top
		//	if (lastInput == Constants.Player.Inputs.A)
		//	{
		//		// Was moving up wall, go to ceiling
		//		rbody.velocityX = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//		rbody.gravityScale = 0f;
		//	}
		//	else if (lastInput == Constants.Player.Inputs.D)
		//	{
		//		// Move down the wall
		//		rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//		rbody.gravityScale = 0f;
		//	}
		//}
		//else if (rightHit.collider != null && topHit.collider != null)
		//{
		//	// Wall on right and top
		//	if (lastInput == Constants.Player.Inputs.A)
		//	{
		//		// Mode down the wall
		//		rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
		//		rbody.gravityScale = 0f;
		//	}
		//	else if (lastInput == Constants.Player.Inputs.D)
		//	{
		//		// Was moving up wall, go to ceiling
		//		rbody.velocityX = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//		rbody.gravityScale = 0f;

		//	}
		//}
		else if (rightHit.collider == null && bottomHit.collider == null && bottomRightHit.collider != null)
		{
			if (Mathf.Abs(bottomRightHit.normal.x) == 1f)
			{
				// Moving on vertical wall
				if (lastInput == Constants.Player.Inputs.A)
				{
					// Going over edge towards the left
					rbody.velocityX = 0;
					rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
				}
				else if (lastInput == Constants.Player.Inputs.D)
				{
					// Going over corner towards the right
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				}
			}
			else if (Mathf.Abs(bottomRightHit.normal.y) == 1f)
			{
				if (lastInput == Constants.Player.Inputs.A)
				{
					// Still moving towards the left
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
				}
				else if (lastInput == Constants.Player.Inputs.D)
				{
					// Going over corner towards the right
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
				}
			}
			else
			{
				if (move.ReadValue<float>() < 0f)
				{
					// Moving down a ramp on the left
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -Mathf.Atan(bottomRightHit.normal.x / bottomRightHit.normal.y) * 180f / Mathf.PI));
				}
				else if (move.ReadValue<float>() > 0f)
				{
					// Moving up a ramp on the right
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -Mathf.Atan(bottomRightHit.normal.x / bottomRightHit.normal.y) * 180f / Mathf.PI));
				}
				else
				{
					// Not moving on ramp
					rbody.velocity = Vector2.zero;
					rbody.gravityScale = 0f;
				}
			}
		}
		else if (leftHit.collider == null && bottomHit.collider == null && bottomLeftHit.collider != null)
		{
			if (Mathf.Abs(bottomLeftHit.normal.x) == 1f)
			{
				// Moving on vertical wall
				if (lastInput == Constants.Player.Inputs.A)
				{
					// Going over corner towards the left
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				}
				else if (lastInput == Constants.Player.Inputs.D)
				{
					// Going over edge towards the left
					rbody.velocityX = 0;
					rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
				}
			}
			else if (Mathf.Abs(bottomLeftHit.normal.y) == 1f)
			{
				if (lastInput == Constants.Player.Inputs.A)
				{
					// Going over corner towards the left
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;

					
				}
				else if (lastInput == Constants.Player.Inputs.D)
				{
					// Still moving towards the right
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
				}
			}
			else
			{
				if (move.ReadValue<float>() < 0f)
				{
					// Moving up a ramp on the left
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -Mathf.Atan(bottomLeftHit.normal.x / bottomLeftHit.normal.y) * 180f / Mathf.PI));

				}
				else if (move.ReadValue<float>() > 0f)
				{
					// Moving down a ramp on the right
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.velocityY = -Mathf.Abs(move.ReadValue<float>()) * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -Mathf.Atan(bottomLeftHit.normal.x / bottomLeftHit.normal.y) * 180f / Mathf.PI));
				}
				else
				{
					// Not moving on ramp
					rbody.velocity = Vector2.zero;
					rbody.gravityScale = 0f;
				}
			}
		}
		//else if (leftHit.collider == null && topHit.collider == null && topLeftHit.collider != null)
		//{
		//	if (lastInput == Constants.Player.Inputs.A)
		//	{
		//		// Go around the corner
		//		rbody.velocityX = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//		rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//		rbody.gravityScale = 0f;
		//	}
		//	else if (lastInput == Constants.Player.Inputs.D)
		//	{
		//		// Going away from corner
		//		rbody.velocityX = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//		rbody.gravityScale = 0f;
		//	}
		//}
		//else if (rightHit.collider == null && topHit.collider == null && topRightHit.collider != null)
		//{
		//	if (Mathf.Abs(topRightHit.normal.x) == 1f)
		//	{
		//		// Moving on vertical wall
		//		if (lastInput == Constants.Player.Inputs.A)
		//		{
		//			// Going away from corner
		//			rbody.velocityX = 0f;
		//			rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//			rbody.gravityScale = 0f;
		//		}
		//		else if (lastInput == Constants.Player.Inputs.D)
		//		{
		//			// Going over edge towards up
		//			rbody.velocityX = 0;
		//			rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
		//			rbody.gravityScale = 0f;
		//		}
		//	}
		//	else if (Mathf.Abs(topRightHit.normal.y) == 1f)
		//	{
		//		// Moving on horizontal wall
		//		if (lastInput == Constants.Player.Inputs.A)
		//		{
		//			// Going away from corner
		//			rbody.velocityX = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//			rbody.gravityScale = 0f;
		//		}
		//		else if (lastInput == Constants.Player.Inputs.D)
		//		{
		//			// Go around the corner
		//			rbody.velocityX = -move.ReadValue<float>() * movespeed * Time.deltaTime;
		//			rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
		//			rbody.gravityScale = 0f;
		//		}
		//	}
		//	else
		//	{
		//		if (move.ReadValue<float>() < 0f)
		//		{
		//			// Moving up a ramp on the right
		//			rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
		//			rbody.velocityY = -Mathf.Abs(move.ReadValue<float>()) * movespeed * Time.deltaTime;
		//			rbody.gravityScale = 0f;

		//		}
		//		else if (move.ReadValue<float>() > 0f)
		//		{
		//			// Moving down a ramp on the right
		//			rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
		//			rbody.gravityScale = 0f;
		//		}
		//		else
		//		{
		//			// Not moving on ramp
		//			rbody.velocity = Vector2.zero;
		//			rbody.gravityScale = 0f;
		//		}
		//	}
		//}
		else if (rightHit.collider == null && leftHit.collider == null && bottomHit.collider == null && bottomRightHit.collider == null && bottomLeftHit.collider == null)
		{
			// Freefall, allow for air strafe
			rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
			rbody.gravityScale = 1f;
			transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		}
	}

	private void HandleBones()
	{
		RaycastHit2D hit = Physics2D.Raycast(boneTransforms[boneTransforms.Length - 1].position, new Vector2(1, 0), Constants.Player.RaycastDistance.x, 1 << LayerMask.NameToLayer("Wall"));
		Debug.DrawRay(boneTransforms[boneTransforms.Length - 1].position, Vector2.right, Color.green, 20f);
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		if (context.ReadValue<float>() > 0)
		{
			lastInput = Constants.Player.Inputs.D;

			if (facingRight)
			{
				facingRight = false;
				transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		else if (context.ReadValue<float>() < 0)
		{
			lastInput = Constants.Player.Inputs.A;

			if (!facingRight)
			{
				facingRight = true;
				transform.localScale = new Vector3(-1f, 1f, 1f);
			}
		}
	}

	public void OnDrop(InputAction.CallbackContext context)
	{
		Debug.Log("Drop");

		if (interactable != null)
		{
			interactable.StopInteract();
		}

		StartCoroutine(DropCoroutine());
	}

	private IEnumerator DropCoroutine()
	{
		rbody.velocity = Vector2.zero;
		rbody.gravityScale = 1f;
		transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		dropping = true;

		yield return new WaitForSeconds(Constants.Player.DropDuration);

		dropping = false;
	}

	public void OnSplit(InputAction.CallbackContext context)
	{
		Debug.Log("Split");
		if (rbody.mass > 1f)
		{
			Instantiate(blobPrefab, transform.position - new Vector3(1f, 0), Quaternion.identity);
			rbody.mass = 1f;

			GameObject[] longObjects = GameObject.FindGameObjectsWithTag("Long");
			foreach (GameObject obj in longObjects)
			{
				obj.GetComponent<Collider2D>().enabled = false;
			}

			GameObject[] shortObjects = GameObject.FindGameObjectsWithTag("Short");
			foreach (GameObject obj in shortObjects)
			{
				obj.GetComponent<Collider2D>().enabled = true;
			}
		}
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		Debug.Log("Interact");
		List<Collider2D> colliders = new List<Collider2D>();
		Physics2D.OverlapCollider(coll, colliders);
		foreach (Collider2D collider in colliders)
		{
			Debug.Log(collider.gameObject.name);
			IInteractable interactable = collider.GetComponent<IInteractable>();
			if (interactable != null)
			{
				if (interactable.Interact(gameObject))
				{
					this.interactable = interactable;
					return;
				}
			}
		}

	}

	public void PickupBlob(GameObject blob)
	{
		rbody.mass = 2f;
		Destroy(blob);

		GameObject[] longObjects = GameObject.FindGameObjectsWithTag("Long");
		foreach (GameObject obj in longObjects)
		{
			obj.GetComponent<Collider2D>().enabled = true;
		}

		GameObject[] shortObjects = GameObject.FindGameObjectsWithTag("Short");
		foreach (GameObject obj in shortObjects)
		{
			obj.GetComponent<Collider2D>().enabled = false;
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

	public bool CanBendGrass()
	{
		return rbody.mass > 1;
	}

	public bool IsHeavyPath()
	{
		return rbody.mass > 1;
	}

	public void OnAttached(GameObject dandylion)
	{
		canMove = false;
		transform.parent = dandylion.transform;
		rbody.velocity = Vector3.zero;
		rbody.gravityScale = 0;
	}

	public void OnDetached(GameObject dandylion)
	{
		canMove = true;
		interactable = null;
		transform.parent = null;
		rbody.gravityScale = 1;
	}

	public float GetWeight()
	{
		return rbody.mass;
	}
}
