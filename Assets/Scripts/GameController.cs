using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    [SerializeField]
    Camera camera;

    MapGen mapGen;
    public int level = -1;
    //archivments
    public int enemiesKilled;


    public List<GameObject> enemies;
    void Awake()
    {
       enemies = new List<GameObject>();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log(enemies.Count);
        }
    }

    public float SetEnemyStatMultiplier(int n)
    {
        return (1f + (n * 0.1f));
    }

    public void AddEnemy(GameObject e)
    {
        enemies.Add(e);
    }

    public void RemoveEnemy(GameObject e)
    {
        for (int i = enemies.Count - 1; i >= 0; i--) if (enemies[i] == e) enemies.RemoveAt(i);
        enemiesKilled++;
    }

    public void StartGame()
    {
        GameObject p = Instantiate(player);
        Camera c = Instantiate(camera);
        mapGen = gameObject.GetComponent<MapGen>();
        mapGen.Generate(SetMapSize(level), SetMapSize(level), 6, 10, .7f, level);
        p.transform.position = new Vector3(mapGen.playerX, 10f, mapGen.playerY);
    }

    public void UpdateDifficulty()
    {
        Slider s;
    }

    private void OnLevelWasLoaded(int index)
    {
        if(index == 1)
        {
            level++;
            StartGame();
        }
    }
    int SetMapSize(int n)
    {
        return (int)(50 + (Mathf.Sqrt(n) * 10));
    }

    public void LoadMenu()
    {
        level = -1;
        enemiesKilled = 0;
        SceneManager.LoadScene("Menu");
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Menu"));
        StartCoroutine(UnloadGame());
        
    }

    IEnumerator UnloadGame()
    {
        //Debug.Log(SceneManager.GetActiveScene());
        AsyncOperation async = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        Debug.Log(async);
        while(async.progress <= 0.9f)
        {
            yield return null;
        }
        yield break;
    }

}
