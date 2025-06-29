using System.Collections.Generic;

using UnityEngine;
[System.Serializable]
public class ObstaclePattern
{
    public string patternName;
    public List<ObstacleData> obstacles;
    public float patternLength;
    public float spawnProbability;
    public int minLevelRequired;
}
