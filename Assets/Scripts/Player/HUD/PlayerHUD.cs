using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance { get; private set; }

    [SerializeField] FillBar healthBar;
    [SerializeField] FillBar fuelBar;
    [SerializeField] TextMeshProUGUI scrapCount;
    [SerializeField] TextMeshProUGUI speedText;

    Health playerHealth;
    CarMovement carMovement;
    Inventory inventory;

    InteractionUI interactionUI;
    public static event Action OnExitInteraction;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

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

    public void EnterInteraction(InteractionUI interactionUIPrefab)
    {
        interactionUI = Instantiate(interactionUIPrefab, transform);
        interactionUI.OnExit += ExitInteraction;
    }   

    public void ExitInteraction()
    {
        if (interactionUI != null)
        {
            interactionUI.OnExit -= ExitInteraction;
            Destroy(interactionUI.gameObject);
        }

        OnExitInteraction?.Invoke();
    }
}