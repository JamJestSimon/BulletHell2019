using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    float angle;
    float timePassed;
    float speed;
    float gravityAcceleration = 10f;
    float zSpeed;
    float ySpeed;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.rotation = new Quaternion(0f, gameObject.transform.rotation.y, 0f, gameObject.transform.rotation.w);
        zSpeed = Mathf.Cos(angle) * speed;
        ySpeed = Mathf.Sin(angle) * speed;


    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0f, ySpeed*Time.deltaTime, zSpeed*Time.deltaTime));
        ySpeed -= Time.deltaTime * 10f;
        if(transform.position.y <= 0)
        {
            Destroy(gameObject);
        }
    }

    void SetAngle(float x)
    {
        angle = x;
    }

    void SetSpeed(float x)
    {
        speed = x;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Support Enemy")
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Collider[] possibleEnemies = Physics.OverlapSphere(gameObject.transform.position, 10f);
        foreach(Collider c in possibleEnemies)
        {
            if (c.gameObject.tag.ToLower().Contains("enemy"))
            {
                if (c.gameObject.GetComponent<SupportEnemyAI>() != null) c.gameObject.GetComponent<SupportEnemyAI>().SendMessage("Boost", 2f);
                if (c.gameObject.GetComponent<ShootingEnemyAI>() != null) c.gameObject.GetComponent<ShootingEnemyAI>().SendMessage("Boost", 2f);
                if (c.gameObject.GetComponent<MeleeEnemyAI>() != null) c.gameObject.GetComponent<MeleeEnemyAI>().SendMessage("Boost", 2f);

            }
        }
    }
}
