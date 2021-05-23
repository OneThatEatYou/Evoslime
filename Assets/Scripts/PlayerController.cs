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

    private void Awake()
    {
        playerInput = new PlayerInputAction();
        monsterControls = GetComponent<MonsterControls>();
        hudManager = FindObjectOfType<HUDManager>();

        // set up HUD
        hudManager.Init(monsterControls.monsterData);
        monsterControls.OnHealthChangedHandler += hudManager.SetHealthSlider;
        monsterControls.OnEatFoodHandler += hudManager.SetFoodSlider;

        if (!monsterControls)
        {
            Debug.LogError($"No monster controls found in {name}");
        }
    }

    private void Start()
    {
        playerInput.Player.Attack.performed += contex => monsterControls.Attack(movementInput);
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        movementInput = playerInput.Player.Movement.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        monsterControls.Move(movementInput);
    }
}
