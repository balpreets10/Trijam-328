using System.Collections.Generic;

using UnityEngine;

public interface ILevelBuilder
{
    void GenerateLevel(int levelNumber);
    void ClearLevel();
    event System.Action<int> OnLevelGenerated;
}



public interface ILevelDataProvider
{
    LevelData GetLevelData(int levelNumber);
    void SetCurrentLevelData(LevelData levelData);
}

public interface ILevelGenerator
{
    List<ObstacleData> GenerateObstacles(int levelNumber, LevelData levelData);
}


