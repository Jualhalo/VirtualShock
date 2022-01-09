using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Attack : MonoBehaviour
{
    private AI_Master aiMaster;
    public MusicPlayer musicPlayer;
    private Transform attackTarget;
    private Transform aiTransform;
    private float attackRate = 2f;
    private float nextAttack;
    public float attackRange = 5f;
    public int attackDamage = 30;

    void Awake()
    {
        aiMaster = GetComponent<AI_Master>();
        aiTransform = transform;
        aiMaster.EventDie += DisableThis;
        aiMaster.EventSetNavTarget += SetAttackTarget;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventDie -= DisableThis;
        aiMaster.EventSetNavTarget -= SetAttackTarget;
    }*/
    
    void Update()
    {
        if (aiMaster.huntState || aiMaster.stealthState)
        {
            Attack();
        }            
    }

    void SetAttackTarget(Transform targetTransform)
    {
        attackTarget = targetTransform;
    }
    
    void Attack()
    {
        if (attackTarget != null)
        {
            if (Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate;

                if (Vector3.Distance(aiTransform.position, attackTarget.position) <= attackRange && !aiMaster.navPaused)
                {
                    //turn the monster to look at the player while attacking
                    Vector3 lookAtDirection = new Vector3(attackTarget.position.x, aiTransform.position.y,
                        attackTarget.position.z);
                    aiTransform.LookAt(lookAtDirection);
                    if (aiMaster.stealthState && !aiMaster.aggroAnimationPlayed)
                    {
                        aiMaster.CallEventAggro();
                        aiMaster.CallEventGotoHuntState();

                        //scare the player
                        if (musicPlayer.GetComponent<MusicPlayer>() != null)
                        {
                            musicPlayer.GetComponent<MusicPlayer>().PlayScareAmbient();
                        }
                    }
                    aiMaster.CallEventAttack();
                    aiMaster.CallEventNavPause(1);
                    aiMaster.onRoute = false;
                }                
            }
        }
    }
    public void OnAIAttack()
    {
        //This function is called from the attack animation
        if (attackTarget != null)
        {
            //before dealing damage to player, check if player is in front of the AI and in attack range
            if (Vector3.Distance(aiTransform.position, attackTarget.position) <= attackRange 
                && attackTarget.GetComponentInParent<PlayerController>() != null)
            {
                Vector3 toOther = attackTarget.position - aiTransform.position;

                if (Vector3.Dot(toOther, aiTransform.forward) > 0.5f)
                {
                    attackTarget.GetComponentInParent<PlayerController>().RestoreHP(-attackDamage);
                    if (DatabaseController.instance != null)
                    {
                        DatabaseController.instance.damageTaken += attackDamage;
                    }
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
