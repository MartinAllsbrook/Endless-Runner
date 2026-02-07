using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

class TunnelPOI : PointOfInterest
{
    [SerializeField] Transform tunnelExitPoint;

    async void EnterTunnel()
    {
        await GameManager.Instance.EnterTunnel();

        // Match Player.Instance transform to tunnelExitPoint transform
        if (Player.Instance != null)
        {
            Player.Instance.SetTransform(Vector3.zero, Quaternion.identity);
            Player.Instance.GetComponent<Health>().ResetHealth();
            Player.Instance.GetComponent<CarMovement>().Refuel();
        }
    }

    protected override void OnPlayerEnter()
    {
        EnterTunnel();
    }
}