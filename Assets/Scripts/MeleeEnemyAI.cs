using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyAI : MonoBehaviour
{
    [SerializeField]
    GameObject target, targetingSupport;
    List<GameObject> enemies;
    public EnemyStats stats;
    GameObject[] possibleTargets;
    float timePassed;
    bool supportTarget;
    Vector3 repulsionVector;
    [SerializeField]
    float repulsionConstant, playerAttraction, repulsionDistance;
    Rigidbody rb;
    GameObject gameController;
    float boostTime=0f;
    bool boostDeactivated = false;

    [SerializeField]
    AudioClip AttackSound;

    AudioSource audio;

    void Start()
    {
        timePassed = 0;
        audio = gameObject.GetComponent<AudioSource>();
        repulsionVector = Vector3.zero;
        rb = gameObject.GetComponent<Rigidbody>();
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
        stats = new EnemyStats(100f, 100f, 10f, 0.6f, 1f, 0f, 0f);
        gameController.SendMessage("AddEnemy", gameObject);
        float multiplier = gameController.GetComponent<GameController>().SetEnemyStatMultiplier(gameController.GetComponent<GameController>().level);
        stats.Boost(multiplier);
    }

    

    GameObject[] possibleEnemies;

    private void Swarming()        //TODO prhysic.overlapsphere
    {
        Vector3 separation = Vector3.zero, adhesion = Vector3.zero, center = Vector3.zero;
        float count = 0f;
        enemies = gameController.GetComponent<GameController>().enemies;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != gameObject)
            {
                float distance = (enemies[i].transform.position - gameObject.transform.position).magnitude;
                if (distance <= 1.7f)
                {
                    separation -= (enemies[i].transform.position - gameObject.transform.position) * GetValueFromRange(distance, 5f);
                }
                if (distance <= 1000f)
                {
                    center += enemies[i].transform.position;
                    count += 1f;
                }
            }
        }
        adhesion = ((target.transform.position) - gameObject.transform.position);// 00f;
        Vector3 velocityAdd = separation + adhesion / 10f;
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
        if (boostTime <= 0f && !boostDeactivated)
        {
            boostDeactivated = true;
        }

        timePassed += Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, target.transform.position - gameObject.transform.position, out hit, stats.range))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                if (timePassed > stats.attackSpeed)
                {
                    timePassed = 0;
                    target.SendMessage("Damage", stats.damage);
                    audio.PlayOneShot(AttackSound);
                }
            }
        }
        gameObject.transform.LookAt(target.transform.position);
    }

    void SupportTarget(GameObject gameObject)
    {
        this.gameObject.layer = 8;
        supportTarget = true;
        targetingSupport = gameObject;
    }

    void Damage(float v)
    {
        stats.health -= v;
        if (stats.health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void GetEnemies(List<GameObject> list)
    {
        Debug.Log("Got message");
        enemies = list;
    }

    public void Boost(float time)
    {
        boostDeactivated = false;
        boostTime = time;
        stats.health = stats.maxHealth;
    }

    private void OnDestroy()
    {
        if (supportTarget)
        {
            targetingSupport.SendMessage("FindNewTarget");
        }
        gameController.SendMessage("RemoveEnemy", gameObject);
    }
}
