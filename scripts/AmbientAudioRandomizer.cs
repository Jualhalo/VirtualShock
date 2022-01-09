using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

public class AmbientAudioRandomizer : MonoBehaviour
{
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    [SerializeField] private float minVolume;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private AudioSource audioSource;

    private float timer;
    private float waitTime;
    // Start is called before the first frame update
    void Start()
    {
        waitTime = Random.Range(minTime, maxTime);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= waitTime)
        {
            PlayAudioClip(Random.Range(0, audioClips.Length - 1));
        }
    }

    void PlayAudioClip(int index)
    {
        int yRotation = Random.Range(0, 359);
        int xRotation = Random.Range(0, 359);
        Vector3 rotation = new Vector3(xRotation, yRotation, transform.rotation.z);
        gameObject.transform.rotation = Quaternion.Euler(rotation);
        audioSource.PlayOneShot(audioClips[index], Random.Range(minVolume, 1));
        timer = 0;
        waitTime = Random.Range(minTime, maxTime);
    }

}
