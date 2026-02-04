using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPause : MonoBehaviour
{
    private InputAction pauseAction;
    public bool isFreeze;
    public throwScript playerThrow;
    public playercontroler playercontroller;
    public GameObject pauseMenu;
    void Start()
    {
        pauseAction = InputSystem.actions.FindAction("PauseAction");
    }

    // Update is called once per frame
    void Update()
    {
        OnPause();
        if (isFreeze)
        {
            pauseMenu.SetActive(true);
            playerThrow.enabled = false;
            playercontroller.enabled = false;
        }
        else
        {
            pauseMenu.SetActive(false);
            playercontroller.enabled = true;
            playerThrow.enabled = true;
        }


    }

    public void OnPause()
    {
        if (pauseAction.WasPressedThisFrame())
        {
            isFreeze = !isFreeze;
        }
    }
}
