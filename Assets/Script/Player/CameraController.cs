using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;
    [SerializeField] private float xSenstivity = 20f;
    [SerializeField] private float ySenstivity = 20f;

    void Update()
    {
        ProcessLook(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
    }

    public void ProcessLook(Vector2 Input)
    {
        float mouseX = Input.x;
        float mouseY = Input.y;

        //calculate rotation
        xRotation -= (mouseY * Time.deltaTime) * ySenstivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        //rotate the camera
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //rotate the player
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSenstivity);

    }
}
