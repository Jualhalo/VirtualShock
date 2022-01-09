using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_DestinationReached : MonoBehaviour
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
            DestinationReachedCheck();
        }
    }

    void DestinationReachedCheck()
    {
        if (aiMaster.onRoute)
        {
            if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
            {
                aiMaster.onRoute = false;
                aiMaster.CallEventTargetReached();
                navMeshAgent.isStopped = true;

                if (aiMaster.heardSound)
                {
                    aiMaster.heardSound = false;
                    aiMaster.CallEventExitSearchState();
                }
            }
        }
    }

    void DisableThis()
    {
        //when the AI dies, this component will be disabled
        this.enabled = false;
    }
}
