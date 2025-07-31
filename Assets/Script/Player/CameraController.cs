using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;

    private float xRotation = 0f;
    [SerializeField] private float xSenstivity = 20f;
    [SerializeField] private float ySenstivity = 20f;

    [Header("FOV Effect")]
    public float defaultFov = 85f;
    private float targetFov;
    public float fovTransitionSpeed = 5f;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        targetFov = defaultFov;
        cam.fieldOfView = defaultFov;
    }

    void Update()
    {
        ProcessLook(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));

        // Smooth FOV transition
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * fovTransitionSpeed);
    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        xRotation -= (mouseY * Time.deltaTime) * ySenstivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSenstivity);
    }

    public void SetFov(float fov)
    {
        targetFov = fov;
    }

    public void ResetFov()
    {
        targetFov = defaultFov;
    }

    public Transform GetCameraTransform()
    {
        return cam.transform;
    }
}
