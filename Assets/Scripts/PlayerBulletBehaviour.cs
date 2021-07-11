using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletBehaviour : MonoBehaviour
{
    float speed;
    float damage;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(Vector3.forward * speed * Time.deltaTime);
        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, transform.TransformDirection(Vector3.forward), out hit, gameObject.transform.lossyScale.z + 0.1f))
        {
            if (hit.collider.gameObject.tag == "Wall")
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Shooting Enemy" || other.gameObject.tag == "Support Enemy" || other.gameObject.tag == "Melee Enemy" || other.gameObject.tag == "Boss")
        {
            other.gameObject.SendMessage("Damage", damage);
            Destroy(gameObject);
        }
    }

    void SetDamage(float x)
    {
        damage = x;
    }

    void SetSpeed(float x)
    {
        speed = x;
    }
}
