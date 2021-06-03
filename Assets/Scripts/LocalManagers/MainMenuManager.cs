using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public InputAction playAction;
    public InputAction exitAction;

    [Space(20)]

    public TextMeshProUGUI versionText;

    private void Start()
    {
        UpdateVersionText();
    }

    void OnEnable()
    {
        playAction.Enable();
        exitAction.Enable();

        playAction.performed += contex => StartGame();
        exitAction.performed += contex => ExitGame();
    }

    void OnDisable()
    {
        playAction.Disable();
        exitAction.Disable();

        playAction.performed -= contex => StartGame();
        exitAction.performed -= contex => ExitGame();
    }


    void StartGame()
    {
        AudioManager.PlayAudioAtPosition(AudioManager.buttonYesSFX, transform.position, AudioManager.sfxMixerGroup, true, 0);
        GameManager.Instance.ChangeScene(GameManager.mainSceneName);
    }

    void ExitGame()
    {
        Debug.Log("Exiting application");
        Application.Quit();
    }

    void UpdateVersionText()
    {
        versionText.text = "v" + Application.version;
    }
}
