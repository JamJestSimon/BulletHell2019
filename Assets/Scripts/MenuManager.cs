using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    Camera camera;

    Slider volumeSlider;
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void Start()
    {
        volumeSlider = GameObject.FindGameObjectWithTag("AudioSlider").GetComponent<Slider>();
    }

    void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }

    public void ShowAudioCanvas()
    {
        camera.GetComponent<MenuCameraBehaviour>().SendMessage("SetRotationY", 90f);
    }
    public void ShowSettingsCanvas()
    {
        camera.GetComponent<MenuCameraBehaviour>().SendMessage("SetRotationY", 270f);
    }

    public void ShowMainCanvas()
    {
        camera.GetComponent<MenuCameraBehaviour>().SendMessage("SetRotationY", .0001f);
    }
}
