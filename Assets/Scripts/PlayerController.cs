using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Vector2 movementInput;

    MonsterControls monsterControls;
    PlayerInputAction playerInput;
    HUDManager hudManager;
    CameraController cameraController;

    private void Awake()
    {
        playerInput = new PlayerInputAction();
        monsterControls = GetComponent<MonsterControls>();
        hudManager = FindObjectOfType<HUDManager>();

        // set up HUD
        hudManager.Init(monsterControls.monsterData);

        cameraController = FindObjectOfType<CameraController>();
        cameraController.target = transform;

        if (!monsterControls)
        {
            Debug.LogError($"No monster controls found in {name}");
        }
    }

    private void OnEnable()
    {
        playerInput.Enable();

        playerInput.Player.Attack.performed += contex => monsterControls.Attack(movementInput);
        playerInput.Player.Evolve.performed += contex => monsterControls.Evolve();

        monsterControls.onHealthChangedHandler += hudManager.SetHealthSlider;
        monsterControls.onFoodConsumedHandler += hudManager.SetFoodSlider;
        monsterControls.onFoodConsumedBoolHandler += hudManager.SetEvolutionPrompt;
        monsterControls.onEvolvedHandler += SwitchPlayerMonster;
        monsterControls.onEvolvedHandler += GameManager.Instance.ReportPlayerEvolution;

        // not unreferenced on disabled or else it wont be called when destroyed
        monsterControls.onDeathHandler += GameOver;
    }

    private void OnDisable()
    {
        playerInput.Disable();

        playerInput.Player.Attack.performed -= contex => monsterControls.Attack(movementInput);
        playerInput.Player.Evolve.performed -= contex => monsterControls.Evolve();

        monsterControls.onHealthChangedHandler -= hudManager.SetHealthSlider;
        monsterControls.onFoodConsumedHandler -= hudManager.SetFoodSlider;
        monsterControls.onFoodConsumedBoolHandler -= hudManager.SetEvolutionPrompt;
        monsterControls.onEvolvedHandler -= SwitchPlayerMonster;
        monsterControls.onEvolvedHandler -= GameManager.Instance.ReportPlayerEvolution;
    }

    private void Update()
    {
        movementInput = playerInput.Player.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        monsterControls.Move(movementInput);
    }

    void SwitchPlayerMonster(MonsterData newMonsterData)
    {
        GameObject newPlayer = GameManager.Instance.SpawnPlayer("GlobalScene", newMonsterData);
        newPlayer.transform.position = transform.position;
        Destroy(gameObject);

        Debug.Log("Switched player");
    }

    void GameOver()
    {
        Debug.Log("Game Over");
        hudManager.ShowGameOverPanel();
        Time.timeScale = 0;

        GameManager.Instance.ReportPlayerGameOver(monsterControls.monsterData);
    }
}
