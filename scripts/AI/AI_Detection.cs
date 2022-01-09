using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Detection : MonoBehaviour
{
    private AI_Master aiMaster;
    private Transform aiTransform;
    private NavMeshAgent navMeshAgent;
    public Transform head;
    public LayerMask playerLayer;
    public LayerMask sightLayer;
    public float fovAngle = 150f;
    private float checkRate;
    private float nextCheck;
    public float detectRadius = 20f;
    public float detectionScore = 0;
    private float distanceFromPlayer;
    private float detectionScoreTimerCheckRate = 1;
    private float detectionScoreTimerNextCheck;
    private RaycastHit hit;

    void Awake()
    {
        aiMaster = GetComponent<AI_Master>();
        //sightRange = GetComponent<SphereCollider>();
        if (GetComponent<NavMeshAgent>() != null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        checkRate = Random.Range(0.8f, 1.2f);
        aiTransform = transform;

        aiMaster.EventDie += DisableThis;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventDie -= DisableThis;
    }*/

    void Update()
    {
        DetectionFieldOfView();
        LowerDetectionScore();

        if (detectionScore < 0)
        {
            detectionScore = 0;
        }

        if (detectionScore > 10)
        {
            detectionScore = 10;
        }

        //change the fov angle depending on the state the ai is in
        if (aiMaster.neutralState)
        {
            fovAngle = 150f;
        }

        if (aiMaster.searchState)
        {
            fovAngle = 225f;
        }

        if (aiMaster.huntState || aiMaster.stealthState)
        {
            fovAngle = 360f;
        }
    }

    void DetectionFieldOfView()
    {
        if (Time.time > nextCheck)
        {
            nextCheck = Time.time + checkRate;

            //check if the player is within the monster's range
            Collider[] colliders = Physics.OverlapSphere(aiTransform.position, detectRadius, playerLayer);

            if (colliders.Length > 0)
            {
                foreach (Collider target in colliders)
                {
                    if (target.CompareTag("PlayerController"))
                    {
                        Vector3 direction = target.transform.position - aiTransform.position;
                        float angle = Vector3.Angle(direction, aiTransform.forward);

                        //check if the player is within the monster's field of view
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
            else
            {
                aiMaster.CallEventTargetLost();
            }
        }
    }

    bool LineOfSight(Transform target)
    {
        //check if the player can be seen
        if (Physics.Linecast(head.position, target.position, out hit, sightLayer))
        {
            if (hit.transform == target)
            {
                if (aiMaster.neutralState)
                {
                    aiMaster.CallEventGotoInvestigationState();
                }

                if (aiMaster.takenDamage)
                {
                    //if the player has damaged the ai, it will be at maximum alert
                    detectionScore = 10;
                }

                if (detectionScore >= 10)
                {
                    aiMaster.CallEventSetNavTarget(target);

                    if (!aiMaster.huntState && aiMaster.seenByPlayer)
                    {
                        if (aiMaster.stealthState)
                        {
                            aiMaster.CallEventExitStealthState();
                        }                     
                        //switch from detecting player state to hunt state  
                        aiMaster.CallEventGotoHuntState();
                    }
                    
                    if (!aiMaster.stealthState && !aiMaster.seenByPlayer)
                    {      
                        //switch to stealth state if the player hasn't seen the monster
                        aiMaster.CallEventGotoStealthState();
                    }   
                                      
                    if (!aiMaster.stealthState && !aiMaster.aggroAnimationPlayed && !aiMaster.takenDamage)
                    {
                        //roar angrily at the player
                        aiMaster.CallEventAggro();
                    }
                    return true;
                }    
                
                else
                {
                    //if detection score is under 10 points, increase detection score based on distance from player
                    distanceFromPlayer = Vector3.Distance(head.position, target.position);
                    detectionScore += 300 / distanceFromPlayer;
                    aiMaster.CallEventSetNavTarget(target);
                    return true;
                }
            }
            else
            {
                aiMaster.CallEventExitInvestigationState();
                aiMaster.CallEventTargetLost();
                return false;
            }
        }
        else
        {
            aiMaster.CallEventTargetLost();
            return false;
        }
    }

    void LowerDetectionScore()
    {
        //when the monster has returned to default roam state, its detection score will lower
        if (Time.time > detectionScoreTimerNextCheck)
        {
            detectionScoreTimerNextCheck = Time.time + detectionScoreTimerCheckRate;
            if (detectionScore > 0 && !aiMaster.huntState && !aiMaster.searchState 
                && !aiMaster.investigationState && !aiMaster.stealthState)
            {
                detectionScore -= 1;
            }
        }
    }
 
    void DisableThis()
    {
        //when the AI dies, this component will be disabled
        this.enabled = false;
    }
}
