using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Intro_End : MonoBehaviour
{
    public GameObject Intro_AI_Object;
    public Door ToiletDoor;
    public Door LabExitDoor;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "PlayerController")
        {
            //destroy the AI prop object used in the AI intro
            if (Intro_AI_Object != null)
            {
                if (Intro_AI_Object.GetComponent<AI_Tutorial>() != null)
                {
                    Intro_AI_Object.GetComponent<AI_Tutorial>().DestroyIntroPropAI();
                }
            }            

            //Close both of the doors in the room
            if (ToiletDoor != null)
            {
                ToiletDoor.DoorClose();
            }

            if (LabExitDoor != null)
            {
                LabExitDoor.DoorClose();
            }

            DisableThis();
        }
    }

    void DisableThis()
    {
        //disable the trigger to prevent the player from triggering it multiple times
        Destroy(this);
    }
}
