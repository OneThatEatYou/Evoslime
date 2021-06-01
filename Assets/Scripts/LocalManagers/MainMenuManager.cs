using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour
{
    public InputAction playAction;
    public InputAction exitAction;

    const string mainSceneName = "GlobalScene";

    private void Start()
    {
        playAction.performed += contex => StartGame();
        exitAction.performed += contex => ExitGame();
    }

    void OnEnable()
    {
        playAction.Enable();
        exitAction.Enable();
    }

    void OnDisable()
    {
        playAction.Disable();
        exitAction.Disable();
    }


    void StartGame()
    {
        Debug.Log("Loading " + mainSceneName);
        GameManager.Instance.ChangeScene(mainSceneName);
    }

    void ExitGame()
    {
        Debug.Log("Exiting application");
        Application.Quit();
    }
}
