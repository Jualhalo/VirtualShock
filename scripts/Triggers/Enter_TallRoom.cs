using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enter_TallRoom : MonoBehaviour
{
    public GameObject musicPlayer;
    void OnTriggerEnter(Collider col)
    {       
        if (col.tag == "PlayerController")
        {
            if (musicPlayer.GetComponent<MusicPlayer>() != null)
            {
                musicPlayer.GetComponent<MusicPlayer>().StartTallRoomAmbient();
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
