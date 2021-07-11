using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraBehaviour : MonoBehaviour
{

    public float targetRotY = 0f, targetRotX = 0f;
    void Start()
    {

    }

    void Update()
    {


        float diffY = targetRotY - gameObject.transform.rotation.eulerAngles.y;
        float rotY = gameObject.transform.rotation.eulerAngles.y + ((diffY+540f)%360f-180f) / 20f;
        

        gameObject.transform.rotation = Quaternion.Euler(0f, rotY, 0f);
    }

    public void SetRotationX(float rotX)
    {
        targetRotX = rotX;
    }
    public void SetRotationY(float rotY)
    {
        targetRotY = rotY;
    }
}