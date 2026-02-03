using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action OnTunnelEnteredAndLoaded = delegate { };
    public static event Action OnTunnelExitedAndWorldLoaded = delegate { };
    public static event Action OnWorldLoaded = delegate { };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private async void Start()
    {
        // Optionally load the world at start
        await LoadWorld();
    }

    public async Task EnterTunnel()
    {
        await SceneManager.LoadSceneAsync("TunnelUI", LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync("World");
        OnTunnelEnteredAndLoaded.Invoke();
    }

    public async Task ExitTunnel()
    {
        await SceneManager.UnloadSceneAsync("TunnelUI");
        await LoadWorld();
        OnTunnelExitedAndWorldLoaded.Invoke();
    }

    async Task LoadWorld()
    {
        await SceneManager.LoadSceneAsync("World", LoadSceneMode.Additive);

        float timeout = 5f;
        float elapsed = 0f;
        while (World.Instance == null && elapsed < timeout)
        {
            await System.Threading.Tasks.Task.Yield();
            elapsed += Time.deltaTime;
        }

        if (World.Instance == null)
        {
            Debug.LogError("Failed to load World scene within timeout.");
        }

        Debug.Log("World scene loaded successfully.");
        OnWorldLoaded.Invoke();
    }
}
