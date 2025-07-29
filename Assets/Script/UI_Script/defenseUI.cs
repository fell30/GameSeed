
// using UnityEngine;
// using UnityEngine.UI;

// public class defenseUI : MonoBehaviour
// {
//     [SerializeField] private DefenRepair defenRepair;
//     [SerializeField] Image sliderImage;
//     [SerializeField] private bool faceCamera = true;
//     [SerializeField] private Vector3 offset = new Vector3(0f, 01, 0f);
//     private Camera mainCamera;

//     private void Start()
//     {
//         mainCamera = Camera.main;
//         if (defenRepair == null || sliderImage == null)
//         {
//             enabled = false;
//             return;
//         }
//         if (sliderImage.type != Image.Type.Filled)
//         {

//             sliderImage.type = Image.Type.Filled;
//         }
//     }

//     private void Update()
//     {
//         if (defenRepair.IsActive)
//         {
//             float timeLeft = defenRepair.GetRemainingTime;
//             sliderImage.fillAmount = timeLeft / 30f;
//             sliderImage.enabled = true;
//         }
//         else
//         {
//             sliderImage.fillAmount = 0f;
//             sliderImage.enabled = false; // Sembunyikan gambar saat rusak
//         }


//         if (faceCamera && mainCamera != null)
//         {
//             transform.rotation = mainCamera.transform.rotation;
//         }


//         if (defenRepair != null)
//         {
//             transform.position = defenRepair.transform.position + offset;
//         }
//     }
// }
