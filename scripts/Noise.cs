using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Noise : MonoBehaviour
{
    //The range of the noise
    public GameObject noiseColliderPrefab;

    public void SoundRangeColliderOn(float radius, float timer)
    {
        //score = noiseScore;
        GameObject soundRange = Instantiate(noiseColliderPrefab, transform.position, Quaternion.identity);
        soundRange.GetComponentInChildren<SphereCollider>().radius = radius;
        StartCoroutine(DestroySoundRangeCollider(timer, soundRange));
    }

    IEnumerator DestroySoundRangeCollider(float timer, GameObject soundRange)
    {
        yield return new WaitForSeconds(timer);
        Destroy(soundRange);
    }
}
