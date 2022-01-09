using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_TakeDamage : MonoBehaviour
{
    private AI_Master aiMaster;
    public int damageMultiplier = 1;

    void Awake()
    {
        aiMaster = transform.root.gameObject.GetComponent<AI_Master>();
        aiMaster.EventDie += DisableThis;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventDie -= DisableThis;
    }*/

    public void DoDamage(int damage)
    {
        int damageToApply = damage * damageMultiplier;       
        if (aiMaster != null)
        {
            aiMaster.CallEventTakeDamage(damageToApply);
            PlayDamageAnimation(damageToApply);
            if (DatabaseController.instance != null)
            {
                DatabaseController.instance.damageGiven += damageToApply;
            }

            //if the monster isn't already angry at the player, it is now
            if (!aiMaster.huntState)
            {
                aiMaster.CallEventGotoHuntState();
                aiMaster.takenDamage = true;
            }
        }
    }

    void PlayDamageAnimation(int damageTaken)
    {
        if (!aiMaster.isDead)
        {

            if (damageTaken >= 20 && damageTaken < 40)
            {
                aiMaster.CallEventTakeNormalDamage();
                aiMaster.CallEventNavPause(0.5f);
            }
            if (damageTaken == 40)
            {
                aiMaster.CallEventTakeBackDamage();
                aiMaster.CallEventNavPause(0.5f);
            }
            if (damageTaken > 40)
            {
                aiMaster.CallEventTakeCriticalDamage();
                aiMaster.CallEventNavPause(2f);
            }
        }
    }

    void DisableThis()
    {
        //when the AI dies, this component will be disabled
        Destroy(this);
    }
}
