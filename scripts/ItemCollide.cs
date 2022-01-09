using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class ItemCollide : MonoBehaviour
{
    private Interactable interactable;
    private Noise noise;
    private Rigidbody rb;
    private AudioSource audio;
    public AudioClip[] collideNoise;
    public AudioClip[] enemyCollideNoise;
    private float checkrate = 0.5f;
    private float nextcheck;
    private float velocityTotal;
    public GameObject bloodParticle;
    public bool willSpawnBloodParticles;

    //The amount of velocity needed for this item to deal damage to an enemy on collision
    public float damageThresholdVelocity;

    //the amount of damage done by this item colliding on the enemy, multiplied by total velocity
    public float damageMultiplier;

    void Start()
    {
        noise = GetComponent<Noise>();
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
        interactable = GetComponent<Interactable>();
    }

    void FixedUpdate()
    {
        if (rb.velocity.x != 0 || rb.velocity.y != 0 || rb.velocity.z != 0)
        {
            velocityTotal = Mathf.Abs(rb.velocity.x + rb.velocity.y + rb.velocity.z);
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (Time.time > nextcheck)
        {
            nextcheck = Time.time + checkrate;

            //calculate the volume of the collision noise by velocity
            float collideVolume = 0.1f * velocityTotal;

            //limit the volume to prevent it from being absurdly loud
            if (collideVolume > 1f)
            {
                collideVolume = 1f;
            }           

            //only alert enemies with the noise if the velocity is high enough
            if (velocityTotal > 2)
            {
                noise.SoundRangeColliderOn(velocityTotal * 5, 0.1f);
            }

            //play a different sound if the item hits an enemy
            //also do melee damage to the enemy based on velocity and damage multiplier           
            if (col.gameObject.tag == "Enemy")
            {
                if (velocityTotal >= damageThresholdVelocity)
                {
                    if (willSpawnBloodParticles)
                    {
                        GameObject go = (GameObject)Instantiate(bloodParticle, col.contacts[0].point, Quaternion.identity);
                    }

                    int damageToDeal = Convert.ToInt32(velocityTotal * damageMultiplier);
                    if (enemyCollideNoise.Length > 0)
                    {
                        PlayCollideNoise(collideVolume, enemyCollideNoise);
                    }
                    else
                    {
                        PlayCollideNoise(collideVolume, collideNoise);
                    }

                    if (col.gameObject.GetComponent<AI_TakeDamage>() != null)
                    {
                        col.gameObject.GetComponent<AI_TakeDamage>().DoDamage(damageToDeal);
                    }
                    if (interactable != null && DatabaseController.instance != null && interactable.attachedToHand != null)
                    {
                        DatabaseController.instance.meleeHitsOnEnemy++;
                        DatabaseController.instance.meleeHits++;
                    }
                }
            }
            
            else
            {
                //prevent noise before the player has started the game
                //this is to prevent the horrible amount of noise resulting from everything in the level dropping into their places at start
                if (GameController.instance.gameStarted)
                {
                    PlayCollideNoise(collideVolume, collideNoise);
                }
                
                if (interactable != null && DatabaseController.instance != null && interactable.attachedToHand != null)
                {
                    DatabaseController.instance.meleeHits++;
                }               
            }
        }       
    }

    void PlayCollideNoise(float volume, AudioClip[] collide)
    {
        //play a sound at random from the collision sound array 
        audio.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        if (collide.Length > 1)
        {
            int n = UnityEngine.Random.Range(1, collide.Length);
            audio.clip = collide[n];
            audio.PlayOneShot(audio.clip, volume);
            collide[n] = collide[0];
            collide[0] = audio.clip;
        }
        else
        {           
            audio.clip = collide[0];
            audio.PlayOneShot(audio.clip, volume);
        }
    }
}
