using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    [SerializeField]
    GameObject ChestPrefab, BossPrefab, ShootingEnemyPrefab, SupportEnemyPrefab, MeleeEnemyPrefab;
    GameObject[] enemies;
    List<GameObject> spawnedEnemies;
    GameObject[] gameObjects;
    EnemyStats[] enemyStats = new EnemyStats[] { new EnemyStats(100f, 100f, 10f, 0.6f, 0.5f, 0f, 0f) , new EnemyStats(80f, 80f, 8f, 10f, 0f, 2f, 0f) , new EnemyStats(120f, 120f, 0f, 10f, 0f, 0.1f, 10f) };
    public int level;

    bool used = false;
    public int roomType;//1 start room  2 boss room  3 chest room
    public float w, h;

    private void Start()
    {
        enemies = new GameObject[3];
        enemies[0] = MeleeEnemyPrefab;
        enemies[1] = ShootingEnemyPrefab;
        enemies[2] = SupportEnemyPrefab;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !used)
        {
            Debug.Log("RoomType:" + roomType);
            used = true;

            switch (roomType)
            {
                case 1:

                    break;
                case 2:
                    Spawn(BossPrefab, NumberOfBosses(level));
                    break;  
                case 3:
                    Spawn(ChestPrefab, 1);
                    break;
                default:
                    SpawnEnemies((int)(UnityEngine.Random.value* NumberOfEnemiesPerRoom(level)) +5);
                    break;
            }
        }
    }

    int NumberOfBosses(int n)
    {
        return (int)(1f + (n * 0.5f));
    }

    int NumberOfEnemiesPerRoom(int n)
    {
        return (5 + 2 * n);
    }

    private void SpawnEnemies(int v)
    {
        for(int i = 0; i < v; i++)
        {
            int x = (int)Mathf.Round(UnityEngine.Random.value * (enemies.Length - 1));
            GameObject go = Instantiate(enemies[x]);
            if(go.tag == "Melee Enemy")
            {
                go.GetComponent<MeleeEnemyAI>().stats = enemyStats[x];
            }
            else if(go.tag == "Shooting Enemy")
            {
                go.GetComponent<ShootingEnemyAI>().stats = enemyStats[x];
            }
            else
            {
                go.GetComponent<SupportEnemyAI>().stats = enemyStats[x];
            }
            go.transform.position = transform.position + (new Vector3(w - UnityEngine.Random.value * w * 2f, 5f, h - UnityEngine.Random.value * h * 2f) / 2f);
        }
        spawnedEnemies = new List<GameObject>();
        gameObjects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].tag == "Shooting Enemy" || gameObjects[i].tag == "Melee Enemy" || gameObjects[i].tag == "Support Enemy")
            {
                spawnedEnemies.Add(gameObjects[i]);
            }
        }
    }

    void Spawn(GameObject gameObject, int n)
    {
        for(int i = 0; i < n; i++)
        {
            GameObject go = Instantiate(gameObject);
            go.transform.position = transform.position + (new Vector3(w- UnityEngine.Random.value * w * 2f, 5f, h - UnityEngine.Random.value * h * 2f)/2f);
        }
    }
}
