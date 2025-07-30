using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;



    public void SetHealth(float current, float max)
    {
        if (slider != null)
            slider.value = current / max;
        Debug.Log($"Health set: {current}/{max}");
    }

}
