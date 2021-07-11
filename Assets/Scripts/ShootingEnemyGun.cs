using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemyGun : MonoBehaviour
{
    public GameObject target;
    [SerializeField]
    GameObject bullet;
    BulletBehaviour bulletData;
    float timePassed;
    EnemyStats stats;

    [SerializeField]
    AudioClip AttackSound;

    AudioSource audio;
    void Start()
    {
        timePassed = 0;
        bulletData = bullet.GetComponent<BulletBehaviour>();
        audio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        gameObject.transform.LookAt(target.transform);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, stats.range))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                if (timePassed >= 1f / stats.fireRate)
                {
                    timePassed = 0;
                    Fire();
                }
            }
        }
    }

    void Fire()
    {
        GameObject firedBullet = Instantiate(bullet, gameObject.transform.position, gameObject.transform.rotation);
        Destroy(firedBullet, stats.range / bulletData.speed * 10f);
        audio.PlayOneShot(AttackSound);
    }
    
    public void GetStats(EnemyStats stats)
    {
        this.stats = stats;
    }
}
