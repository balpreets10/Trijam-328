using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ObstacleFactory : IObstacleFactory
{
    private readonly LevelSystemConfig config;
    private readonly Dictionary<ObstacleType, Queue<GameObject>> obstaclePools;
    private readonly HashSet<GameObject> activeObstacles;
    private readonly int maxPoolSize = 50;
    private Transform parent;
    public ObstacleFactory(LevelSystemConfig config, Transform parent)
    {
        this.config = config;
        this.parent = parent;
        this.obstaclePools = new Dictionary<ObstacleType, Queue<GameObject>>();
        this.activeObstacles = new HashSet<GameObject>();

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (ObstacleType type in System.Enum.GetValues(typeof(ObstacleType)))
        {
            obstaclePools[type] = new Queue<GameObject>();
            PrewarmPool(type, 10); // Prewarm with 10 objects per type
        }
    }

    private void PrewarmPool(ObstacleType type, int count)
    {
        GameObject prefab = GetObstaclePrefab(type);
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = UnityEngine.Object.Instantiate(prefab, parent);
            obj.SetActive(false);
            obstaclePools[type].Enqueue(obj);
        }
    }

    public GameObject CreateObstacle(ObstacleData data, Vector3 position)
    {
        GameObject obstacle = GetPooledObstacle(data.type);
        if (obstacle == null) return null;

        // Configure obstacle
        obstacle.transform.position = position;
        obstacle.transform.rotation = Quaternion.identity;
        obstacle.transform.localScale = new Vector3(data.width, data.height, 1f);

        // Set obstacle properties
        var obstacleComponent = obstacle.GetComponent<Obstacle>();
        if (obstacleComponent == null)
        {
            obstacleComponent = obstacle.AddComponent<Obstacle>();
        }

        obstacleComponent.destructionValue = data.destructionValue;
        obstacleComponent.isDestructible = data.isDestructible;
        obstacleComponent.SetObstacleType(data.type);
        obstacleComponent.Initialize();
        obstacleComponent.UpdateUI();
        // Set visual properties
        var renderer = obstacle.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = data.obstacleColor;
        }

        obstacle.SetActive(true);
        activeObstacles.Add(obstacle);

        return obstacle;
    }

    private GameObject GetPooledObstacle(ObstacleType type)
    {
        if (obstaclePools[type].Count > 0)
        {
            return obstaclePools[type].Dequeue();
        }

        // Pool is empty, create new object if under max pool size
        GameObject prefab = GetObstaclePrefab(type);
        if (prefab != null)
        {
            return UnityEngine.Object.Instantiate(prefab, parent);
        }

        return null;
    }

    public void DestroyObstacle(GameObject obstacle)
    {
        if (obstacle == null || !activeObstacles.Contains(obstacle)) return;

        activeObstacles.Remove(obstacle);
        ReturnToPool(obstacle);
    }

    private void ReturnToPool(GameObject obstacle)
    {
        obstacle.SetActive(false);

        // Determine obstacle type and return to appropriate pool
        var obstacleComponent = obstacle.GetComponent<Obstacle>();
        if (obstacleComponent != null)
        {
            ObstacleType type = DetermineObstacleType(obstacle);

            if (obstaclePools[type].Count < maxPoolSize)
            {
                obstaclePools[type].Enqueue(obstacle);
            }
            else
            {
                // Pool is full, destroy the object
                UnityEngine.Object.Destroy(obstacle);
            }
        }
        else
        {
            UnityEngine.Object.Destroy(obstacle);
        }
    }

    private ObstacleType DetermineObstacleType(GameObject obstacle)
    {
        // Simple approach: check against prefab names or add a component that stores the type
        string name = obstacle.name.Replace("(Clone)", "").Trim();

        for (int i = 0; i < config.obstaclePrefabs.Length; i++)
        {
            if (config.obstaclePrefabs[i] != null && config.obstaclePrefabs[i].name == name)
            {
                return (ObstacleType)(i % System.Enum.GetValues(typeof(ObstacleType)).Length);
            }
        }

        return ObstacleType.Wall; // Default fallback
    }

    private GameObject GetObstaclePrefab(ObstacleType type)
    {
        if (config.obstaclePrefabs == null || config.obstaclePrefabs.Length == 0) return null;

        int index = (int)type % config.obstaclePrefabs.Length;
        return config.obstaclePrefabs[index];
    }

    public void ClearAllObstacles()
    {
        foreach (var obstacle in activeObstacles.ToArray())
        {
            DestroyObstacle(obstacle);
        }
        activeObstacles.Clear();
    }

    // Optional: Method to clear pools completely (useful for scene transitions)
    public void ClearPools()
    {
        ClearAllObstacles();

        foreach (var pool in obstaclePools.Values)
        {
            while (pool.Count > 0)
            {
                var obj = pool.Dequeue();
                if (obj != null)
                {
                    UnityEngine.Object.Destroy(obj);
                }
            }
        }
    }
}

public interface IObstacleFactory
{
    GameObject CreateObstacle(ObstacleData data, Vector3 position);
    void DestroyObstacle(GameObject obstacle);

    void ClearAllObstacles();
}


