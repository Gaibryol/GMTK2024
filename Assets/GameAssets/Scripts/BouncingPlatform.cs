using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*  Adds an upwards force to objects that collide with this.
*  Requires to implment IBounceable and have a RigidBody2D
*  Can override forceMultiplier in IBounceable to scale the force exerted on the object.
*/
[RequireComponent(typeof(Collider2D))]
public class BouncingPlatform : MonoBehaviour
{
    private Animator animator;
    
    [SerializeField] private float upwardsForce = 7f;
    private EventBrokerComponent eventBrokerComponent = new EventBrokerComponent();

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IBounceable bounceable = collision.gameObject.GetComponent<IBounceable>();
        if (bounceable == null)
        {
            return;
        }

        Rigidbody2D rbody = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rbody == null)
        {
            return;
        }

        if (bounceable.forceMultiplier > 0.5)
        {
            animator.SetTrigger("Bounce");
        }
        eventBrokerComponent.Publish(this, new AudioEvents.PlaySFX(Constants.Audio.SFX.MushroomJump));

        rbody.AddForceY(upwardsForce * bounceable.forceMultiplier * rbody.mass, ForceMode2D.Impulse);

		collision.gameObject.GetComponent<Animator>().SetTrigger("LongJumpTrigger");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IBounceable bounceable = collision.gameObject.GetComponent<IBounceable>();
        if (bounceable == null)
        {
            return;
        }

        Rigidbody2D rbody = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rbody == null)
        {
            return;
        }
        
        animator.SetBool("InRange", true);

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IBounceable bounceable = collision.gameObject.GetComponent<IBounceable>();
        if (bounceable == null)
        {
            return;
        }

        Rigidbody2D rbody = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rbody == null)
        {
            return;
        }

        animator.SetBool("InRange", false);
    }
}
