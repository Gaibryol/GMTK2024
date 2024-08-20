using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    [SerializeField, Range(-1f, 1f)] private float boneBlend = 0;
    [SerializeField] private List<Vector3> boneTailStartEndRotation3;
    [SerializeField, Range(-1f, 1f)] private float boneTailBlend = 0;

    [SerializeField, Header("Stats")] private float movespeed;

	[SerializeField, Header("Splitting")] private GameObject blobPrefab;

	private Rigidbody2D rbody;
	private Collider2D coll;
	private Animator anim;
	private Constants.Player.Inputs lastInput;

	private bool dropping;

	private bool canMove;
	private IInteractable interactable;

	[SerializeField] private SpriteSkin skin;
	private Transform[] boneTransforms;

	private Constants.Player.Orientations orientation;

	private readonly EventBrokerComponent eventBroker = new EventBrokerComponent();

	private List<Transform> groundedColliders;

    public float forceMultiplier { get { return rbody.mass == 1 ? 0.5f : 1f; } }

	private void Awake()
	{
		groundedColliders = new List<Transform>();
	}

	void Start()
    {
		rbody = GetComponent<Rigidbody2D>();
		coll = GetComponent<Collider2D>();
		anim = GetComponent<Animator>();
		lastInput = Constants.Player.Inputs.None;
		dropping = false;
		canMove = true;
		orientation = Constants.Player.Orientations.Flat;

		boneTransforms = skin.boneTransforms;
		//Physics2D.queriesHitTriggers = false
    }

    private void LateUpdate()
    {
        for (int i = 0; i < 7; i++)
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

        for (int i = 7; i < boneTransforms.Length; i++)
        {
            Transform boneTransform = boneTransforms[i];
			int inex = i - 7;
            float zRot = boneTailStartEndRotation3[inex].x;

            if (boneTailBlend >= 0)
            {
                zRot = Mathf.Lerp(boneTailStartEndRotation3[inex].x, boneTailStartEndRotation3[inex].y, boneTailBlend);
            }
            else
            {
                zRot = Mathf.Lerp(boneTailStartEndRotation3[inex].x, boneTailStartEndRotation3[inex].z, boneTailBlend * -1f);
            }
            skin.boneTransforms[i].localEulerAngles = new Vector3(boneTransform.eulerAngles.x, boneTransform.eulerAngles.y, zRot);
        }
	}

	private void Update()
	{
		HandleScale();
		HandleBones();
		HandleAnimations();
	}

	void FixedUpdate()
    {
		if (!canMove)
		{
			return;
		}

		if (dropping)
		{
			rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
			return;
		}
		ContactFilter2D contactFilter = new ContactFilter2D();
		contactFilter.useTriggers = false;
		contactFilter.useLayerMask = true;
		contactFilter.layerMask = 1 << LayerMask.NameToLayer("Wall");


		RaycastHit2D[] hitResults = new RaycastHit2D[1];
		int rightHitCount = Physics2D.Raycast(transform.position, Vector2.right, contactFilter, hitResults, Constants.Player.RaycastDistance.x);
		RaycastHit2D rightHit = rightHitCount == 0 ? new RaycastHit2D() : hitResults[0];

        int leftHitCount = Physics2D.Raycast(transform.position, Vector2.left, contactFilter, hitResults, Constants.Player.RaycastDistance.x);
        RaycastHit2D leftHit = leftHitCount == 0 ? new RaycastHit2D() : hitResults[0];


        int bottomHitCount = Physics2D.Raycast(transform.position, Vector2.down, contactFilter, hitResults, Constants.Player.RaycastDistance.y);
        RaycastHit2D bottomHit = bottomHitCount == 0 ? new RaycastHit2D() : hitResults[0];


        int bottomRightHitCount = Physics2D.Raycast(transform.position, new Vector2(1f, -1f), contactFilter, hitResults, Constants.Player.DiagonalRaycastDistance);
        RaycastHit2D bottomRightHit = bottomRightHitCount == 0 ? new RaycastHit2D() : hitResults[0];


        int bottomLeftHitCount = Physics2D.Raycast(transform.position, new Vector2(-1f, -1f), contactFilter, hitResults, Constants.Player.DiagonalRaycastDistance);
        RaycastHit2D bottomLeftHit = bottomLeftHitCount == 0 ? new RaycastHit2D() : hitResults[0];

        if (rightHit.collider != null)
		{
			if (rightHit.collider.isTrigger == true)
			{
				rightHit = new RaycastHit2D();
			}
		}
		if (leftHit.collider != null)
		{
			if (leftHit.collider.isTrigger == true)
			{
				leftHit = new RaycastHit2D();
			}
		}
		if (bottomHit.collider != null)
		{
			if (bottomHit.collider.isTrigger == true)
			{
				bottomHit = new RaycastHit2D();
			}
		}
		if (bottomRightHit.collider != null)
		{
			if (bottomRightHit.collider.isTrigger == true)
			{
				bottomRightHit = new RaycastHit2D();
			}
		}
		if (bottomLeftHit.collider != null)
		{
			if (bottomLeftHit.collider.isTrigger == true)
			{
				bottomLeftHit = new RaycastHit2D();
			}
		}

		if (rightHit.collider != null && leftHit.collider == null && bottomHit.collider == null)
		{
			// Wall on right
			rbody.velocityX = 0f;
			rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
			rbody.gravityScale = 0f;
			orientation = Constants.Player.Orientations.WallOnRight;
		}
		else if (rightHit.collider == null && leftHit.collider != null && bottomHit.collider == null)
		{
			// Wall on left
			rbody.velocityX = 0f;
			rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
			rbody.gravityScale = 0f;
			orientation = Constants.Player.Orientations.WallOnLeft;
		}
		else if (rightHit.collider == null && leftHit.collider == null && bottomHit.collider != null)
		{
			// Wall on bottom
			rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
			rbody.gravityScale = 1f;
			orientation = Constants.Player.Orientations.Flat;
		}
		else if (rightHit.collider != null && bottomHit.collider != null)
		{
			// Wall on right and bottom
			if (lastInput == Constants.Player.Inputs.A)
			{
				// Was coming down wall, move off wall
				rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
				rbody.gravityScale = 1f;
				orientation = Constants.Player.Orientations.Flat;
			}
			else if (lastInput == Constants.Player.Inputs.D)
			{
				// Was moving towards wall, go up wall
				rbody.velocityY = move.ReadValue<float>() * movespeed * Time.deltaTime;
				rbody.gravityScale = 0f;
				orientation = Constants.Player.Orientations.WallOnRight;
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
				orientation = Constants.Player.Orientations.WallOnLeft;
			}
			else if (lastInput == Constants.Player.Inputs.D)
			{
				// Was coming down wall, move off wall
				rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
				rbody.gravityScale = 1f;
				orientation = Constants.Player.Orientations.Flat;
			}
		}
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
					orientation = Constants.Player.Orientations.Flat;
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
					orientation = Constants.Player.Orientations.WallOnRight;
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
					orientation = Constants.Player.Orientations.Flat;
				}
				else if (move.ReadValue<float>() > 0f)
				{
					// Moving up a ramp on the right
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					orientation = Constants.Player.Orientations.Flat;
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
					orientation = Constants.Player.Orientations.Flat;
				}
				else if (lastInput == Constants.Player.Inputs.D)
				{
					// Going over edge towards the left
					rbody.velocityX = 0;
					rbody.velocityY = -move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					orientation = Constants.Player.Orientations.WallOnLeft;
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
					orientation = Constants.Player.Orientations.Flat;
				}
			}
			else
			{
				if (move.ReadValue<float>() < 0f)
				{
					// Moving up a ramp on the left
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					orientation = Constants.Player.Orientations.Flat;

				}
				else if (move.ReadValue<float>() > 0f)
				{
					// Moving down a ramp on the right
					rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
					rbody.velocityY = -Mathf.Abs(move.ReadValue<float>()) * movespeed * Time.deltaTime;
					rbody.gravityScale = 1f;
					orientation = Constants.Player.Orientations.Flat;
				}
				else
				{
					// Not moving on ramp
					rbody.velocity = Vector2.zero;
					rbody.gravityScale = 0f;
				}
			}
		}
		else if (rightHit.collider == null && leftHit.collider == null && bottomHit.collider == null && bottomRightHit.collider == null && bottomLeftHit.collider == null)
		{
			// Freefall, allow for air strafe
			rbody.velocityX = move.ReadValue<float>() * movespeed * Time.deltaTime;
			rbody.gravityScale = 1f;
			orientation = Constants.Player.Orientations.Flat;
		}
	}

	private void HandleAnimations()
	{
		anim.SetFloat("Mass", rbody.mass);
		anim.SetFloat("Velocity", Mathf.Abs(rbody.velocity.x) + Mathf.Abs(rbody.velocityY));
		anim.SetBool("LongDroppingBool", dropping && rbody.mass > 1f);
		anim.SetBool("ShortDroppingBool", dropping && rbody.mass == 1f);
	}

	private void HandleBones()
	{
		if (dropping)
		{
			boneBlend = 0f;
			boneTailBlend = 0f;
			return;
		}

		//if (groundedColliders.Count <= 0)
		//{
		//	// not touching any walls, revert to default
		//	boneBlend = 0f;
		//	boneTailBlend = 0f;
		//	Debug.Log("not touching any colliders");
		//	return;
		//}

		Vector2 headDirection = Vector2.right;
		Vector2 tailDirection = Vector2.left;
		Vector2 bodyDirection = Vector2.down;

		if (orientation == Constants.Player.Orientations.Flat)
		{
			headDirection = transform.TransformDirection(transform.localScale.x > 0 ? Vector2.right : Vector2.left);
			tailDirection = transform.TransformDirection(transform.localScale.x > 0 ? Vector2.left : Vector2.right);
			bodyDirection = transform.TransformDirection(Vector2.down);
		}
		else if (orientation == Constants.Player.Orientations.WallOnLeft)
		{
			headDirection = transform.TransformDirection(transform.localScale.y > 0 ? Vector2.down : Vector2.up);
			tailDirection = transform.TransformDirection(transform.localScale.y > 0 ? Vector2.up : Vector2.down);
			bodyDirection = transform.TransformDirection(Vector2.left);
		}
		else if (orientation == Constants.Player.Orientations.WallOnRight)
		{
			headDirection = transform.TransformDirection(transform.localScale.y > 0 ? Vector2.up : Vector2.down);
			tailDirection = transform.TransformDirection(transform.localScale.y > 0 ? Vector2.down : Vector2.up);
			bodyDirection = transform.TransformDirection(Vector2.right);
		}

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;
        contactFilter.layerMask = 1 << LayerMask.NameToLayer("Wall");
		contactFilter.useLayerMask = true;

        RaycastHit2D[] hitResults = new RaycastHit2D[1];
		int headHitCount = Physics2D.CircleCast(transform.position, Constants.Player.CirclecastRadius, headDirection, contactFilter, hitResults, Constants.Player.BoneRaycastDistance);
		RaycastHit2D headHit = headHitCount == 0 ? new RaycastHit2D() : hitResults[0];

        int tailhitCount = Physics2D.CircleCast(transform.position, Constants.Player.CirclecastRadius, tailDirection, contactFilter, hitResults, Constants.Player.BoneRaycastDistance);
        RaycastHit2D tailHit = tailhitCount == 0 ? new RaycastHit2D() : hitResults[0];

        int bodyHitCount = Physics2D.CircleCast(transform.position, Constants.Player.CirclecastRadius, bodyDirection, contactFilter, hitResults, Constants.Player.BoneDownRaycastDistance);
		RaycastHit2D bodyHit = bodyHitCount == 0 ? new RaycastHit2D() : hitResults[0];

		//Debug.DrawRay(transform.position, headDirection, Color.red, 5f);
		//Debug.DrawRay(transform.position, tailDirection, Color.cyan, 5f);
		//Debug.DrawRay(transform.position, bodyDirection, Color.green, 5f);

		//Debug.Log("Orientation: " + orientation);
		//Debug.Log("Head: " + headHit.collider);
		//Debug.Log("Body: " + bodyHit.collider);
		//Debug.Log("Tail: " + tailHit.collider);

		if (headHit.collider != null)
		{
			if (headHit.collider.isTrigger == true)
			{
				headHit = new RaycastHit2D();
			}
		}
		if (tailHit.collider != null)
		{
			if (tailHit.collider.isTrigger == true)
			{
				tailHit = new RaycastHit2D();
			}
		}
		if (bodyHit.collider != null)
		{
			if (bodyHit.collider.isTrigger == true)
			{
				bodyHit = new RaycastHit2D();
			}
		}

		if (headHit.collider == null && tailHit.collider == null && bodyHit.collider == null)
		{
			boneBlend = 0f;
			boneTailBlend = 0f;
		}

		if (headHit.collider != null)
		{
			if (bodyHit.collider == null)
			{
				// Space below, move head down
				// Check if boneBlend is already in that state
				float target = 0f;
				if (orientation == Constants.Player.Orientations.Flat)
				{
					if (lastInput == Constants.Player.Inputs.A)
					{
						target = 0.5f;
					}
					else if (lastInput == Constants.Player.Inputs.D)
					{
						target = 0f;
					}
				}
				else if (orientation == Constants.Player.Orientations.WallOnLeft)
				{
					target = 0f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnRight)
				{
					target = 1f;
				}

				if (boneBlend != target)
				{
					boneBlend = target;
				}
			}
			else
			{
				// No space below, move head up
				// Check if boneBlend is already in that state
				float target = 0f;
				if (orientation == Constants.Player.Orientations.Flat)
				{
					target = 0.5f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnLeft)
				{
					target = 0f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnRight)
				{
					target = 1f;
				}

				if (boneBlend != target)
				{
					boneBlend = target;
				}
			}
		}
		else
		{
			if (bodyHit.collider == null)
			{
				// Space below, move head down
				// Check if boneBlend is already in that state
				float target = 0f;
				if (orientation == Constants.Player.Orientations.Flat)
				{
					if (lastInput == Constants.Player.Inputs.A)
					{
						if (transform.localScale.x < 0)
						{
							target = 0f;
						}
						else if (transform.localScale.x > 0)
						{
							if (transform.localScale.y < 0)
							{
								target = 0.5f;
							}
							else if (transform.localScale.y > 0)
							{
								target = 0f;
							}
						}
					}
					else if (lastInput == Constants.Player.Inputs.D)
					{
						if (transform.localScale.x < 0)
						{
							target = 0f;
						}
						else if (transform.localScale.x > 0)
						{
							if (boneTailBlend == 0f && boneBlend == 0f)
							{
								// Was moving horizontally
								target = 0f;
							}

							ContactPoint2D[] contacts = new ContactPoint2D[1];
							coll.GetContacts(contacts);

							if (contacts[0].point.x < transform.position.x)
							{
								target = -0.5f;
							}
							else if (contacts[0].point.x > transform.position.x)
							{
								target = 0f;
							}
						}
					}
				}
				else if (orientation == Constants.Player.Orientations.WallOnLeft)
				{
					target = -0.5f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnRight)
				{
					target = 0.5f;
				}

				if (boneBlend != target)
				{
					boneBlend = target;
				}
			}
			else
			{
				// No space below, move head to default
				// Check if boneBlend is already in that state
				float target = 0f;
				if (orientation == Constants.Player.Orientations.Flat)
				{
					target = 0f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnLeft)
				{
					target = -0.5f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnRight)
				{
					target = 0.5f;
				}

				if (boneBlend != target)
				{
					boneBlend = target;
				}
			}
		}

		if (tailHit.collider != null)
		{
			if (bodyHit.collider == null)
			{
				// Space below, move head down
				// Check if boneBlend is already in that state
				float target = 0f;
				if (orientation == Constants.Player.Orientations.Flat)
				{
					target = -0.5f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnLeft)
				{
					target = 0f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnRight)
				{
					target = 1f;
				}

				if (boneTailBlend != target)
				{
					boneTailBlend = target;
				}
			}
			else
			{
				// No space below, move head up
				// Check if boneBlend is already in that state
				float target = 0f;
				if (orientation == Constants.Player.Orientations.Flat)
				{
					target = 0.5f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnLeft)
				{
					target = 1f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnRight)
				{
					target = 0f;
				}

				if (boneTailBlend != target)
				{
					boneTailBlend = target;
				}
			}
		}
		else
		{
			if (bodyHit.collider == null)
			{
				// Space below, move tail down
				// Check if boneBlend is already in that state
				float target = 0f;
				if (orientation == Constants.Player.Orientations.Flat)
				{
					if (lastInput == Constants.Player.Inputs.A)
					{
						target = -0.5f;
					}
					else if (lastInput == Constants.Player.Inputs.D)
					{
						if (boneBlend == 0)
						{
							target = -0.5f;
						}
					}
				}
				else if (orientation == Constants.Player.Orientations.WallOnLeft)
				{
					target = 0f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnRight)
				{
					target = -1f;
				}

				if (boneTailBlend != target)
				{
					boneTailBlend = target;
				}
			}
			else
			{
				// No space below, move head to default
				// Check if boneBlend is already in that state
				float target = 0f;
				if (orientation == Constants.Player.Orientations.Flat)
				{
					target = 0f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnLeft)
				{
					target = 0.5f;
				}
				else if (orientation == Constants.Player.Orientations.WallOnRight)
				{
					target = -0.5f;
				}

				if (boneTailBlend != target)
				{
					boneTailBlend = target;
				}
			}
		}
	}

	private void HandleScale()
	{
		if (dropping)
		{
			if (lastInput == Constants.Player.Inputs.A)
			{
				transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
			}
			if (lastInput == Constants.Player.Inputs.D)
			{
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
			}
			return;
		}

		if (lastInput == Constants.Player.Inputs.D)
		{
			if (orientation == Constants.Player.Orientations.Flat)
			{
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), transform.localScale.z);
			}
			else if (orientation == Constants.Player.Orientations.WallOnRight)
			{
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), transform.localScale.z);
			}
			else if (orientation == Constants.Player.Orientations.WallOnLeft)
			{
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), transform.localScale.z);
			}
		}
		else if (lastInput == Constants.Player.Inputs.A)
		{
			if (orientation == Constants.Player.Orientations.Flat)
			{
				transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), transform.localScale.z);
			}
			else if (orientation == Constants.Player.Orientations.WallOnRight)
			{
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), -Mathf.Abs(transform.localScale.y), transform.localScale.z);
			}
			else if (orientation == Constants.Player.Orientations.WallOnLeft)
			{
				transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), -Mathf.Abs(transform.localScale.y), transform.localScale.z);
			}
		}
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		if (context.ReadValue<float>() > 0)
		{
			lastInput = Constants.Player.Inputs.D;
		}
		else if (context.ReadValue<float>() < 0)
		{
			lastInput = Constants.Player.Inputs.A;
		}
	}

	public void OnDrop(InputAction.CallbackContext context)
	{
		if (interactable != null)
		{
			interactable.StopInteract();
		}

		rbody.velocity = Vector2.zero;
		rbody.gravityScale = 1f;
		dropping = true;

		boneBlend = 0f;
		boneTailBlend = 0f;

		transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
	}

	public void OnSplit(InputAction.CallbackContext context)
	{
		if (rbody.mass > 1f)
		{
			anim.SetTrigger("LongSplitTrigger");

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

        eventBroker.Subscribe<LevelEvents.EndLevel>(EndLevelHandler);
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

        eventBroker.Unsubscribe<LevelEvents.EndLevel>(EndLevelHandler);

    }

    private void EndLevelHandler(BrokerEvent<LevelEvents.EndLevel> @event)
    {
		anim.SetBool("LongVictoryDance", true);
		canMove = false;
		rbody.velocity = Vector2.zero;
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
		transform.localPosition = Vector3.zero;
		rbody.velocity = Vector3.zero;
		rbody.gravityScale = 0;
		
		if (rbody.mass == 1)
		{
			anim.SetBool("ShortDandelionBool", true);
		}
		else if (rbody.mass == 2)
		{
			anim.SetBool("LongDandelionBool", true);
		}
	}

	public void OnDetached(GameObject dandylion)
	{
		canMove = true;
		interactable = null;
		transform.parent = null;
		rbody.gravityScale = 1;

		if (rbody.mass == 1)
		{
			anim.SetBool("ShortDandelionBool", false);
		}
		else if (rbody.mass == 2)
		{
			anim.SetBool("LongDandelionBool", false);
		}
	}

	public float GetWeight()
	{
		return rbody.mass;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			groundedColliders.Add(collision.transform.transform);
		}

		dropping = false;
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			if (!groundedColliders.Contains(collision.transform.transform))
			{
				groundedColliders.Add(collision.transform.transform);
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
		{
			groundedColliders.Remove(collision.transform.transform);
		}
	}
}
