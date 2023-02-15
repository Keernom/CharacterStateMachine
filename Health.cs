using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float _health;

    void Start()
    {
        PlayerEventManager.OnTakeDamage += TakeDamage;
    }

    private void TakeDamage(float damage)
    {
        _health -= damage;
    }
}
