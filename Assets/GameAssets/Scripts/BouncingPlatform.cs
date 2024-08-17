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
    
    [SerializeField] private float upwardsForce = 7f;

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

        rbody.AddForceY(upwardsForce * bounceable.forceMultiplier * rbody.mass, ForceMode2D.Impulse);
    }
}
