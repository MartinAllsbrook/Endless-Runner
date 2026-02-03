using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tunnel : MonoBehaviour
{
    [SerializeField] Transform tunnelExitPoint;

    public async Task EnterTunnel()
    {
        Debug.Log("Entered Tunnel Trigger");

        Vector3 exitPosition = tunnelExitPoint != null ? tunnelExitPoint.position : Vector3.zero;
        Quaternion exitRotation = tunnelExitPoint != null ? tunnelExitPoint.rotation : Quaternion.identity;
        
        await GameManager.Instance.EnterTunnel();

        // Match Player.Instance transform to tunnelExitPoint transform
        if (Player.Instance != null)
        {
            Debug.Log("Setting Player Position to Tunnel Exit Point");
            Player.Instance.SetTransform(exitPosition, exitRotation);
            Player.Instance.GetComponent<Health>().ResetHealth();
        }
    }
}