using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Tutorial : MonoBehaviour
{
    private AI_Master aiMaster;
    private AI_Audio aiAudio;
    private NavMeshAgent navMeshAgent;
    private Transform aiTransform;
    public Transform[] waypoints;    
    private float nextCheck;
    private float checkRate = 0.1f;
    private int i = 0;
    private bool aiIntroStarted;

    void OnEnable()
    {
        aiMaster = GetComponent<AI_Master>();
        aiAudio = GetComponent<AI_Audio>();
        if (GetComponent<NavMeshAgent>() != null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        aiTransform = transform;
    }

    void Update()
    {
        if (aiIntroStarted)
        {
            if (Time.time > nextCheck)
            {
                nextCheck = Time.time + checkRate;
                DestinationReachedCheck();
            }
        }      
    }

    public void StartAIIntro()
    {
        SetNewDestination();
        if (aiAudio != null)
        {
            aiAudio.TutorialAudioAggro();
        }       
        aiIntroStarted = true;        
    }

    void DestinationReachedCheck()
    {
        if (aiMaster.onRoute)
        {
            if (navMeshAgent.remainingDistance < 1)
            {
                if (i == 1)
                {
                    GameController.instance.ShowSprintHint(10.0f);
                }
                if (i == 2)
                {
                    aiAudio.TutorialAudioHuntState();
                }
                aiMaster.onRoute = false;
                aiMaster.CallEventTargetReached();
                navMeshAgent.isStopped = true;
                i++;
                if (i > 3)
                {
                    DestroyIntroPropAI();
                }
                else
                {
                    SetNewDestination();
                }               
            }
        }
    }

    void SetNewDestination()
    {
        if (i < waypoints.Length)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(waypoints[i].position);
            navMeshAgent.speed = 1;
            aiMaster.onRoute = true;
            aiMaster.CallEventIsWalking();
        }              
    }

    public void DestroyIntroPropAI()
    {
        Destroy(aiTransform.gameObject);
    }
}
