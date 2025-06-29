using System.Collections.Generic;

using UnityEngine;

public class ObstacleSpawner : IObstacleSpawner
{
    private readonly IObstacleFactory obstacleFactory;
    private Transform levelParent;

    public ObstacleSpawner(IObstacleFactory obstacleFactory)
    {
        this.obstacleFactory = obstacleFactory;
    }

    public void SpawnObstacles(List<ObstacleData> obstacles, Transform parent)
    {
        levelParent = parent;

        foreach (var obstacleData in obstacles)
        {
            GameObject obstacle = obstacleFactory.CreateObstacle(obstacleData, obstacleData.position);
            if (obstacle != null && levelParent != null)
            {
                obstacle.transform.SetParent(levelParent);
            }
        }
    }

    public void ClearAllObstacles()
    {
        obstacleFactory.ClearAllObstacles();
    }
}

public interface IObstacleSpawner
{
    void SpawnObstacles(List<ObstacleData> obstacles, Transform parent);

    void ClearAllObstacles();

}

