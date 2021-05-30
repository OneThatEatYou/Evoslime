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

        monsterControls.onHealthChangedHandler += hudManager.SetHealthSlider;
        monsterControls.onFoodConsumedHandler += hudManager.SetFoodSlider;
        monsterControls.onFoodConsumedBoolHandler += hudManager.SetEvolutionPrompt;
        playerInput.Player.Attack.performed += contex => monsterControls.Attack(movementInput);
        playerInput.Player.Evolve.performed += contex => monsterControls.Evolve();
        monsterControls.onEvolvedHandler += SwitchPlayerMonster;
    }

    private void OnDisable()
    {
        playerInput.Disable();

        monsterControls.onHealthChangedHandler -= hudManager.SetHealthSlider;
        monsterControls.onFoodConsumedHandler -= hudManager.SetFoodSlider;
        monsterControls.onFoodConsumedBoolHandler -= hudManager.SetEvolutionPrompt;
        playerInput.Player.Attack.performed -= contex => monsterControls.Attack(movementInput);
        playerInput.Player.Evolve.performed -= contex => monsterControls.Evolve();
        monsterControls.onEvolvedHandler -= SwitchPlayerMonster;
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
}
