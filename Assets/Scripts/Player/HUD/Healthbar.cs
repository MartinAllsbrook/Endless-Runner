using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] RectTransform healthFillTransform;
    [SerializeField] Image healthFillImage;
    [SerializeField] Gradient healthGradient;

    float fill = 1f;

    void Awake()
    {
        UpdateHealthBar();
    }

    public void SetFill(float value)
    {
        fill = Mathf.Clamp01(value);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        healthFillTransform.localScale = new Vector3(1f, fill, 1f);
        healthFillImage.color = healthGradient.Evaluate(fill);
    }
}
