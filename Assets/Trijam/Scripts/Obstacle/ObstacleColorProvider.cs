using UnityEngine;

public class ObstacleColorProvider : IObstacleColorProvider
{
    private readonly ObstacleColorConfig _config;

    public ObstacleColorProvider(ObstacleColorConfig config)
    {
        _config = config;
    }

    public Color GetFinishLineColor()
    {
        return _config.finishColor;
    }

    public Color GetObstacleColor(float destructionValue, float recommendedForce)
    {
        return _config.GetObstacleColor(destructionValue, recommendedForce);
    }

}