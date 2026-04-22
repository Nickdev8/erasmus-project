using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public bool isPaused;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }
    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
    }
}
