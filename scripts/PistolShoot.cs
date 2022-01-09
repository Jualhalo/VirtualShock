using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PistolShoot : MonoBehaviour
{
    public float bulletHoleDestroyTimer = 60.0f;
    public Transform pistolEnd;
    //public Transform bullet;
    public ParticleSystem muzzleFlashLong;
    public ParticleSystem muzzleFlashShort;
    public GameObject hitPrefab;
    public GameObject enemyHitPrefab;
    public GameObject muzzleLight;
    private AudioSource audio;
    public AudioClip shootAudio;
    public AudioClip noBulletsAudio;
    public AudioClip clipInsertAudio;
    public AudioClip clipRemoveAudio;
    public GameObject bulletHoleDecal;
    public GameObject bulletHoleDecalWindow;

    public bool inHand;
    private Noise noise;

    //the weapon's fire rate
    public float fireRate = 0.25f;
    private float nextFire = 0;

    //duration of particle effect
    public float muzzleLightTimer = 0.1f;

    //weapon range
    public float range = 50;
    public float offset = 0.005f;

    //damage done to enemy on hit
    public int damage = 25;

    //when the raycast hits a rigidbody, this force pushes the object
    public float hitForce = 100;

    //ENABLE THESE FOR VR
    public SteamVR_Action_Boolean shoot;
    public SteamVR_Action_Vector2 magazineReleaseSwipe;
    public SteamVR_Action_Boolean magazineReleaseToggle;
    private Interactable pistol;
    private PistolMagazine magazine;

    [SerializeField] private Text ammoCountText;

    void Start()
    {
        magazine = GetComponentInChildren<PistolMagazine>();
        if (magazine != null)
        {
            if (magazine.bulletsLeft > 0)
            {
                ammoCountText.text = magazine.bulletsLeft.ToString();
            }
            else
            {
                ammoCountText.text = "N/A";
            }
        }
        else
        {
            ammoCountText.text = "N/A";
        }
        if (muzzleLight != null)
        {
            muzzleLight.SetActive(false);
        }

        audio = GetComponent<AudioSource>();
        noise = GetComponent<Noise>();

        //ENABLE FOR VR
        pistol = GetComponent<Interactable>();
    }

    void Update()
    {
        //NON-VR INPUT
        /*
        if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }*/


        
        //VR INPUT
        if (pistol.attachedToHand != null)
        {
            SteamVR_Input_Sources source = pistol.attachedToHand.handType;
            if (shoot[source].stateDown && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Shoot();
            }

            if ((magazineReleaseToggle[source].stateDown || magazineReleaseSwipe[source].axis.y < 0.0f) && magazine != null)
            {
                magazine.gameObject.transform.parent = null;
                magazine.DetachFromPistol();
                magazine = null;
                ammoCountText.text = "N/A";
                audio.PlayOneShot(clipRemoveAudio);
            }
        }
    }

    void Shoot()
    {

        //old code for shoot testing
        /*
        Instantiate(bullet, new Vector3(pistolEnd.position.x, pistolEnd.position.y, pistolEnd.position.z),
            Quaternion.identity);
        */
        if (magazine == null || magazine.bulletsLeft < 1)
        {
            audio.PlayOneShot(noBulletsAudio);
            return;
        }

        RaycastHit hit;

        //offset range for the shot
        Vector3 offsetVect = new Vector3(Random.Range(-offset, offset), Random.Range(-offset, offset), Random.Range(-offset, offset));

        if (Physics.Raycast(pistolEnd.transform.position, pistolEnd.transform.forward + offsetVect, out hit, range))
        {
            //Spawn a bullethole decal to the hit position
            if (hit.transform.gameObject.layer != 8)
            {
                GameObject decal;
                if (hit.collider.gameObject.CompareTag("Window"))
                {
                    decal = Instantiate(bulletHoleDecalWindow, hit.point, Quaternion.LookRotation(-hit.normal, Vector3.up));
                }
                else
                {
                    decal = Instantiate(bulletHoleDecal, hit.point, Quaternion.LookRotation(-hit.normal, Vector3.up));
                }
                decal.transform.SetParent(hit.transform, true);
                Destroy(decal, bulletHoleDestroyTimer);
            }

            //Do damage to enemies on hit
            if (hit.transform.gameObject.tag == "Enemy")
            {
                GameObject go = (GameObject)Instantiate(enemyHitPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                if (hit.transform.gameObject.GetComponent<AI_TakeDamage>() != null)
                {
                    hit.transform.gameObject.GetComponent<AI_TakeDamage>().DoDamage(damage);
                    
                    if (hit.transform.root.gameObject.GetComponent<AI_Master>() != null)
                    {
                        if (!hit.transform.root.gameObject.GetComponent<AI_Master>().isDead)
                        {
                            //Record the bullet hit on enemy to the database controller, bullet hits on dead enemies are not recorded
                            if (DatabaseController.instance != null)
                            {
                                DatabaseController.instance.bulletsHitOnEnemy++;
                            }
                        }
                    }                    
                }                              
            }

            //instantiate a particle effect on hit
            if (hitPrefab != null)
            {
                if (hit.transform.gameObject.tag != "Enemy")
                {
                    GameObject go = (GameObject)Instantiate(hitPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }                   
            }

            if (hit.rigidbody != null)
            {
                //push rigidbodies hit by the gun's raycast

                hit.rigidbody.AddForce(-hit.normal * hitForce, ForceMode.Impulse);
            }

            magazine.bulletsLeft--;
            magazine.UpdateColor();
            if (magazine.bulletsLeft == 0)
            {
                ammoCountText.text = "0";
                if (!GameController.instance.magReleaseTutorial)
                {
                    GameController.instance.ShowMagReleaseHint(pistol.attachedToHand, 10.0f);
                }
            }
            else
            {
                ammoCountText.text = magazine.bulletsLeft.ToString();
            }


        }

        //play muzzle flash particle effect and enable point light
        muzzleFlashLong.Play();
        muzzleFlashShort.Play();
        if (muzzleLight != null)
        {
            MuzzleLightSourceOn();
        }

        //play audio for the shot
        audio.pitch = Random.Range(0.9f, 1.1f);
        audio.PlayOneShot(shootAudio);

        //Alert nearby enemies with noise
        noise.SoundRangeColliderOn(20,0.1f);

        //Record the shot to the database controller
        if (DatabaseController.instance != null)
        {
            DatabaseController.instance.shotBullets++;
        }
        
    }

    void MuzzleLightSourceOn()
    {
        muzzleLight.SetActive(true);

        //turn off point light after a time
        Invoke("MuzzleLightSourceOff", muzzleLightTimer);
    }
    void MuzzleLightSourceOff()
    {
        muzzleLight.SetActive(false);
    }

    public void AttachMagazine(PistolMagazine mag)
    {
        if (inHand && magazine == null)
        {

            if (!mag.AttachToPistol())
            {
                return;
            }
            magazine = mag;
            magazine.gameObject.transform.parent = gameObject.transform;
            magazine.gameObject.transform.localRotation = Quaternion.identity;
            magazine.gameObject.transform.localPosition = Vector3.zero;
            //audio.pitch = 1.0f;
            audio.PlayOneShot(clipInsertAudio);
            if (magazine.bulletsLeft > 0)
            {
                ammoCountText.text = magazine.bulletsLeft.ToString();
            }
            else
            {
                ammoCountText.text = "EMPTY";
            }
        }
    }

    void OnAttachedToHand(Hand hand)
    {
        inHand = true;
        if (!GameController.instance.shootTutorial)
        {
            GameController.instance.ShowShootHint(hand, 10.0f);
        }
    }

    void OnDetachedFromHand()
    {
        inHand = false;
    }

}
