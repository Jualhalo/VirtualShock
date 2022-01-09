using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Roam : MonoBehaviour
{
    private AI_Master aiMaster;
    private NavMeshAgent navMeshAgent;
    private float checkRate;
    private float nextCheck;
    private float roamTimelimit = 30f;
    private float roamLimitNext;
    [SerializeField]private float wanderRange = 100;
    private Transform aiTransform;
    private NavMeshHit navHit;  
    private Vector3 wanderTarget;

    void Awake()
    {
        aiMaster = GetComponent<AI_Master>();
        if (GetComponent<NavMeshAgent>() != null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.isStopped = true;
        }
        checkRate = Random.Range(0.1f, 0.2f);
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
        if (Time.time > nextCheck)
        {
            nextCheck = Time.time + checkRate;

            if (aiMaster.target == null && !aiMaster.onRoute && !aiMaster.navPaused)
            {
                SetWanderDestination();
            }

            if (aiMaster.target == null && aiMaster.onRoute && !aiMaster.navPaused)
            {
                //if it takes longer than 30 seconds for the ai to reach its wandertarget, set a new destination.
                //This is to prevent the AI from getting stuck trying to move where it cant move to
                if (Time.time > roamLimitNext)
                {
                    roamLimitNext = Time.time + roamTimelimit;
                    SetWanderDestination();
                }
            }
        }
    }

    void SetWanderDestination()
    {
        if (RandomWanderTarget(aiTransform.position, wanderRange, out wanderTarget))
        {
            if (!aiMaster.searchState)
            {
                aiMaster.CallEventIsWalking();
                navMeshAgent.speed = 1;
            }
            else
            {
                aiMaster.CallEventIsRunning();
                navMeshAgent.speed = 5;
            }
            aiMaster.onRoute = true;
            aiMaster.roaming = true;
            if (aiMaster.onRoute && (aiMaster.walking || aiMaster.running))
            {
                navMeshAgent.SetDestination(wanderTarget);
                navMeshAgent.isStopped = false;
            }
        }
    }

    bool RandomWanderTarget(Vector3 centre, float range, out Vector3 result)
    {
        //finds a random target location for the nav mesh agent to move to
        Vector3 randomPoint = centre + Random.insideUnitSphere * range;
        //if the position cant be reached, check if the navmesh can get near it
        if (NavMesh.SamplePosition(randomPoint, out navHit, 1.0f, NavMesh.AllAreas))
        {
            result = navHit.position;
            return true;
        }
        else
        {
            result = centre;
            return false;
        }
    }

    void DisableThis()
    {
        //when the AI dies, this component will be disabled
        this.enabled = false;
    }
}
