using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] FillBar healthBar;
    [SerializeField] FillBar fuelBar;
    [SerializeField] TextMeshProUGUI scrapCount;
    [SerializeField] TextMeshProUGUI speedText;

    Health playerHealth;
    CarMovement carMovement;
    Inventory inventory;

    void Start()
    {
        carMovement = Player.Instance.GetComponent<CarMovement>();
        playerHealth = Player.Instance.GetComponent<Health>();
        playerHealth.OnHealthChangedPercent += healthBar.SetFill;   

        inventory = Player.Instance.GetComponent<Inventory>();
        inventory.OnScrapChanged += UpdateScrapCount;
        UpdateScrapCount(0);
    }

    void OnDisable()
    {
        playerHealth.OnHealthChangedPercent -= healthBar.SetFill;   
    }

    void UpdateScrapCount(int count)
    {
        scrapCount.text = count.ToString() + " Scrap";
    }

    void Update()
    {
        // Update fuel bar
        float fuelPercent = carMovement.GetFuelPercent();
        fuelBar.SetFill(fuelPercent);

        // Update speed display
        float speed = carMovement.GetCurrentSpeedKPH();
        speedText.text = $"{speed:0} km/h";
    }
}
