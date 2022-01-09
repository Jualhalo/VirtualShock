using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Audio : MonoBehaviour
{
    private AI_Master aiMaster;
    private AudioSource audio;
    private Noise noise;
    public AudioClip audioAggro;
    public AudioClip audioAttack;
    public AudioClip audioBackToNeutral;
    public AudioClip audioHuntState;
    public AudioClip audioHuntState2;
    public AudioClip audioDamage;
    public AudioClip audioDeath;
    public AudioClip audioDeath2;

    void Awake()
    {
        aiMaster = GetComponent<AI_Master>();
        audio = GetComponent<AudioSource>();
        noise = GetComponent<Noise>();
        aiMaster.EventDie += AudioDeath;
        aiMaster.EventTakeNormalDamage += AudioNormalDamage;
        aiMaster.EventTakeBackDamage += AudioBackDamage;
        aiMaster.EventTakeCriticalDamage += AudioCriticalDamage;
        aiMaster.EventAggro += AudioAggro;
        aiMaster.EventAttack += AudioAttack;
        aiMaster.EventGotoNeutralState += AudioBackToNeutral;
        aiMaster.EventGotoHuntState += AudioHuntState;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventDie -= AudioDeath;
        aiMaster.EventTakeNormalDamage -= AudioNormalDamage;
        aiMaster.EventTakeBackDamage -= AudioBackDamage;
        aiMaster.EventTakeCriticalDamage -= AudioCriticalDamage;
        aiMaster.EventAggro -= AudioAggro;
        aiMaster.EventAttack -= AudioAttack;
        aiMaster.EventGotoNeutralState -= AudioBackToNeutral;
        aiMaster.EventGotoHuntState -= AudioHuntState;
    }*/

    void AudioAggro()
    {
        audio.PlayOneShot(audioAggro, 1f);
        noise.SoundRangeColliderOn(20,0.1f);
    }

    void AudioAttack()
    {
        audio.PlayOneShot(audioAttack, 1f);
        noise.SoundRangeColliderOn(20, 0.1f);
    }

    void AudioBackToNeutral()
    {
        if (!aiMaster.searchState)
        {
            audio.PlayOneShot(audioBackToNeutral, 1f);
        }      
    }

    void AudioNormalDamage()
    {
        audio.PlayOneShot(audioDamage, 1f);
    }
    void AudioBackDamage()
    {
        audio.PlayOneShot(audioDamage, 1f);
    }
    void AudioCriticalDamage()
    {
        audio.PlayOneShot(audioDamage, 1f);
    }

    void AudioHuntState()
    {
        Debug.Log("playing hunt state audio");
        int random = Random.Range(0, 1);
        if (random == 1)
        {
            audio.PlayOneShot(audioHuntState, 1f);
        }
        else
        {
            audio.PlayOneShot(audioHuntState2, 1f);
        }             
    }

    void AudioDeath()
    {
        //play one of the death sounds at random
        int random = Random.Range(0, 2);
        if (random == 1)
        {
            audio.PlayOneShot(audioDeath, 1f);
        }
        else
        {
            audio.PlayOneShot(audioDeath2, 1f);
        }

        //disable the component after a delay to ensure that the audio can play
        Invoke("DisableThis",1f);
    }

    public void TutorialAudioAggro()
    {
        //played during monster intro trigger
        audio.PlayOneShot(audioAggro, 1f);
    }

    public void TutorialAudioHuntState()
    {
        //played during monster intro trigger
        audio.PlayOneShot(audioHuntState, 1f);
    }

    void DisableThis()
    {
        this.enabled = false;
    }

}
