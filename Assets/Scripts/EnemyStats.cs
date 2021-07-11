using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats
{
    public float maxHealth, health, damage, range, attackSpeed, fireRate, projectileSpeed;

    public EnemyStats(float maxHealth, float health, float damage, float range, float attackSpeed, float fireRate, float projectileSpeed)
    {
        this.maxHealth = maxHealth;
        this.health = health;
        this.damage = damage;
        this.range = range;
        this.attackSpeed = attackSpeed;
        this.fireRate = fireRate;
        this.projectileSpeed = projectileSpeed;
    }

    public void Boost(float multiplier)
    {
        health *= multiplier;
        damage *= multiplier;
        range *= multiplier;
        attackSpeed *= multiplier;
        fireRate *= multiplier;
        projectileSpeed *= multiplier;
    }
}
