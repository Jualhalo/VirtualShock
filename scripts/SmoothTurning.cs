using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SmoothTurning : MonoBehaviour
{
    public Transform player;
    public Transform cameraObj;
    public float turnSpeed = 45f;
    public SteamVR_Action_Boolean turnLeft;
    public SteamVR_Action_Boolean turnRight;

    void Update()
    {
        Vector3 cameraPosition = cameraObj.position;

        if (turnLeft.state)
        {
            player.RotateAround(cameraPosition, Vector3.up, -turnSpeed * Time.deltaTime);
        }
        if (turnRight.state)
        {
            player.RotateAround(cameraPosition, Vector3.up, turnSpeed * Time.deltaTime);
        }
    }
}
