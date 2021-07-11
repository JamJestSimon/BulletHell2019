using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportEnemyAI : MonoBehaviour
{
    public GameObject target;
    GameObject[] possibleTargets;
    List<GameObject> enemies;
    Vector3 repulsionVector;
    [SerializeField]
    float repulsionDistance, repulsionConstant, targetAttraction, playerRepulsion;
    Rigidbody rb;
    GameObject player;
    float playerDistance, boostTime=0f;
    bool boostDeactivated = true;
    public EnemyStats stats;
    GameObject gameController;
    void Start()
    {
        possibleTargets = FindObjectsOfType<GameObject>();
        for(int i = 0; i < possibleTargets.Length; i++)
        {
            if (possibleTargets[i].tag == "Shooting Enemy" || possibleTargets[i].tag == "Melee Enemy")
            {
                target = possibleTargets[i]; //TODO Random target
                target.SendMessage("SupportTarget", gameObject);
            }
            else if (possibleTargets[i].tag == "Player")
            {
                player = possibleTargets[i];
            }
            else if (possibleTargets[i].tag == "GameController") gameController = possibleTargets[i];
            else if (gameController != null && target != null && player != null) break;
        }
        rb = gameObject.GetComponent<Rigidbody>();
        stats = new EnemyStats(120f, 120f, 0f, 10f, 0f, 0.1f, 10f);
        gameObject.GetComponentInChildren<ThrowingEnemyGun>().GetStats(stats);
        gameController.SendMessage("AddEnemy", gameObject);
        float multiplier = gameController.GetComponent<GameController>().SetEnemyStatMultiplier(gameController.GetComponent<GameController>().level);
        stats.Boost(multiplier);
    }

    private void Swarming()        //TODO prhysic.overlapsphere
    {
        Vector3 separation = Vector3.zero, adhesionToPlayer = Vector3.zero, separationFromPlayer = Vector3.zero, separationFromTarget = Vector3.zero;
        float count = 0f;
        enemies = gameController.GetComponent<GameController>().enemies;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != gameObject)
            {
                float distance = (enemies[i].transform.position - gameObject.transform.position).magnitude;
                if (distance <= 5f)
                {
                    separation -= (enemies[i].transform.position - gameObject.transform.position) * GetValueFromRange(distance, 5f);
                }
            }
        }

        if ((player.transform.position - gameObject.transform.position).magnitude <= 10f)
        {
            separationFromPlayer = -(player.transform.position - gameObject.transform.position);
            separationFromPlayer *= GetValueFromRange((player.transform.position - gameObject.transform.position).magnitude, 10f);
        }
        if (target != null && (target.transform.position - gameObject.transform.position).magnitude <= 10f)
        {
            separationFromTarget = -(target.transform.position - gameObject.transform.position);
            separationFromTarget *= GetValueFromRange((target.transform.position - gameObject.transform.position).magnitude, 10f);
        }

        if (target != null) adhesionToPlayer = ((target.transform.position) - gameObject.transform.position);
        Vector3 velocityAdd = separation + separationFromPlayer + adhesionToPlayer / 2f;
        velocityAdd.y = 0f;
        gameObject.GetComponent<Rigidbody>().velocity += Limit(velocityAdd, 1f);
    }

    float GetValueFromRange(float x, float range)
    {
        return 1f - x / range;
    }

    Vector3 Limit(Vector3 v, float l)
    {
        Vector3 ret = v;
        if (v.magnitude > l)
        {
            Vector3.Normalize(ret);
            ret *= l;
        }
        return ret;
    }

    void Update()
    {
        Swarming();
        if (boostTime > 0f) boostTime -= Time.deltaTime;
        if ( boostTime <= 0f && !boostDeactivated)
        {
            boostDeactivated = true;
            //
        }
    }

    void FindNewTarget()
    {
        possibleTargets = FindObjectsOfType<GameObject>();
        for (int i = 0; i < possibleTargets.Length; i++)
        {
            if (possibleTargets[i].tag == "Shooting Enemy" || possibleTargets[i].tag == "Melee Enemy")
            {
                target = possibleTargets[i];
                target.SendMessage("SupportTarget", gameObject);
            }
        }
    }
    void GetEnemies(List<GameObject> list)
    {
        enemies = list;
    }

    void Damage(float v)
    {
        stats.health -= v;
        if (stats.health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Boost(float time)
    {
        boostDeactivated = false;
        boostTime = time;
        stats.health = stats.maxHealth;
    }

    private void OnDestroy()
    {
        try { gameController.SendMessage("RemoveEnemy", gameObject); } catch { }
    }
}
