using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Pool;

public class EnemyObjectPool : MonoBehaviour
{
    //public IObjectPool<Obstacle> ObstaclePool;
    //public List<GameObject> Obstacles;

    //private List<Obstacle> activeObstacles = new List<Obstacle>();
    //public bool collectionCheck = true;
    //public int defaultCapacity = 5;
    //public int maxPoolSize = 20;

    //private void Awake()
    //{
    //    InitializePool();
    //}
    //private void InitializePool()
    //{
    //    ObstaclePool = new ObjectPool<Obstacle>(
    //        CreateObstacle,
    //        OnTakeFromPool,
    //        OnReturnedToPool,
    //        OnDestroyPoolObject,
    //        collectionCheck,
    //        defaultCapacity,
    //        maxPoolSize

    //        );
    //}

    //public Obstacle Get()
    //{
    //    return ObstaclePool.Get();
    //}

    //public void ReturnToPool(Obstacle Obstacle)
    //{
    //    Debug.Log("Returning to Pool", Obstacle.gameObject);
    //    ObstaclePool.Release(Obstacle);
    //}

    //private void OnReturnedToPool(Obstacle Obstacle)
    //{
    //    Debug.Log("Returned to Pool", Obstacle.gameObject);
    //    Obstacle.gameObject.SetActive(false);
    //    activeObstacles.Remove(Obstacle);
    //}

    //private void OnDestroyPoolObject(Obstacle Obstacle)
    //{
    //    Destroy(Obstacle.gameObject);
    //}

    //private void OnTakeFromPool(Obstacle Obstacle)
    //{
    //    Obstacle.gameObject.SetActive(true);
    //    activeObstacles.Add(Obstacle);
    //}

    //Obstacle CreateObstacle()
    //{
    //    Obstacle Obstacle = Instantiate(GetRandomObstacle());
    //    Obstacle.gameObject.SetActive(false);
    //    return Obstacle;
    //}

    //private Obstacle GetRandomObstacle()
    //{
    //    return Obstacles[UnityEngine.Random.Range(0, Obstacles.Count)].GetComponent<Obstacle>();
    //}
}
