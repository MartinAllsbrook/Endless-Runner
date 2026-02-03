using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class TunnelUI : MonoBehaviour
{
    [SerializeField] Button closeButton;

    void Start()
    {
        closeButton.onClick.AddListener(CloseTunnelUI);
    }

    async void CloseTunnelUI()
    {
        Debug.Log("Closing Tunnel UI");
        await GameManager.Instance.ExitTunnel();
    }
}