using System.Collections.Generic;

using UnityEngine;

public interface IObstacle
{
    bool TryDestroy();
    float GetDestructionForce();
    float GetHealthReductionValue();

    ObstacleType GetObstacleType();

    void SetObstacleType(ObstacleType obstacleType);

    GameObject gameObject { get; set; }
}


