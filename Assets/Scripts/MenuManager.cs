using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // untuk menu berpindah di set sebagai object
    public GameObject menuPanel;
    public GameObject infoPanel;

    // Menu Panel
    void Start()
    {
        menuPanel.SetActive(true);
        infoPanel.SetActive(false);
    }

    public void PlayButton(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void InfoButton()
    {
        menuPanel.SetActive(false);
        infoPanel.SetActive(true);
    }


    public void ExitButton()
    {
        Application.Quit();
    }

    // Info Panel
    public void BackButton()
    {
        menuPanel.SetActive(true);
        infoPanel.SetActive(false);
    }

    public float speed = 2f; // kecepatan gerak awan
    public float leftLimit = -10f; // batas kiri layar
    public float rightStart = 10f; // posisi awal saat kembali ke kanan


}
