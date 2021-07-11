using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverCanvasController : MonoBehaviour
{
    GameObject gameController;

    [SerializeField]
    Text text;

    private void OnEnable()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        text.text = "YOU LOST AT level: " + (gameController.GetComponent<GameController>().level+1) + " frags: " + gameController.GetComponent<GameController>().enemiesKilled;

    }

    public void ToMenu()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        gameController.SendMessage("LoadMenu");
    }

    public void ToDesktop()
    {
        Application.Quit();
    }
}

