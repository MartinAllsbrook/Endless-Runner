using UnityEngine;

class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [SerializeField] Enemy enemyPrefab;
    [SerializeField] int enemyPoolSize = 30;

    ObjectPool<Enemy> enemyPool; 

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Initialize enemy pool
        enemyPool = new ObjectPool<Enemy>(enemyPrefab, enemyPoolSize, this.transform);
    }

    public Enemy SpawnEnemyAt(Vector2 position)
    {
        return enemyPool.Get(position);
    }
}