using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FillBar : MonoBehaviour
{
    [SerializeField] RectTransform fillTransform;
    [SerializeField] Image fillImage;
    [SerializeField] Gradient fullToEmptyGradient;
    [SerializeField] bool horizontal = false;

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
        if (horizontal)
            fillTransform.localScale = new Vector3(fill, 1f, 1f);
        else
            fillTransform.localScale = new Vector3(1f, fill, 1f);
        
        fillImage.color = fullToEmptyGradient.Evaluate(fill);
    }
}
