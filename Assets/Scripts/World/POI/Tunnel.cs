using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tunnel : MonoBehaviour
{
    [SerializeField] Transform tunnelExitPoint;

    public async Task EnterTunnel()
    {
        await GameManager.Instance.EnterTunnel();

        // Match Player.Instance transform to tunnelExitPoint transform
        if (Player.Instance != null)
        {
            Player.Instance.SetTransform(Vector3.zero, Quaternion.identity);
            Player.Instance.GetComponent<Health>().ResetHealth();
        }
    }
}