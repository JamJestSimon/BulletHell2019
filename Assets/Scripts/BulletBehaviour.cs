using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float speed;
    float damage = 10f;

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
            if(hit.collider.gameObject.tag == "Wall")
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.SendMessage("Damage", damage);
            Destroy(gameObject);
        }
        
    }

    void GetDamage(float x)
    {
        damage = x;
    }

    private void OnDestroy()
    {

    }


}
