using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;

    PlayerInputAction playerInput;

    float pausedTimeScale = 1;
    bool isPaused;

    private void Awake()
    {
        playerInput = new PlayerInputAction();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.UI.Pause.performed += contex => Pause();
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.UI.Pause.performed -= contex => Pause();
    }

    void Pause()
    {
        // cannot pause when time scale is 0
        if (Time.timeScale == 0 && !isPaused)
        { return; }

        if (isPaused)
        {
            // unpause
            pauseMenu.SetActive(false);

            Time.timeScale = pausedTimeScale;
            pausedTimeScale = 1;    // resets paused time scale
        }
        else
        {
            // pause
            pauseMenu.SetActive(true);

            pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        isPaused = !isPaused;
    }

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ChangeScene(GameManager.mainMenuSceneName);
    }
}
