using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Master : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public Transform target;
    public bool onRoute;

    public bool huntState;   
    public bool searchState;
    public bool neutralState = true;
    public bool stealthState;

    //used on the AI intro prop to disable some parts of this script
    public bool introProp;

    /*the state when the AI has seen the player, but hasn't yet reached the hunt state*/
    public bool investigationState;

    public bool navPaused;
    public bool walking;
    public bool running;    
    public bool heardSound;
    public bool aggroAnimationPlayed;

    //for debug purposes only, activates when the ai is in it's default roaming state
    public bool roaming;

    //when the enemy is within the player's sight collider range
    public bool seenByPlayer;

    public bool isDead;
    public bool takenDamage;

    public delegate void GeneralEventHandler();
    public event GeneralEventHandler EventDie;
    public event GeneralEventHandler EventAggro;
    public event GeneralEventHandler EventIsWalking;
    public event GeneralEventHandler EventIsRunning;
    public event GeneralEventHandler EventTargetReached;
    public event GeneralEventHandler EventAttack;
    public event GeneralEventHandler EventTargetLost;
    public event GeneralEventHandler EventGotoNeutralState;
    public event GeneralEventHandler EventExitNeutralState;
    public event GeneralEventHandler EventGotoInvestigationState;
    public event GeneralEventHandler EventExitInvestigationState;
    public event GeneralEventHandler EventGotoHuntState;
    public event GeneralEventHandler EventGotoSearchState;
    public event GeneralEventHandler EventExitHuntState;
    public event GeneralEventHandler EventExitSearchState;
    public event GeneralEventHandler EventGotoStealthState;
    public event GeneralEventHandler EventExitStealthState;
    public event GeneralEventHandler EventTakeNormalDamage;
    public event GeneralEventHandler EventTakeBackDamage;
    public event GeneralEventHandler EventTakeCriticalDamage;

    public delegate void HealthEventHandler(int health);
    public event HealthEventHandler EventTakeDamage;

    public delegate void NavTargetEventHandler(Transform targetTransform);
    public event NavTargetEventHandler EventSetNavTarget;

    public delegate void NavPauseEventHandler(float pauseTime);
    public event NavPauseEventHandler EventNavPause;

    public void CallEventTakeDamage(int health)
    {
        if (EventTakeDamage != null)
        {
            EventTakeDamage(health);
        }
    }

    public void CallEventNavPause(float pauseTime)
    {
        if (EventNavPause != null)
        {
            EventNavPause(pauseTime);
        }
    }

    public void CallEventSetNavTarget(Transform targTransform)
    {
        if (EventSetNavTarget != null)
        {
            EventSetNavTarget(targTransform);
        }
        target = targTransform;

        //cancel the timed ending of search state
        CancelInvoke();
    }

    public void CallEventDie()
    {
        if (EventDie != null)   
        {
            EventDie();
        }
        walking = false;
        running = false;
        isDead = true;

        if (DatabaseController.instance != null)
        {
            DatabaseController.instance.kills++;
        }      
    }

    public void CallEventAggro()
    {
        if (EventAggro != null)
        {
            EventAggro();
        }
        aggroAnimationPlayed = true;           
    }

    public void CallEventIsWalking()
    {
        if (EventIsWalking != null)
        {
            EventIsWalking();
        }
        walking = true;
        running = false;
    }

    public void CallEventIsRunning()
    {
        if (EventIsRunning != null)
        {
            EventIsRunning();
        }
        running = true;
        walking = false;
    }

    public void CallEventTargetReached()
    {
        if (EventTargetReached != null)
        {
            EventTargetReached();
        }
        //if the AI was investigating a noise it heard, when it reaches its destination, this will be reset
        heardSound = false;
        walking = false;
        running = false;
    }

    public void CallEventAttack()
    {
        if (EventAttack != null)
        {
            EventAttack();
        }
    }

    public void CallEventTargetLost()
    {
        if (EventTargetLost != null)
        {
            EventTargetLost();
        }
        target = null;       
        if (huntState)
        {
            //if the AI was in hunt state before losing target, it will switch to search state
            CallEventExitHuntState();
            if (!introProp)
            {
                CallEventGotoSearchState();
                //end search state after 30 seconds
                Invoke("CallEventExitSearchState", 30);
            }           
        }        
    }

    public void CallEventGotoNeutralState()
    {
        if (EventGotoNeutralState != null)
        {
            EventGotoNeutralState();
        }
        neutralState = true;
    }
    public void CallEventExitNeutralState()
    {
        if (EventExitNeutralState != null)
        {
            EventExitNeutralState();
        }
        neutralState = false;
    }
    public void CallEventGotoInvestigationState()
    {
        if (EventGotoInvestigationState != null)
        {
            EventGotoInvestigationState();           
        }
        investigationState = true;

        if (neutralState)
        {
            CallEventExitNeutralState();
        }        
    }

    public void CallEventExitInvestigationState()
    {
        if (EventExitInvestigationState != null)
        {
            EventExitInvestigationState();           
        }
        investigationState = false;
        if (!huntState && !searchState && !neutralState)
        {
            CallEventGotoNeutralState();
        }
    }

    public void CallEventGotoHuntState()
    {
        if (EventGotoHuntState != null)
        {
            EventGotoHuntState();            
        }
        huntState = true;
        if (DatabaseController.instance != null)
        {
            DatabaseController.instance.timesGamerSpottedByEnemy++;
        }
        if (investigationState)
        {
            CallEventExitInvestigationState();
        }       
        if (searchState)
        {
            CallEventExitSearchState();
        }       
        if (stealthState)
        {
            CallEventExitStealthState();
        }      
    }

    public void CallEventExitHuntState()
    {
        if (EventExitHuntState != null)
        {
            EventExitHuntState();           
        }
        huntState = false;    
    }

    public void CallEventGotoSearchState()
    {
        if (EventGotoSearchState != null)
        {
            EventGotoSearchState();           
        }

        if (!introProp)
        {
            searchState = true;
        }
    }

    public void CallEventExitSearchState()
    {
        if (EventExitSearchState != null)
        {
            EventExitSearchState();         
        }
        searchState = false;
        takenDamage = false;
        if (!huntState)
        {
            CallEventGotoNeutralState();
        }
    }

    public void CallEventGotoStealthState()
    {
        if (EventGotoStealthState != null)
        {
            EventGotoStealthState();
        }
        CallEventExitNeutralState();
        stealthState = true;
    }

    public void CallEventExitStealthState()
    {
        if (EventExitStealthState != null)
        {
            EventExitStealthState();
        }
        stealthState = false;
    }

    public void CallEventTakeNormalDamage()
    {
        if (EventTakeNormalDamage != null)
        {
            EventTakeNormalDamage();
        }
    }
    public void CallEventTakeBackDamage()
    {
        if (EventTakeBackDamage != null)
        {
            EventTakeBackDamage();
        }
    }
    public void CallEventTakeCriticalDamage()
    {
        if (EventTakeCriticalDamage != null)
        {
            EventTakeCriticalDamage();
        }
    }
}
