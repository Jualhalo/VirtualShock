using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    private float checkRate = 1f;
    private float nextCheck;
    private float detectRadius = 40f;
    public float fovAngle = 110f;
    public LayerMask enemyLayer;
    public LayerMask sightLayer;
    private RaycastHit hit;

    void OnEnable()
    {

    }

    void Update()
    {
        DetectEnemy();
    }
    /*
    void OnTriggerStay(Collider enemy)
    {
        if (enemy.gameObject.tag == "Enemy")
        {
            Vector3 direction = enemy.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);
            if (Time.time > nextCheck)
            {
                nextCheck = Time.time + checkRate;
                if (angle < fovAngle * 0.5f)
                {
                    //if the enemy is within fov, check if enemy is in line of sight
                    LineOfSight(enemy.transform);
                }
            }
        }
    }*/

    void DetectEnemy()
    {
        //checks if the enemy is within player's field of view
        if (Time.time > nextCheck)
        {
            nextCheck = Time.time + checkRate;
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectRadius, enemyLayer);

            if (colliders.Length > 0)
            {
                foreach (Collider target in colliders)
                {
                    if (target.CompareTag("Enemy"))
                    {
                        Vector3 direction = target.transform.position - transform.position;
                        float angle = Vector3.Angle(direction, transform.forward);

                        if (angle < fovAngle * 0.5f)
                        {
                            if (LineOfSight(target.transform))
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    bool LineOfSight(Transform target)
    {
        //checks if there are any objects blocking the player's sight between the player and the seen enemy
        if (Physics.Linecast(transform.position, target.position, out hit, sightLayer))
        {
            if (hit.transform == target)
            {
                AI_Master aiMaster = target.transform.root.gameObject.GetComponent<AI_Master>();
                if (aiMaster != null)
                {
                    if (!aiMaster.isDead)
                    {
                        //Debug.Log("enemy sighted");
                        aiMaster.seenByPlayer = true;
                        if (aiMaster.stealthState)
                        {
                            aiMaster.CallEventExitStealthState();
                            aiMaster.CallEventGotoHuntState();
                        }
                    }                   
                }               
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }        
    }
}
