using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Intro_Start : MonoBehaviour
{
    public GameObject Intro_AI_Object;
    public GameObject musicPlayer;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "PlayerController")
        {           
            if (Intro_AI_Object.GetComponent<AI_Tutorial>() != null)
            {
                Intro_AI_Object.GetComponent<AI_Tutorial>().StartAIIntro();
            }
            if (musicPlayer.GetComponent<MusicPlayer>() != null)
            {
                musicPlayer.GetComponent<MusicPlayer>().StartAiIntroAmbient();
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
