using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelInMenuBehaviour : MonoBehaviour
{
    float y, x, z, rotY, offY, time = 0f;
    void Start()
    {
        offY = gameObject.transform.position.y;
        x = gameObject.transform.position.x;
        z = gameObject.transform.position.z;
        rotY = 180f * UnityEngine.Random.value;
        time =  10f * UnityEngine.Random.value;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(x,offY + Mathf.Sin(time)*0.4f,z);
        gameObject.transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        time += Time.deltaTime;
        rotY += 60f * Time.deltaTime;
    }
}
