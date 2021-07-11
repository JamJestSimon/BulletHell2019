using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingEnemyGun : MonoBehaviour
{
    SupportEnemyAI enemy;
    float throwAngle;
    float gravityAcceleration;
    float timePassed;
    [SerializeField]
    GameObject projectile;
    GameObject firedProjectile;
    float distance;
    EnemyStats stats;

    // Start is called before the first frame update
    void Start()
    {
        timePassed = 0f;
        gravityAcceleration = 10f;
        enemy = gameObject.GetComponentInParent<SupportEnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy.target == null) return;
        timePassed += Time.deltaTime;
        gameObject.transform.LookAt(enemy.target.transform);
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, transform.TransformDirection(Vector3.forward), out hit, stats.range))
        {
            if(timePassed >= 1f / stats.fireRate)
            {
                distance = hit.distance;
                timePassed = 0;
                throwAngle = Mathf.Asin(distance * gravityAcceleration / (stats.projectileSpeed * stats.projectileSpeed)) / 2;
                Throw();
            }
        }
    }

    void Throw()
    {
        firedProjectile = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation);
        firedProjectile.SendMessage("SetAngle", throwAngle);
        firedProjectile.SendMessage("SetSpeed", stats.projectileSpeed);

    }

    public void GetStats(EnemyStats stats)
    {
        this.stats = stats;
    }
}
