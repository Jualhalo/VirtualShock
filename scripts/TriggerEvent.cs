using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class TriggerEvent : MonoBehaviour
{
    public GameObject gameObject;
    public GameObject gameObject2;
    public float timer = 0;
    private bool triggered = false;
    // Start is called before the first frame update
    void Start()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
        if (gameObject2 != null)
        {
            gameObject2.SetActive(false);
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (gameObject != null)
        {
            if (col.tag == "Player" && !triggered)
            {
                Invoke("setGameObjectActive", timer);
            }
        }
    }

    void setGameObjectActive()
    {
        gameObject.SetActive(true);
        gameObject2.SetActive(true);
        triggered = true;
    }
}
