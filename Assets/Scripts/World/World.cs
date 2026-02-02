using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance;

    [SerializeField] private GameObject genericEnvironmentPrefab;
    [SerializeField] private GameObject genericEnemyPrefab;

    private float lastSpawnY = float.MinValue;
    private float lastEnemySpawnY = float.MinValue;
    private float spawnDistance = 5f;
    private float enemySpawnDistance = 8f;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (Player.Instance != null)
        {
            float playerY = Player.Instance.transform.position.y;
            // Spawn environment
            if (playerY - lastSpawnY > spawnDistance)
            {
                Vector3 spawnPos = Player.Instance.transform.position + new Vector3(Random.Range(-2f, 2f), 5f, 0f);
                Instantiate(genericEnvironmentPrefab, spawnPos, Quaternion.identity);
                lastSpawnY = playerY;
            }

            // Spawn enemy
            if (playerY - lastEnemySpawnY > enemySpawnDistance)
            {
                Vector3 enemySpawnPos = Player.Instance.transform.position + new Vector3(Random.Range(-2f, 2f), 7f, 0f);
                Instantiate(genericEnemyPrefab, enemySpawnPos, Quaternion.identity);
                lastEnemySpawnY = playerY;
            }
        }
    }
}
