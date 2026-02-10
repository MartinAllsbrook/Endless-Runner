using System.Collections;
using UnityEngine;

class GasStationPOI : PointOfInterest
{
    [Header("UIs")]
    [SerializeField] GameObject mainUI;
    [SerializeField] GameObject refuelUI;
    [SerializeField] GameObject scavengeUI;

    [SerializeField] float refuelRate = 1f;
    [SerializeField] FillBar refuelBar;

    private Coroutine refuelCoroutineInstance;

    public void RefuelPlayer()
    {
        if (refuelCoroutineInstance == null)
        {
            refuelCoroutineInstance = StartCoroutine(RefuelCoroutine());
        }
    }

    public void StopRefueling()
    {
        if (refuelCoroutineInstance != null)
        {
            StopCoroutine(refuelCoroutineInstance);
            refuelCoroutineInstance = null;
        }

        CarMovement carMovement = Player.Instance.GetComponent<CarMovement>();
        carMovement.EnableMovement(true);

        mainUI.SetActive(true);
        refuelUI.SetActive(false);
        scavengeUI.SetActive(false);
    }

    IEnumerator RefuelCoroutine()
    {
        mainUI.SetActive(false);
        refuelUI.SetActive(true);

        CarMovement carMovement = Player.Instance.GetComponent<CarMovement>();

        if (carMovement.IsMoving())
        {
            refuelBar.gameObject.SetActive(false);
        }

        // Wait for car to be stopped
        while (carMovement.IsMoving())
        {
            yield return null;
        }

        carMovement.EnableMovement(false);
        refuelBar.gameObject.SetActive(true);

        // Wait until fuel is full
        while (carMovement.GetFuelPercent() < 1f)
        {
            carMovement.AddFuel(refuelRate * Time.deltaTime);
            float fuelPercent = carMovement.GetFuelPercent();
            refuelBar.SetFill(fuelPercent);
            yield return null;
        }

        carMovement.EnableMovement(true);

        mainUI.SetActive(true);
        refuelUI.SetActive(false);
    }


    public void ScavengeForParts()
    {
        if (Player.Instance != null)
        {
            // Implement scavenging logic here, e.g., give the player some random parts or upgrades
            Debug.Log("Scavenged for parts!");
        }
    }
}