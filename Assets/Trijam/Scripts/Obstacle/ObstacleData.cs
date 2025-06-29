using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class ObstacleData
{
    public ObstacleType type;
    public Vector3 position; // Relative to pattern start
    public float destructionValue;
    public float width = 1f;
    public float height = 1f;
    public bool isDestructible = true;
    public Color obstacleColor = Color.red;

    public bool isFinishLine => type == ObstacleType.FinishLine;
}

[System.Serializable]
public enum ObstacleType
{
    Wall, Barrier, Explosive, MovingBlock, Spike, PowerUp, HealthOrb, FinishLine // Add this new type
}

