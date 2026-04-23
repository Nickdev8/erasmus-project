using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() // this makes it so it changes to level 1 when you pres the button (Alicia)
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame() // this makes it so the game gose away oooooooo (Alicia)
    {
        Application.Quit();
    }
}
