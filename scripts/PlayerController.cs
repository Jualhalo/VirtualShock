using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public SteamVR_Action_Vector2 input;
    public SteamVR_Action_Boolean sprint;
    public SteamVR_Action_Boolean jump;
    public SteamVR_Action_Boolean inventoryToggleLeft;
    public SteamVR_Action_Boolean inventoryToggleRight;
    public GameObject inventoryObject;
    public GameObject camera;
    public GameObject damageOverlay;
    public PlayerWatch watch;

    public float staminaDrainSpeed;
    public float staminaRecoverySpeed;
    public float jumpSpeed = 1;
    public float walkSpeed = 1;
    public float runSpeed = 2;
    public float crouchSpeed = 0.75f;
    public float pushPower = 2f;
    public float gravity = 1;
    public float crouchThreshold = 1;
    public float stepInterval = 1f;
    public AudioClip[] footSteps;
    public AudioClip jumpAudio;
    public AudioClip fallAudio;
    //public Vector3 velocity;
    public bool grounded;
    public float stepVolume = 1f;

    private PlayerCrouch playerCrouch;
    private float sprintStepInterval;
    private float walkStepInterval;
    private float crouchStepInterval;
    private float maxStamina;
    private float maxHP;
    private float currentStamina;
    [SerializeField] private float currentHP;
    private float timeStampStep = 0;
    private float timeStampFall = 0;
    private AudioSource audio;
    private float fallAudioInterval = 0.2f;
    private CharacterController characterController;
    private float vSpeed = 0;//How fast you are going up or down
    private Noise noise;
    [SerializeField] private GameObject inventoryMenuPanel;
    [SerializeField] private Transform inventoryPositionLeft;
    [SerializeField] private Transform inventoryPositionRight;

    public bool dead;

    private void Start()
    {
        maxStamina = 100.0f;
        maxHP = 100;
        characterController = GetComponent<CharacterController>();
        playerCrouch = GetComponent<PlayerCrouch>();
        audio = GetComponent<AudioSource>();
        noise = GetComponent<Noise>();
        walkStepInterval = stepInterval;
        sprintStepInterval = stepInterval / 2;
        crouchStepInterval = stepInterval * 1.5f;
        instance = this;
        damageOverlay.SetActive(false);
        //camera.GetComponent<Camera>().farClipPlane = 500;
    }


    void Update()
    {
        Vector3 direction = Vector3.zero;

        //Prevents the player from moving with the thumbsticks inside the cryopod if main menu is open
        if (GameController.instance.gameStarted)
        {
            Vector3 right = new Vector3(Player.instance.hmdTransform.right.x, 0, Player.instance.hmdTransform.right.z);
            Vector3 forward = new Vector3(Player.instance.hmdTransform.forward.x, 0, Player.instance.hmdTransform.forward.z);

            direction = right * input.axis.x + forward * input.axis.y;
        }

        //Set the player controllers height based on the heaset height
        float playerHeight = Player.instance.eyeHeight;
        characterController.height = playerHeight;
        Vector3 position = new Vector3(Player.instance.hmdTransform.localPosition.x, playerHeight/2 + 0.1f, Player.instance.hmdTransform.localPosition.z);
        characterController.center = position;

        float speed;

        //velocity = characterController.velocity;
        if (!dead)
        {
            if (sprint.state && !playerCrouch.crouching && Player.instance.eyeHeight > crouchThreshold && currentStamina > 0.0f)
            {
                speed = runSpeed;
                stepInterval = sprintStepInterval;
                currentStamina -= staminaDrainSpeed * Time.deltaTime;
                if (watch != null)
                {
                    watch.UpdateStamina(currentStamina);
                }
            }
            else
            {
                speed = walkSpeed;
                stepInterval = walkStepInterval;
                currentStamina += staminaRecoverySpeed * Time.deltaTime;

                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }

                if (watch != null)
                {
                    watch.UpdateStamina(currentStamina);
                }
            }

            if (characterController.isGrounded)
            {
                vSpeed = 0;

                if (jump.stateDown)
                {
                    audio.PlayOneShot(jumpAudio);
                    vSpeed = jumpSpeed;
                }
            }

            if (playerCrouch.crouching || Player.instance.eyeHeight < crouchThreshold)
            {
                speed = crouchSpeed;
                stepInterval = crouchStepInterval;
                stepVolume = 0.75f;
            }
            if (!playerCrouch && !sprint.state)
            {
                speed = walkSpeed;
                stepInterval = walkStepInterval;
                stepVolume = 1f;
            }

            vSpeed -= gravity * Time.deltaTime;

            characterController.Move(speed * Time.deltaTime * direction + new Vector3(0, vSpeed, 0) * Time.deltaTime);

            //Vector3.ProjectOnPlane(direction, Vector3.up)
            if (inventoryToggleLeft.stateDown)
            {
                if (inventoryObject.transform.position == inventoryPositionLeft.position)
                {
                    inventoryObject.SetActive(!inventoryObject.activeSelf);
                }
                else
                {
                    inventoryObject.transform.SetParent(inventoryPositionLeft, false);
                    inventoryObject.SetActive(true);
                }
            }
            else if (inventoryToggleRight.stateDown)
            {
                if (inventoryObject.transform.position == inventoryPositionRight.position)
                {
                    inventoryObject.SetActive(!inventoryObject.activeSelf);
                }
                else
                {
                    inventoryObject.transform.SetParent(inventoryPositionRight, false);
                    inventoryObject.SetActive(true);
                }
            }

            if ((input.axis.x > 0 || input.axis.y > 0) && timeStampStep < Time.time)
            {
                timeStampStep = Time.time + stepInterval;
                PlayFootStepAudio();
            }
        }       
      
        if (characterController.velocity.y < -2 && IsGrounded() && timeStampFall < Time.time)
        {
            timeStampFall = Time.time + fallAudioInterval;
            audio.PlayOneShot(fallAudio);
            noise.SoundRangeColliderOn(10, 0.1f);
        }

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body != null)
        {
            if (!body.isKinematic || hit.moveDirection.y > -0.3)
            {
                Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                body.velocity = pushDir * pushPower;
            }
        }
    }

    void PlayFootStepAudio()
    {
        if (!characterController.isGrounded)
        {
            return;
        }

        int n = Random.Range(1, footSteps.Length);
        audio.clip = footSteps[n];
        audio.PlayOneShot(audio.clip,stepVolume);
        if (!playerCrouch && !sprint.state)
        {
            noise.SoundRangeColliderOn(5f,0.1f);
        }

        if (sprint.state)
        {
            noise.SoundRangeColliderOn(10f, 0.1f);
        }

        footSteps[n] = footSteps[0];
        footSteps[0] = audio.clip;
    }

    bool IsGrounded()
    {
        if(Physics.Raycast(camera.transform.position, Vector3.down, Player.instance.eyeHeight + 0.01f))
        {
            return true;
        }
        return false;
    }

    public bool RestoreHP(int amount)
    {
        if ((amount > 0) && currentHP == maxHP)
        {
            return false;
        }

        //if the amount of hp restored is negative (damage), activate damage overlay
        if (!dead && amount < 0)
        {
            damageOverlay.SetActive(true);
            StartCoroutine(DisableDamageOverlay());
        }

        currentHP += amount;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        if (currentHP <= 0)
        {
            currentHP = 0;
            //kill the player when hp reaches 0 or below
            PlayerDeath();
        }

        if (watch != null)
        {
            UpdateHPMeter();
        }

        return true;
    }

    IEnumerator DisableDamageOverlay()
    {
        yield return new WaitForSeconds(1f);
        damageOverlay.SetActive(false);
    }

    public void PlayerDeath()
    {
        dead = true;
        if (DatabaseController.instance != null)
        {
            DatabaseController.instance.deaths++;
        }
        SteamVR_Fade.Start(Color.black, 5f,true);
        Invoke("CallSceneRestart",5);
    }

    public void CallSceneRestart()
    {
        GameController.instance.RestartLevel();
    }

    public void UpdateHPMeter()
    {
        watch.UpdateHealth(currentHP);
    }
}
