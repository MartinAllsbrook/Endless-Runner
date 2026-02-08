using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action OnTunnelEnteredAndLoaded = delegate { };
    public static event Action OnTunnelExitedAndWorldLoaded = delegate { };
    public static event Action OnWorldLoaded = delegate { };
    public static event Action BeforeWorldClosed = delegate { };

    [SerializeField] Camera backupCamera;

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
        await SceneManager.LoadSceneAsync("MainMenuUI", LoadSceneMode.Additive);
        Time.timeScale = 0f;        
    }

    public async Task StartGame()
    {
        await SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        backupCamera.gameObject.SetActive(false);
        await LoadWorld();
        await SceneManager.UnloadSceneAsync("MainMenuUI");
        Time.timeScale = 1f;
    }

    public async Task EndGame()
    {
        Time.timeScale = 0f;
        await SceneManager.LoadSceneAsync("GameOverUI", LoadSceneMode.Additive);
        
        // Only unload World if it's currently loaded
        Scene worldScene = SceneManager.GetSceneByName("World");
        if (worldScene.isLoaded)
        {
            await SceneManager.UnloadSceneAsync("World");
        }
    }

    public async Task RestartGame()
    {
        await SceneManager.UnloadSceneAsync("GameOverUI");
        
        // Unload TunnelUI if player died in tunnel
        Scene tunnelScene = SceneManager.GetSceneByName("TunnelUI");
        if (tunnelScene.isLoaded)
        {
            await SceneManager.UnloadSceneAsync("TunnelUI");
        }
        
        backupCamera.gameObject.SetActive(true);
        await SceneManager.UnloadSceneAsync("Player");
        await SceneManager.LoadSceneAsync("Player", LoadSceneMode.Additive);
        backupCamera.gameObject.SetActive(false);
        await LoadWorld();
        Time.timeScale = 1f;
    }

    #region Tunnel
    public async Task EnterTunnel()
    {
        Time.timeScale = 0f;
        await SceneManager.LoadSceneAsync("TunnelUI", LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync("World");
        OnTunnelEnteredAndLoaded.Invoke();
    }

    public async Task ExitTunnel()
    {
        await SceneManager.UnloadSceneAsync("TunnelUI");
        await LoadWorld();
        OnTunnelExitedAndWorldLoaded.Invoke();
        Time.timeScale = 1f;
    }
    #endregion

    #region World
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

    async Task CloseWorld()
    {
        BeforeWorldClosed.Invoke();
        await SceneManager.UnloadSceneAsync("World");
    }
    #endregion
}
