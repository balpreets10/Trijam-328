using UnityEngine;

public interface IObstacleColorProvider
{
    Color GetFinishLineColor();
    Color GetObstacleColor(float destructionValue, float recommendedForce);
}
