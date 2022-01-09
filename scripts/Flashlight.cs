using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Flashlight : MonoBehaviour
{
    public SteamVR_Action_Boolean flashLightToggle;
    public GameObject light;
    public AudioClip flashLightToggleAudio;

    private Interactable flashlight;
    private bool toggleBool =false;
    private InventoryItem invItem;
    private AudioSource audio;
    private Renderer[] renderers;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        invItem = GetComponent<InventoryItem>();
        light.SetActive(toggleBool);
        flashlight = GetComponent<Interactable>();
        renderers = GetComponentsInChildren<Renderer>();
        ToggleEmission();
    }

    void Update()
    {
        if (flashlight.attachedToHand != null)
        {
            SteamVR_Input_Sources source = flashlight.attachedToHand.handType;
            if (flashLightToggle[source].stateDown)
            {
                ToggleLight();
            }
        }

        if (light.activeInHierarchy && invItem.inInventory)
        {
            ToggleLight();
        }
    }

    void ToggleLight()
    {
        toggleBool = !toggleBool;
        light.SetActive(toggleBool);
        audio.PlayOneShot(flashLightToggleAudio);
        ToggleEmission();
    }

    void OnAttachedToHand(Hand hand)
    {
        if (!GameController.instance.flaslightTutorial)
        {
            GameController.instance.ShowFlashlightTutorial(hand, 10.0f);
        }
    }

    void ToggleEmission()
    {
        foreach (Renderer renderer in renderers)
        {
            if (toggleBool)
            {
                renderer.material.EnableKeyword("_EMISSION");
            }
            else
            {
                renderer.material.DisableKeyword("_EMISSION");
            }
        }
    }
}
