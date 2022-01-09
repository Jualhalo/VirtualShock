using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerCrouch : MonoBehaviour
{
    public SteamVR_Action_Boolean crouch;
    public Transform camera;
    public float crouchHeight = 0.9f;
    public float crouchSpeed = 1;

    public bool crouching;
    private float originalHeight;

    void Start()
    {
        originalHeight = camera.localPosition.y;
        crouching = false;
    }

    void Update()
    {
        RaycastHit hitUp;

        if (crouch.stateDown)
        {
            crouching = !crouching;
        }

        if (crouching && camera.localPosition.y > originalHeight - crouchHeight)
        {
            camera.localPosition = new Vector3(camera.localPosition.x, camera.localPosition.y - crouchSpeed * Time.deltaTime, camera.localPosition.z);
            if (camera.localPosition.y <= originalHeight - crouchHeight)
            {
                camera.localPosition = new Vector3(camera.localPosition.x, originalHeight - crouchHeight, camera.localPosition.z);
            }
        }

        if (!Physics.Raycast(Player.instance.hmdTransform.position, Vector3.up, out hitUp, crouchHeight + 0.2f) && !crouching && camera.localPosition.y < originalHeight)
        {
            camera.localPosition = new Vector3(camera.localPosition.x, camera.localPosition.y + crouchSpeed * Time.deltaTime, camera.localPosition.z);
            if (camera.localPosition.y >= originalHeight)
            {
                camera.localPosition = new Vector3(camera.localPosition.x, originalHeight, camera.localPosition.z);
            }
        }

    }

    public bool CheckHeight(float crouchThreshold)
    {
        RaycastHit hitDown;

        if (Physics.Raycast(Player.instance.hmdTransform.position, Vector3.down, out hitDown))
        {
            float distance = Vector3.Distance(Player.instance.hmdTransform.position, hitDown.transform.position);

            if (distance < crouchThreshold)
            {
                return true;
            }
        }
        return false;
    }

}
