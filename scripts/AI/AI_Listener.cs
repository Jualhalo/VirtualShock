using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Listener : MonoBehaviour
{
    private AI_Master aiMaster;
    private NavMeshAgent navMeshAgent;
    
    void Awake()
    {
        aiMaster = GetComponentInParent<AI_Master>();
        if (GetComponentInParent<NavMeshAgent>() != null)
        {
            navMeshAgent = GetComponentInParent<NavMeshAgent>();
        }
        aiMaster.EventDie += DisableThis;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventDie -= DisableThis;
    }*/

    void OnTriggerEnter(Collider col)
    {
        //if the AI collides with a noise trigger collider, it will set its destination to the source of the noise
        if (col.tag == "Noise")
        {
            float noiseVolume = CalculateNoiseVolume(col.gameObject.transform.parent.transform);
            if (aiMaster.target == null && navMeshAgent != null && !aiMaster.huntState && noiseVolume > 0)
            {
                if (!aiMaster.searchState)
                {
                    aiMaster.CallEventGotoSearchState();
                }
               
                navMeshAgent.SetDestination(col.transform.position);       

                if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                {
                    navMeshAgent.speed = 5;
                    aiMaster.CallEventIsRunning();
                    navMeshAgent.isStopped = false;
                    aiMaster.onRoute = true;
                    aiMaster.heardSound = true;
                    aiMaster.roaming = false;
                }
            }
        }     
    }

    float CalculateNoiseVolume(Transform noise)
    {
        //calculates the volume of the noise heard, depending on the amount and type of obstacles between noise source and the listener
        RaycastHit[] blockingObjects;
        float noiseVolumeToCalculate = 100;
        float distanceToTarget = Vector3.Distance(noise.position, transform.position);

        blockingObjects = Physics.RaycastAll
            (transform.position, noise.position - transform.position, distanceToTarget);
        if (blockingObjects.Length > 0)
        {
            for (int i = 0; i < blockingObjects.Length; i++)
            {
                if (blockingObjects[i].transform.gameObject.tag == "NoiseBlock")
                {
                    noiseVolumeToCalculate -= 35;
                }
                if (blockingObjects[i].transform.gameObject.tag == "NoiseBlock2x")
                {
                    noiseVolumeToCalculate -= 50;
                }
            }
        }
        return noiseVolumeToCalculate;
    }

    void DisableThis()
    {
        //when the AI dies, this component will be disabled
        if (GetComponentInParent<NavMeshAgent>() != null)
        {
            navMeshAgent.enabled = false;
        }
        this.enabled = false;
    }
}
