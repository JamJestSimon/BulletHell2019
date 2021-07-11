using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float speed = 1f,
          hp = 100f,
          maxHp = 400f,
          jumpSpeed = 2f,
          shootingSpeed = 5f;
    float reloadTime = 0f, cameraDiff;
 

    [SerializeField]
    bool isGrounded = false;

    [SerializeField]
    GameObject PlayerBullet;

    [SerializeField]
    Camera camera;

    private Rigidbody rb;
    GameObject cube;
    [SerializeField]
    AudioClip AttackSound, DeathSound;

    AudioSource audio;

    public Slider slider;
    float sliderValue;

    GameObject c;

    void Start()
    {

        c = GameObject.FindGameObjectWithTag("GameOverCanvas");
        c.SetActive(false);
        camera = FindObjectOfType<Camera>();
        rb = GetComponent<Rigidbody>();
        cameraDiff = camera.transform.position.y - gameObject.transform.position.y;

        audio = gameObject.GetComponent<AudioSource>();
        slider = GameObject.FindGameObjectWithTag("HealthSlider").GetComponent<Slider>();
        slider.value = hp/maxHp;
    }

    void Update()
    {
        Vector3 oldVel = rb.velocity;
        oldVel.z = Input.GetAxis("Vertical");
        oldVel.x = Input.GetAxis("Horizontal");
        float mag = (float)Math.Sqrt((float)(oldVel.x * oldVel.x + oldVel.z * oldVel.z))/speed;
        if(mag>0)oldVel.x /= mag;
        if(mag>0)oldVel.z /= mag;

        if (Input.GetKey(KeyCode.Space) && isGrounded) oldVel.y  = jumpSpeed;
        if(hp > 0f && ! Input.GetKey(KeyCode.E))rb.velocity = oldVel;

       float mousePosX = Input.mousePosition.x;
       float  mousePosY = Input.mousePosition.y;
        cameraDiff = camera.transform.position.y - gameObject.transform.position.y;
        Vector3 worldPos = camera.ScreenToWorldPoint(new Vector3(mousePosX, mousePosY, cameraDiff));

       if(hp>0f) gameObject.transform.LookAt(new Vector3(worldPos.x, gameObject.transform.position.y, worldPos.z));
       // cube.transform.position = new Vector3(worldPos.x, gameObject.transform.position.y, worldPos.z);
        if (Input.GetMouseButton(0) && reloadTime <= 0f && (hp > 0f))
        {
            audio.PlayOneShot(AttackSound);
            reloadTime = 1f / shootingSpeed;
            GameObject firedBullet = Instantiate(PlayerBullet, gameObject.transform.position, gameObject.transform.rotation);
            firedBullet.SendMessage("SetSpeed", 30f);
            firedBullet.SendMessage("SetDamage", 10f);
            Destroy(firedBullet, 10f);
        }
        else
        {
            reloadTime -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground") isGrounded = true;
    }
    private void OnCollision(Collision collision)
    {
        if (collision.gameObject.tag == "ground") isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground") isGrounded = false;
    }
    bool died = false;
    void Damage(float value)
    {
        hp -= value;
        slider.value = hp / maxHp;
        if (hp <= 0 && !died)
        {
            died = true;
            audio.PlayOneShot(DeathSound);
            StartCoroutine("Die");
            c.SetActive(true);
        }
    }


    IEnumerator Die()
    {
        yield return new WaitForSeconds(2);
    }
}
