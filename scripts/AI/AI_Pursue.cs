using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Pursue : MonoBehaviour
{
    private AI_Master aiMaster;
    private NavMeshAgent navMeshAgent;
    private float checkRate;
    private float nextCheck;

    void Awake()
    {
        aiMaster = GetComponent<AI_Master>();
        if (GetComponent<NavMeshAgent>() != null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        checkRate = Random.Range(0.1f, 0.2f);
        aiMaster.EventDie += DisableThis;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventDie -= DisableThis;
    }*/

    void Update()
    {
        if (Time.time > nextCheck)
        {
            nextCheck = Time.time + checkRate;
            PursueTarget();
        }
    }

    void PursueTarget()
    {
        if (aiMaster.target != null && navMeshAgent != null && !aiMaster.navPaused)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(aiMaster.target.position, out hit, 2f, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
                navMeshAgent.isStopped = false;
            }
            else
            {
                navMeshAgent.SetDestination(aiMaster.target.position);
                navMeshAgent.isStopped = false;
            }

            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                if (aiMaster.huntState || aiMaster.searchState)
                {
                    navMeshAgent.speed = 5;
                    aiMaster.CallEventIsRunning();
                }
                else
                {
                    navMeshAgent.speed = 1;
                    aiMaster.CallEventIsWalking();
                }                               
                aiMaster.onRoute = true;
                aiMaster.roaming = false;
            }
        }
    }

    void DisableThis()
    {
        //when the AI dies, this component will be disabled
        if (GetComponent<NavMeshAgent>() != null)
        {
            navMeshAgent.enabled = false;
        }
        this.enabled = false;
    }
}
