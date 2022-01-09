using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class KinematicObjectCollision : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision col)
    {
        //when a game object with a tag "Object" touches this item, turn off kinematic
        if (col.gameObject.CompareTag("Object"))
        {
            rb.isKinematic = false;
        }
    }

    // Disable kinematic when the player grabs the object
    void OnAttachedToHand()
    {
        rb.isKinematic = false;
    }
}
