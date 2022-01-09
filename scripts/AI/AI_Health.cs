using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Health : MonoBehaviour
{
    private AI_Master aiMaster;
    public int health = 100;

    void Awake()
    {
        aiMaster = GetComponent<AI_Master>();
        aiMaster.EventTakeDamage += TakeDamage;
    }
    /*
    void OnDisable()
    {
        aiMaster.EventTakeDamage -= TakeDamage;
    }*/

    void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);
        if (health <= 0)
        {
            Debug.Log("AI_Health: health 0 time to die");
            health = 0;
            aiMaster.CallEventDie();
        }
    }
}
