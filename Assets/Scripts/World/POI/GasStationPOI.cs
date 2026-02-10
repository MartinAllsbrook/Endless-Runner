using UnityEngine;

class GasStationPOI : PointOfInterest
{
    public void RefuelPlayer()
    {
        if (Player.Instance != null)
        {
            CarMovement carMovement = Player.Instance.GetComponent<CarMovement>();
            if (carMovement != null)
            {
                carMovement.Refuel();
            }
        }
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