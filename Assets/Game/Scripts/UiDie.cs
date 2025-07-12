using UnityEngine;
using UnityEngine.SceneManagement;

public class UiDie : MonoBehaviour
{
    private GameObject dieMenuPanel;

    void Start()
    {
        // Cari DieMenuPanel sebagai child
        dieMenuPanel = transform.Find("DieMenuPanel")?.gameObject;

    }

    // Tampilkan die menu
    public void ShowDieMenu()
    {

        dieMenuPanel.SetActive(true);
    }

    // Restart level
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Quit game
    public void QuitGame()
    {
        Application.Quit();
    }
}
