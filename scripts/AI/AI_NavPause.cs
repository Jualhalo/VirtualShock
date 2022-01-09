using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_NavPause : MonoBehaviour
{
    private AI_Master aiMaster;
    private NavMeshAgent navMeshAgent;
    private bool wasWalking;
    private bool wasRunning;

    void Awake()
    {
        aiMaster = GetComponent<AI_Master>();
        if (GetComponent<NavMeshAgent>() != null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        aiMaster.EventDie += DisableThis;
        aiMaster.EventNavPause += PauseNavAgent;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventDie -= DisableThis;
        aiMaster.EventNavPause -= PauseNavAgent;
    }*/

    void PauseNavAgent(float pauseTime)
    {
        if (aiMaster.walking)
        {
            aiMaster.walking = false;
            wasWalking = true;
        }

        if (aiMaster.running)
        {
            aiMaster.running = false;
            wasRunning = true;
        }

        if (navMeshAgent != null)
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.ResetPath();
                aiMaster.navPaused = true;
                StopAllCoroutines();
                StartCoroutine(RestartNavAgent(pauseTime));
            }
        }
    }

    IEnumerator RestartNavAgent(float pauseTime)
    {
        yield return new WaitForSeconds(pauseTime);
        if (wasWalking)
        {
            wasWalking = false;
            aiMaster.walking = true;            
        }

        if (wasRunning)
        {
            wasRunning = false;
            aiMaster.running = true;           
        }
        aiMaster.navPaused = false;
    }

    void DisableThis()
    {
        //when the AI dies, this component will be disabled
        StopAllCoroutines();
        if (GetComponent<NavMeshAgent>() != null)
        {
            navMeshAgent.enabled = false;
        }
        this.enabled = false;
    }
}
