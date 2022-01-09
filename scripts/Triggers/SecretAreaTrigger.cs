using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretAreaTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "PlayerController")
        {
            if (DatabaseController.instance != null)
            {
                DatabaseController.instance.secretsFound++;
                GameController.instance.DisplayMessage("You have found a secret area!", 10.0f, GameController.LeftHand);
            }
            DisableThis();
        }
    }

    void DisableThis()
    {
        //disable the secret area trigger to prevent the player from triggering it multiple times
        Destroy(this);
    }
}
