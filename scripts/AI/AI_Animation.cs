using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Animation : MonoBehaviour
{

    private AI_Master aiMaster;
    private AI_TakeDamage aiTakeDamage;
    private Animator animator;
    private int healthBeforeAnimation;

    void Awake()
    {
        aiMaster = GetComponent<AI_Master>();

        if (GetComponent<Animator>() != null)
        {
            animator = GetComponent<Animator>();
        }

        aiMaster.EventDie += AnimationDeath;
        //aiMaster.EventIsWalking += AnimationWalk;
        //aiMaster.EventIsRunning += AnimationRun;
        aiMaster.EventTargetReached += AnimationIdle;
        aiMaster.EventAttack += AnimationAttack;
        aiMaster.EventAggro += AnimationAggro;
        aiMaster.EventTakeNormalDamage += AnimationNormalDamage;
        aiMaster.EventTakeBackDamage += AnimationBackDamage;
        aiMaster.EventTakeCriticalDamage += AnimationCriticalDamage;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventDie -= AnimationDeath;
        aiMaster.EventIsWalking -= AnimationWalk;
        aiMaster.EventIsRunning -= AnimationRun;
        aiMaster.EventTargetReached -= AnimationIdle;
        aiMaster.EventAttack -= AnimationAttack;
        aiMaster.EventAggro -= AnimationAggro;
        aiMaster.EventTakeNormalDamage -= AnimationNormalDamage;
        aiMaster.EventTakeBackDamage -= AnimationBackDamage;
        aiMaster.EventTakeCriticalDamage -= AnimationCriticalDamage;
    }*/

    void Update()
    {
        if (aiMaster.walking)
        {
            animator.SetBool("isRoaming", true);
            animator.SetBool("isPursuing", false);

        }

        if (aiMaster.running)
        {
            animator.SetBool("isPursuing", true);
            animator.SetBool("isRoaming", false);
        }
    }

    void AnimationIdle()
    {
        if (animator != null)
        {
            if (animator.enabled)
            {
                if (animator.GetBool("isRoaming"))
                {
                    animator.SetBool("isRoaming", false);
                }

                if (animator.GetBool("isPursuing"))
                {
                    animator.SetBool("isPursuing", false);
                }                         
            }
        }
    }
    /*
    void AnimationWalk()
    {
        if (animator != null)
        {
            if (animator.enabled)
            {                              
                if (animator.GetBool("isPursuing"))
                {
                    animator.SetBool("isPursuing", false);
                }          
            }
        }
    }

    void AnimationRun()
    {
        if (animator != null)
        {
            if (animator.enabled)
            {
                animator.SetBool("isPursuing", true);               
                if (animator.GetBool("isRoaming"))
                {
                    animator.SetBool("isRoaming", false);
                }                            
            }
        }
    }*/

    void AnimationAttack()
    {
        if (animator != null)
        {           
            if (animator.enabled)
            {
                animator.SetTrigger("Attack");               
            }                  
        }
    }

    void AnimationNormalDamage()
    {
        if (animator != null)
        {
            if (animator.enabled)
            {
                animator.SetTrigger("normDamage");
            }
        }
    }

    void AnimationBackDamage()
    {
        if (animator != null)
        {
            if (animator.enabled)
            {
                animator.SetTrigger("backDamage");
            }
        }
    }

    void AnimationCriticalDamage()
    {
        if (animator != null)
        {
            if (animator.enabled)
            {
                animator.SetTrigger("critDamage");
            }
        }
    }

    void AnimationDeath()
    {
        if (animator != null)
        {
            if (animator.enabled)
            {
                if (animator.GetBool("isRoaming"))
                {
                    animator.SetBool("isRoaming", false);
                }

                if (animator.GetBool("isPursuing"))
                {
                    animator.SetBool("isPursuing", false);
                }

                //reset all triggers to ensure that no other animation will obstruct the death animation
                animator.ResetTrigger("Attack");
                animator.ResetTrigger("Aggro");
                animator.ResetTrigger("normDamage");
                animator.ResetTrigger("backDamage");
                animator.ResetTrigger("critDamage");

                //disable the animator after a delay to ensure that the death animation can play
                Invoke("DisableThis",1f);

                animator.SetTrigger("Die");              
            }
        }
    }

    void AnimationAggro()
    {
        if (animator != null)
        {
            if (animator.enabled)
            {
                animator.SetTrigger("Aggro");
                aiMaster.CallEventNavPause(2);
            }
        }
    }

    void DisableThis()
    {
        //when the AI dies, this component will be disabled 
        if (animator != null)
        {
            animator.enabled = false;
        }             
    }
}
