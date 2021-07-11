using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemyAI : MonoBehaviour
{
    public GameObject target;
    bool supportTarget;
    GameObject targetingSupport;
    GameObject[] possibleTargets;
    List<GameObject> enemies;
    Rigidbody rb;
    Vector3 repulsionVector;
    [SerializeField]
    float repulsionDistance, repulsionConstant, playerAttraction;
    public EnemyStats stats;
    GameObject gameController;
    float boostTime=0f;
    bool boostDeactivated = false;

    [SerializeField]
    AudioClip DeathSound;

    AudioSource audio;
    void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
        possibleTargets = FindObjectsOfType<GameObject>();
        for (int i = 0; i < possibleTargets.Length; i++)
        {
            if (possibleTargets[i].tag == "Player")
            {
                target = possibleTargets[i];
            }
            else if (possibleTargets[i].tag == "GameController") gameController = possibleTargets[i];
            else if (gameController != null && target != null) break;
        }
        gameObject.GetComponentInChildren<ShootingEnemyGun>().target = target;
        rb = gameObject.GetComponent<Rigidbody>();
        stats = new EnemyStats(80f, 80f, 8f, 10f, 0f, 2f, 0f);
        gameObject.GetComponentInChildren<ShootingEnemyGun>().GetStats(stats);
        gameController.SendMessage("AddEnemy", gameObject);
        float multiplier = gameController.GetComponent<GameController>().SetEnemyStatMultiplier(gameController.GetComponent<GameController>().level);
        stats.Boost(multiplier);
    }

    private void Swarming()        //TODO prhysic.overlapsphere
    {
        Vector3 separation = Vector3.zero, adhesionToPlayer = Vector3.zero, separationFromPlayer = Vector3.zero;
        float count = 0f;
        enemies = gameController.GetComponent<GameController>().enemies;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != gameObject)
            {
                float distance = (enemies[i].transform.position - gameObject.transform.position).magnitude;
                if (distance <= 5f)
                {
                    separation -= (enemies[i].transform.position - gameObject.transform.position)*GetValueFromRange(distance, 5f);
                }
            }
        }

        if ((target.transform.position - gameObject.transform.position).magnitude <= 6f)
        {
            separationFromPlayer = -(target.transform.position - gameObject.transform.position);
            separationFromPlayer *= GetValueFromRange((target.transform.position - gameObject.transform.position).magnitude, 6f);
        }

        adhesionToPlayer = ((target.transform.position) - gameObject.transform.position);
        Vector3 velocityAdd = separation + separationFromPlayer + adhesionToPlayer/2f;
        velocityAdd.y = 0f;
        gameObject.GetComponent<Rigidbody>().velocity += Limit(velocityAdd, 1f); ;
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
        if (boostTime <= 0f && !boostDeactivated)
        {
            boostDeactivated = true;
            //
        }
    }

    void SupportTarget(GameObject gameObject)
    {
        this.gameObject.layer = 8;
        supportTarget = true;
        targetingSupport = gameObject;
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
        if (supportTarget && targetingSupport != null)
        {
            targetingSupport.SendMessage("FindNewTarget");
        }
        gameController.SendMessage("RemoveEnemy", gameObject);
    }

}
