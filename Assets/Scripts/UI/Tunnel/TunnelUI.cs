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

    void CloseTunnelUI()
    {
        Debug.Log("Closing Tunnel UI");
        SceneManager.UnloadSceneAsync("TunnelUI");
        SceneManager.LoadSceneAsync("World", LoadSceneMode.Additive);
    }
}