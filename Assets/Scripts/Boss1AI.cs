using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1AI : MonoBehaviour
{
    GameObject target;
    [SerializeField]
    GameObject bullet, DeadBoss;
    [SerializeField]
    float maxHealth, health;
    bool attackIsDone;
    [SerializeField]
    float attackLength, bulletSpeed, attackDistance,  recoveryLength, fireRate, damage;
    [SerializeField]
    int numberOfAttacks;


    int numberOfArms;


    int n = 2;

    [SerializeField]
    AudioClip CircleAttackSound, DeathSound, AmbientSound;

    AudioSource audio;



    void Start()
    {
        health = maxHealth;
        attackIsDone = true;
        audio = gameObject.GetComponent<AudioSource>();
        GameObject[] possiblePlayers = FindObjectsOfType<GameObject>();
        for (int i = 0; i < possiblePlayers.Length; i++)
        {
            if (possiblePlayers[i].tag == "Player")
            {
                target = possiblePlayers[i];
                break;
            }
        }
    }
    int mode = 0;

    float timePass = 0f, reloadTime = 0f;
    void Update()
    {
        switch (mode)
        {
            case 0://Spoczynek
                if(timePass >= recoveryLength)
                {
                    timePass = 0f;
                    mode = Mathf.RoundToInt(UnityEngine.Random.Range(1, n + 1));
                    if(mode == 1)
                    {
                        numberOfArms = Mathf.RoundToInt(UnityEngine.Random.Range(2, 8));
                    }
                   
                    CircleFire(180);
                }
                gameObject.transform.LookAt(target.transform.position);
                MoveToTarget(7f);
                break;
            case 1://atak 1

                if(timePass > attackLength)
                {
                    CircleFire(180);
                    timePass = 0f;
                    mode = 0;
                    break;
                }
                if(reloadTime <= 0f)
                {
                    CircleFire(numberOfArms);
                    reloadTime = 1f / fireRate;
                }
                reloadTime -= Time.deltaTime;
                gameObject.transform.Rotate(0, -1f, 0);
                break;
            case 2:
                if (timePass > attackLength)
                {
                    CircleFire(180);
                    timePass = 0f;
                    mode = 0;
                    break;
                }
                if(reloadTime <= 0f)
                {
                    SpreadFire();
                    reloadTime = 1f / fireRate;
                }
                reloadTime -= Time.deltaTime;
                gameObject.transform.LookAt(target.transform.position);
                MoveToTarget(5f);
                break;
        }
        timePass += Time.deltaTime;
    }

    private void MoveToTarget(float v)
    {
        GetComponent<Rigidbody>().velocity = (-Vector3.Normalize(transform.position - target.transform.position) * v);
    }

    void CircleFire(int n)
    {
        float angle = 360f / n;
        for(float f = 0; f < 360f; f += angle)
        {
            audio.PlayOneShot(CircleAttackSound);
            GameObject firedBullet = Instantiate(bullet, gameObject.transform.position, Quaternion.Euler(0, f + gameObject.transform.rotation.eulerAngles.y, 0));
            firedBullet.SendMessage("GetDamage", damage);
            Destroy(firedBullet, attackDistance / bulletSpeed);
        }
    }

    void SpreadFire()
    {
        float angle = UnityEngine.Random.Range(-45f, 45f);
        GameObject firedBullet = Instantiate(bullet, gameObject.transform.position, Quaternion.Euler(0, gameObject.transform.rotation.eulerAngles.y + angle, 0));
        firedBullet.SendMessage("GetDamage", damage);
        Destroy(firedBullet, attackDistance / bulletSpeed);
    }

    void Fire()
    {
        GameObject firedBullet = Instantiate(bullet, gameObject.transform.position, gameObject.transform.rotation);
        firedBullet.SendMessage("GetDamage", damage);
        Destroy(firedBullet, attackDistance / bulletSpeed);
    }

    void Damage(float value)
    {
        health -= value;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        audio.PlayOneShot(DeathSound);
        Instantiate(DeadBoss, gameObject.transform.position, Quaternion.identity);
    }
}
