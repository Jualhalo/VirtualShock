using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEndTrigger : MonoBehaviour
{
    public GameObject [] aiObjects;
    public GameObject otherTrigger;
    public SpaceRotation spaceRotation;
    public GameObject musicPlayer;

    void Start()
    {
        //disable all AI objects associated with this component
        for (int i = 0; i < aiObjects.Length; i++)
        {
            aiObjects[i].SetActive(false);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //once the player touches the trigger, enable monsters around the level
        if (col.tag == "PlayerController")
        {
            for (int i = 0; i < aiObjects.Length; i++)
            {
                aiObjects[i].SetActive(true);
            }

            if (musicPlayer.GetComponent<MusicPlayer>() != null)
            {
                musicPlayer.GetComponent<MusicPlayer>().StartLobbyAmbient();
            }
            
            //start rotating the skybox
            if (spaceRotation != null)
            {
                spaceRotation.rotationStarted = true;
            }           
            DisableThis();
        }        
    }

    void DisableThis()
    {
        //disable the trigger to prevent the player from triggering it multiple times
        Destroy(otherTrigger);
        Destroy(this);
    }
}