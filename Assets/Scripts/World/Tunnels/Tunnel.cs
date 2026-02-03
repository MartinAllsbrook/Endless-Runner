using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tunnel : MonoBehaviour
{
    [SerializeField] Transform tunnelExitPoint;

    public void EnterTunnel()
    {
        Debug.Log("Entered Tunnel Trigger");
        
        SceneManager.LoadSceneAsync("TunnelUI", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("World");

        // Match Player.Instance transform to tunnelExitPoint transform
        if (Player.Instance != null && tunnelExitPoint != null)
        {
            Player.Instance.SetTransform(tunnelExitPoint.position, tunnelExitPoint.rotation);
            Player.Instance.GetComponent<Health>().ResetHealth();
        }
    }
}