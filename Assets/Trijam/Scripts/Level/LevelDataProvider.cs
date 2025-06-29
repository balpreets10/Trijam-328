using UnityEngine;

public class LevelDataProvider : ILevelDataProvider
{
    private readonly LevelSystemConfig config;
    private LevelData currentLevelData;

    public LevelDataProvider(LevelSystemConfig config)
    {
        this.config = config;
        this.currentLevelData = config.defaultLevelData;
    }

    public LevelData GetLevelData(int levelNumber)
    {
        // Try to find specific level data
        foreach (var levelData in config.levelDataAssets)
        {
            if (levelData.levelNumber == levelNumber)
            {
                return levelData;
            }
        }

        // Return default with updated level number
        var defaultCopy = UnityEngine.Object.Instantiate(config.defaultLevelData);
        defaultCopy.levelNumber = levelNumber;
        defaultCopy.levelName = $"Level {levelNumber}";
        return defaultCopy;
    }

    public void SetCurrentLevelData(LevelData levelData)
    {
        currentLevelData = levelData;
    }
}