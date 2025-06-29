using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    // Components
    private CharacterController controller;
    private AudioSource audioSource;

    // Movement variables
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;

    // Dash variables
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    // Audio
    //  [SerializeField] private AudioClip walkSound;

    private Vector3 playerVelocity;
    private Vector2 currentInput;
    private bool isDashing = false;
    private float dashTime;
    private float lastDash;
    private bool isGrounded;

    void Start()
    {
        // Initialize components
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Validate components
        if (controller == null)
        {
            Debug.LogError("CharacterController not found on Player!");
        }
        if (audioSource == null)
        {
            Debug.LogError("AudioSource not found on Player!");
        }

    }

    void Update()
    {
        // Check if player is grounded
        isGrounded = controller.isGrounded;

        // Handle keyboard input
        HandleInput();

        // Process movement
        ProcessMove(currentInput);

        // Process dash
        ProcessDash();

        // Handle audio playback
        HandleAudio();
    }

    private void HandleInput()
    {
        // Get movement input (WASD or Arrow keys)
        currentInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // Handle jump input (Space key)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void ProcessMove(Vector2 input)
    {
        // Calculate movement direction
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        // Apply movement
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        // Apply gravity
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void ProcessDash()
    {
        // Initiate dash with Left Shift
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && Time.time - lastDash >= dashCooldown)
        {
            isDashing = true;
            dashTime = Time.time;
            lastDash = Time.time;
        }

        // Process dash movement
        if (isDashing)
        {
            controller.Move(transform.forward * dashSpeed * Time.deltaTime);
            if (Time.time - dashTime >= dashDuration)
            {
                isDashing = false;
            }
        }
    }

    private void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    private void HandleAudio()
    {
        // Handle audio during gameplay
        if (Time.timeScale > 0)
        {
            if (isGrounded && currentInput.magnitude > 0.1f && !isDashing)
            {

            }
            else if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
        else
        {
            // Pause audio when game is paused
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }
    }
}
