using UnityEngine;
using System.Collections;

public class DashingController : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform orientation;
    public Transform playerCam;

    private PlayerLook playerLook;

    [Header("Dashing")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("FOV Effect")]
    public float dashFov = 100f;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;
    public bool useCameraForward = true;
    public bool allowAllDirections = true;

    private bool isDashing = false;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;

    private void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        // Get PlayerLook and camera
        playerLook = GetComponent<PlayerLook>();
        if (playerLook != null)
        {
            playerCam = playerLook.GetCameraTransform();
        }
    }

    private void Update()
    {
        dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(dashKey) && dashCooldownTimer <= 0f && !isDashing)
        {
            dashDirection = GetDashDirection();
            StartCoroutine(PerformDash());
        }
    }

    private Vector3 GetDashDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = Vector3.zero;

        if (allowAllDirections)
            direction = (orientation.right * h + orientation.forward * v).normalized;
        else
            direction = useCameraForward ? playerCam.forward : orientation.forward;

        if (direction == Vector3.zero)
            direction = orientation.forward;

        direction.y = 0f;
        return direction.normalized;
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        if (playerLook != null)
            playerLook.SetFov(dashFov);

        float elapsedTime = 0f;
        while (elapsedTime < dashDuration)
        {
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (playerLook != null)
            playerLook.ResetFov();

        isDashing = false;
    }
}
