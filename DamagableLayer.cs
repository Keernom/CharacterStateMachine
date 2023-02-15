using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableLayer : MonoBehaviour, IDamagable
{
    [SerializeField] private float damage = 10;

    public float GetDamage()
    {
        return damage;
    }
}
