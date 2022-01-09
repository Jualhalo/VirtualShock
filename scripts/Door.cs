using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    public bool doorIsOpenOnStart;
    public float doorMoveTime = 5;
    
    public bool doorIsOpen;
    public bool doorIsBlocked;
    public bool locked;
    public bool doorIsBulkhead;

    public AudioClip doorOpenAudio;
    public AudioClip doorCloseAudio;

    public float doorOpenHeight;
    private float doorOpenCoord;
    private AudioSource audio;
    private float doorClosedCoordY;
    private Sequence DoorOpenTween;
    private Sequence DoorCloseTween;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        DOTween.Init();

        if (doorIsOpenOnStart)
        {
            doorOpenCoord = transform.position.y;
            doorClosedCoordY = transform.position.y - doorOpenHeight;
            doorIsOpen = true;
        }

        if (!doorIsOpenOnStart)
        {
            doorOpenCoord = transform.position.y + doorOpenHeight;
            doorClosedCoordY = transform.position.y;
            doorIsOpen = false;
        }
    }

    public void DoorOpen()
    {
        doorIsOpen = true;
        DoorCloseTween.Kill();
        DoorOpenTween.Append(transform.DOMoveY(doorOpenCoord, doorMoveTime, false));
        audio.PlayOneShot(doorOpenAudio);

        //have the door automatically close after a while
        //bulkheads are special and will always remain open
        if (!doorIsBulkhead)
        {
            Invoke("DoorClose", 10);
        }
    }

    public void DoorClose()
    {
        CancelInvoke();
        doorIsOpen = false;
        DoorOpenTween.Kill();
        DoorCloseTween.Append(transform.DOMoveY(doorClosedCoordY, doorMoveTime, false));
        audio.PlayOneShot(doorCloseAudio);
    }
    void OnTriggerStay(Collider col)
    {
        //prevent the door from closing if something is blocking it
        if (!doorIsBulkhead)
        {
            if ((col.tag == "Object" || col.tag == "PlayerController" || col.tag == "Player" || 
                 col.tag == "Enemy" || col.tag == "PlayerHands") && !doorIsOpen && !(transform.position.y <= doorClosedCoordY + 0.1))
            {
                doorIsBlocked = true;
                DoorOpen();
                Debug.Log("ovi tukossa");
            }
        }       
    }

    void OnTriggerExit(Collider col)
    {
        if (!doorIsBulkhead)
        {
            if ((col.tag == "Object" || col.tag == "PlayerController" || col.tag == "Player" || 
                 col.tag == "Enemy" || col.tag == "PlayerHands") && doorIsBlocked)
            {
                doorIsBlocked = false;
                Debug.Log("nyt on kunnollista");
            }
        }        
    }
}
