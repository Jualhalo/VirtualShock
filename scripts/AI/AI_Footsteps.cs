using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Footsteps : MonoBehaviour
{
    private AI_Master aiMaster;
    private AudioSource audio;
    public AudioClip[] footSteps;

    void Awake()
    {
        aiMaster = GetComponent<AI_Master>();
        audio = GetComponent<AudioSource>();
        aiMaster.EventDie += DisableThis;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventDie -= DisableThis;
    }*/

    void PlayFootStepAudio()
    {
        //this function is called from the run and walk animations
        //plays a random footstep audio from an array
        int n = Random.Range(1, footSteps.Length);
        audio.clip = footSteps[n];
        audio.PlayOneShot(audio.clip, 1);
        footSteps[n] = footSteps[0];
        footSteps[0] = audio.clip;
    }

    void DisableThis()
    {
        //when the AI dies, this component will be disabled
        this.enabled = false;
    }
}
