using System.Collections.Generic;

using UnityEngine;

public class LevelBuilder : ILevelBuilder
{
    private readonly ILevelDataProvider levelDataProvider;
    private readonly ILevelGenerator levelGenerator;
    private readonly IObstacleSpawner obstacleSpawner;
    private readonly LevelSystemConfig config;

    public event System.Action<int> OnLevelGenerated;

    public LevelBuilder(
        ILevelDataProvider levelDataProvider,
        ILevelGenerator levelGenerator,
        IObstacleSpawner obstacleSpawner,
        LevelSystemConfig config)
    {
        this.levelDataProvider = levelDataProvider;
        this.levelGenerator = levelGenerator;
        this.obstacleSpawner = obstacleSpawner;
        this.config = config;
    }

    public void GenerateLevel(int levelNumber)
    {
        ClearLevel();

        LevelData levelData = levelDataProvider.GetLevelData(levelNumber);
        levelDataProvider.SetCurrentLevelData(levelData);

        List<ObstacleData> obstacles = levelGenerator.GenerateObstacles(levelNumber, levelData);

        Transform levelParent = GetLevelParent();
        Transform objectHolder = GetPooledObjectHolder();
        obstacleSpawner.SpawnObstacles(obstacles, levelParent);

        OnLevelGenerated?.Invoke(levelNumber);

        Debug.Log($"Generated level {levelNumber} with {obstacles.Count} obstacles");
    }

    public void ClearLevel()
    {
        obstacleSpawner.ClearAllObstacles();
    }

    private Transform GetLevelParent()
    {
        GameObject parentObj = GameObject.Find(config.levelParentName);
        if (parentObj == null)
        {
            parentObj = new GameObject(config.levelParentName);
        }
        return parentObj.transform;
    }
    private Transform GetPooledObjectHolder()
    {
        GameObject parentObj = GameObject.Find(config.PooledObjectsHolderName);
        if (parentObj == null)
        {
            parentObj = new GameObject(config.PooledObjectsHolderName);
        }
        return parentObj.transform;
    }


}