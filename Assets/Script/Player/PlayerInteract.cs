using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float Distance = 3f;
    [SerializeField] private LayerMask Mask;
    private PlayerUI playerUI;

    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        playerUI = GetComponent<PlayerUI>();

    }

    void Update()
    {

        playerUI.UpdatePrompt(string.Empty, null);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * Distance);
        RaycastHit hitinfo;
        if (Physics.Raycast(ray, out hitinfo, Distance, Mask))
        {
            Interactable interactable = hitinfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                // Kirim prompt kosong, hanya fokus ke ikon
                playerUI.UpdatePrompt(string.Empty, interactable.PromptIcon);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.BaseInteract();
                }
            }
        }
        else
        {
            playerUI.UpdatePrompt(string.Empty, null); // Sembunyikan icon jika tidak ada interaksi
        }

    }
}
