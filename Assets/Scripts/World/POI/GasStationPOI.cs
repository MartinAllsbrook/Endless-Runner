using UnityEngine;

class GasStationPOI : PointOfInterest
{
    protected override void OnPlayerEnter()
    {
        // Refuel the player's car
        if (Player.Instance != null)
        {
            CarMovement carMovement = Player.Instance.GetComponent<CarMovement>();
            if (carMovement != null)
            {
                carMovement.Refuel();
            }
        }
    }
}