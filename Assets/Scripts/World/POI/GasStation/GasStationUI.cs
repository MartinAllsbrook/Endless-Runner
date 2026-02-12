using UnityEngine;
using UnityEngine.UI;

class GasStationUI : InteractionUI
{
    [SerializeField] Button refuelButton;
    [SerializeField] Button scavengeButton;

    [SerializeField] float refuelRate = 1f; // Fuel units per second

    bool refueling = false;

    CarMovement playerMovement;

    protected override void OnEnable()
    {
        base.OnEnable();

        playerMovement = Player.Instance.GetComponent<CarMovement>();

        refuelButton.onClick.AddListener(Refuel);
        scavengeButton.onClick.AddListener(Scavenge);   
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        refuelButton.onClick.RemoveListener(Refuel);
        scavengeButton.onClick.RemoveListener(Scavenge);
    }

    void Update()
    {
        if (refueling && playerMovement != null)
        {
            playerMovement.AddFuel(refuelRate * Time.deltaTime);
        }
    }

    void Refuel()
    {
        refueling = true;
    }

    void Scavenge()
    {
        if (Player.Instance != null)
        {
            // Implement scavenging logic here, e.g., give the player some random parts or upgrades
            Debug.Log("Scavenged for parts!");
        }
    }
}