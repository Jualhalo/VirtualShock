using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerTriggerPosition : MonoBehaviour
{
    public Transform camera;
    public Transform collider;

    void Update()
    {
        collider.position = new Vector3(camera.position.x,collider.position.y,camera.position.z);
    }
}
