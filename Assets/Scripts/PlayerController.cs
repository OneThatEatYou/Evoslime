using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public MonsterData monsterData;

    Vector2 movementInput;

    PlayerInputAction playerInput;
    Rigidbody2D rb;
    Animator anim;

    private void Awake()
    {
        playerInput = new PlayerInputAction();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
        Move(movementInput);
    }

    void Move(Vector2 input)
    {
        rb.velocity = input * monsterData.moveSpeed * Time.fixedDeltaTime;
        anim.SetFloat("Direction", input.x);
        anim.SetFloat("Speed", rb.velocity.magnitude);
    }
}
