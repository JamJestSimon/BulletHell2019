using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadBossBehaviour : MonoBehaviour
{
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //TODO NEW MAP
            Debug.Log("NEW MAP");
            SceneManager.LoadScene("Game");
            Destroy(gameObject);
        }
    }
}
