using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    // [SerializeField] private TextMeshProUGUI promptText; // KOMENTAR DULU
    [SerializeField] private Image promptIcon;

    public void UpdatePrompt(string promptMessage, Sprite icon)
    {
        bool show = icon != null;

        // if (promptText != null)
        // {
        //     promptText.gameObject.SetActive(false); // KOMENTAR DULU
        // }

        if (promptIcon != null)
        {
            promptIcon.gameObject.SetActive(show);
            promptIcon.sprite = icon;
        }
    }
}
