using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    float zoomSpeed = 1f, targetZoom = 10f, zoom = 10f;

    void Start()
    {
        GameObject[] possiblePlayers = FindObjectsOfType<GameObject>();
        for(int i = 0; i < possiblePlayers.Length; i++)
        {
            if(possiblePlayers[i].tag == "Player")
            {
                player = possiblePlayers[i];
                break;
            }
        }
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 10f, player.transform.position.z);
    }

    void Update()
    {
        targetZoom = player.transform.position.y + 10f + player.GetComponent<Rigidbody>().velocity.magnitude/2f;
        if (Input.GetKey(KeyCode.E)) targetZoom = 100f;
        zoom += (targetZoom - zoom) / 10;
        transform.position = new Vector3(player.transform.position.x, zoom, player.transform.position.z);
    }
}
