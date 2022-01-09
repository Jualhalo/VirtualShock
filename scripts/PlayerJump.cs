using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerJump : MonoBehaviour
{
    public SteamVR_Action_Boolean jump;
    public float jumpHeight = 1;
    public float jumpSpeed = 1;
    public AudioSource audio;

    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (characterController.isGrounded && jump.stateDown)
        {
            Vector3 startPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
            Vector3 jumpApex = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + jumpHeight, gameObject.transform.position.z);
            characterController.Move(Vector3.Slerp(startPosition, jumpApex, jumpSpeed));
            
        }
    }
}
