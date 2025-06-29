using System.Collections.Generic;

using UnityEngine;

public class LevelGenerator : ILevelGenerator
{
    private IObstacleColorProvider _colorProvider;

    public LevelGenerator(IObstacleColorProvider colorProvider)
    {
        _colorProvider = colorProvider;
    }
    public List<ObstacleData> GenerateObstacles(int levelNumber, LevelData levelData)
    {
        var obstacles = new List<ObstacleData>();

        if (levelData.useDynamicGeneration)
        {
            obstacles.AddRange(GenerateDynamicObstacles(levelNumber, levelData));
        }
        else
        {
            obstacles.AddRange(GenerateFromPatterns(levelData));
        }

        obstacles.Add(CreateFinishLine(levelData.levelLength));

        return obstacles;
    }

    private List<ObstacleData> GenerateDynamicObstacles(int levelNumber, LevelData levelData)
    {
        var obstacles = new List<ObstacleData>();
        var formula = levelData.generationFormula;

        float difficulty = formula.CalculateDifficulty(levelNumber);
        float obstacleDensity = formula.CalculateObstacleDensity(levelNumber);
        float recommendedForce = CalculateRecommendedForce(levelData, formula);

        Debug.Log($"Generating Level {levelNumber} - Difficulty: {difficulty:F2}, Density: {obstacleDensity:F3}");

        // Use fixed segment size for more consistent distribution
        float segmentLength = 8f; // Consistent segment size
        float currentZ = 2f; // Start obstacles closer to player spawn

        while (currentZ < levelData.levelLength)
        {
            // Guarantee obstacles in early segments to avoid empty starts
            bool forceObstacle = (currentZ < 30f); // Force obstacles in first 30 units

            if (forceObstacle || UnityEngine.Random.value < obstacleDensity)
            {
                obstacles.AddRange(GenerateObstacleSegment(currentZ, segmentLength, levelNumber, difficulty, recommendedForce, levelData));
            }

            currentZ += segmentLength;
        }

        return obstacles;
    }

    private List<ObstacleData> GenerateObstacleSegment(float startZ, float length, int level, float difficulty, float recommendedForce, LevelData levelData)
    {
        var obstacles = new List<ObstacleData>();
        ObstaclePattern pattern = SelectObstaclePattern(level, difficulty, levelData);

        if (pattern == null)
        {
            obstacles.AddRange(GenerateRandomObstacles(startZ, length, level, difficulty, recommendedForce, levelData));
        }
        else
        {
            obstacles.AddRange(GenerateFromPattern(pattern, startZ));
        }

        return obstacles;
    }

    //private List<ObstacleData> GenerateRandomObstacles(float startZ, float length, int level, float difficulty, float recommendedForce, LevelData levelData)
    //{
    //    var obstacles = new List<ObstacleData>();
    //    // Ensure minimum obstacle count per segment for consistent distribution
    //    int baseObstacleCount = Mathf.Max(1, Mathf.RoundToInt(length * 0.2f)); // At least 1 obstacle per segment
    //    int difficultyBonus = Mathf.RoundToInt(difficulty * 0.5f);
    //    int obstacleCount = baseObstacleCount + difficultyBonus;

    //    // Distribute obstacles evenly within the segment
    //    for (int i = 0; i < obstacleCount; i++)
    //    {
    //        // Use more even distribution instead of pure random
    //        float segmentProgress = (i + 0.5f) / obstacleCount; // Center obstacles in subsegments
    //        float randomOffset = UnityEngine.Random.Range(-0.3f, 0.3f) * (length / obstacleCount);
    //        float z = startZ + (segmentProgress * length) + randomOffset;

    //        // Clamp to segment bounds
    //        z = Mathf.Clamp(z, startZ, startZ + length);

    //        float x = UnityEngine.Random.Range(-4f, 4f);

    //        ObstacleData obstacleData = CreateRandomObstacle(level, difficulty, recommendedForce, levelData);
    //        obstacleData.position = new Vector3(x, 0, z);
    //        obstacles.Add(obstacleData);
    //    }

    //    return obstacles;
    //}

    private List<ObstacleData> GenerateRandomObstacles(float startZ, float length, int level, float difficulty, float recommendedForce, LevelData levelData)
    {
        var obstacles = new List<ObstacleData>();

        // Generate rows of obstacles instead of random placement
        float rowSpacing = 20f; // Distance between rows
        int rowCount = Mathf.Max(1, Mathf.RoundToInt(length / rowSpacing));

        for (int row = 0; row < rowCount; row++)
        {
            float rowZ = startZ + (row * rowSpacing);
            if (rowZ >= startZ + length) break;

            obstacles.AddRange(GenerateObstacleRow(rowZ, level, difficulty, recommendedForce, levelData));
        }

        return obstacles;
    }

    private List<ObstacleData> GenerateObstacleRow(float zPosition, int level, float difficulty, float recommendedForce, LevelData levelData)
    {
        var obstacles = new List<ObstacleData>();

        // Define the play area width and obstacle positions
        float playAreaWidth = 10f; // Total width of play area
        //float obstacleWidth = 1.8f; // Standard obstacle width
        int obstaclesPerRow = Mathf.RoundToInt(playAreaWidth / levelData.baseObstacleWidth);

        // Calculate positions to fully cover the row
        float startX = -(playAreaWidth / 2f) + (levelData.baseObstacleWidth / 2f);

        for (int i = 0; i < obstaclesPerRow; i++)
        {
            float xPosition = startX + (i * levelData.baseObstacleWidth);

            ObstacleData obstacle = CreateRandomObstacle(level, difficulty, recommendedForce, levelData);
            obstacle.position = new Vector3(xPosition, 0, zPosition);
            obstacle.width = levelData.baseObstacleWidth; // Ensure consistent width for full coverage

            obstacles.Add(obstacle);
        }

        // Ensure at least one obstacle is beatable for player progression
        EnsureRowIsPassable(obstacles, recommendedForce, levelData);

        return obstacles;
    }

    private void EnsureRowIsPassable(List<ObstacleData> rowObstacles, float recommendedForce, LevelData levelData)
    {
        if (rowObstacles.Count == 0) return;

        // Find the obstacle with lowest destruction value
        ObstacleData easiestObstacle = rowObstacles[0];
        float lowestDestructionValue = easiestObstacle.destructionValue;

        foreach (var obstacle in rowObstacles)
        {
            if (obstacle.destructionValue < lowestDestructionValue)
            {
                easiestObstacle = obstacle;
                lowestDestructionValue = obstacle.destructionValue;
            }
        }

        // Ensure the easiest obstacle is definitely beatable with reasonable health cost
        float maxBeatable = recommendedForce * 0.9f; // Should be beatable with 90% of recommended force
        if (easiestObstacle.destructionValue > maxBeatable)
        {
            easiestObstacle.destructionValue = UnityEngine.Random.Range(levelData.playerData.baseForce * 0.7f, maxBeatable);
            easiestObstacle.obstacleColor = _colorProvider.GetObstacleColor(easiestObstacle.destructionValue, recommendedForce);
        }
    }

    private ObstacleData CreateRandomObstacle(int level, float difficulty, float recommendedForce, LevelData levelData)
    {
        ObstacleData obstacle = new ObstacleData();
        obstacle.type = (ObstacleType)UnityEngine.Random.Range(0, 4);

        // Create varied difficulty distribution across the row
        float roll = UnityEngine.Random.value;
        if (roll < 0.3f) // 30% easily beatable
        {
            obstacle.destructionValue = UnityEngine.Random.Range(levelData.playerData.baseForce * 0.5f, levelData.playerData.baseForce * 0.9f);
        }
        else if (roll < 0.6f) // 30% medium difficulty
        {
            obstacle.destructionValue = UnityEngine.Random.Range(levelData.playerData.baseForce, recommendedForce * 0.7f);
        }
        else if (roll < 0.85f) // 25% require significant health
        {
            obstacle.destructionValue = UnityEngine.Random.Range(recommendedForce * 0.7f, recommendedForce * 1.1f);
        }
        else // 15% very challenging
        {
            obstacle.destructionValue = UnityEngine.Random.Range(recommendedForce * 1.1f, recommendedForce * 1.5f);
        }

        obstacle.width = 1.8f; // Fixed width for consistent row coverage
        obstacle.height = UnityEngine.Random.Range(1f, 3f);
        obstacle.obstacleColor = _colorProvider.GetObstacleColor(obstacle.destructionValue, recommendedForce);

        return obstacle;
    }

    private float CalculateRecommendedForce(LevelData levelData, LevelGenerationFormula formula)
    {
        float healthToUse = levelData.playerData.maxHealth * formula.recommendedHealthUsage;
        float bonusForce = healthToUse * levelData.playerData.healthToPowerRatio;
        return levelData.playerData.baseForce + bonusForce;
    }

    private ObstaclePattern SelectObstaclePattern(int level, float difficulty, LevelData levelData)
    {
        var availablePatterns = levelData.obstaclePatterns.FindAll(p => p.minLevelRequired <= level);

        if (availablePatterns.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var pattern in availablePatterns)
        {
            totalWeight += pattern.spawnProbability;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var pattern in availablePatterns)
        {
            currentWeight += pattern.spawnProbability;
            if (randomValue <= currentWeight)
            {
                return pattern;
            }
        }

        return availablePatterns[0];
    }

    private List<ObstacleData> GenerateFromPatterns(LevelData levelData)
    {
        var obstacles = new List<ObstacleData>();
        float currentZ = 2f;// Start closer to player spawn

        while (currentZ < levelData.levelLength)
        {
            if (levelData.obstaclePatterns.Count > 0)
            {
                var pattern = levelData.obstaclePatterns[UnityEngine.Random.Range(0, levelData.obstaclePatterns.Count)];
                obstacles.AddRange(GenerateFromPattern(pattern, currentZ));
                currentZ += pattern.patternLength;
            }
            else
            {
                currentZ += 10f;
            }
        }

        return obstacles;
    }

    private List<ObstacleData> GenerateFromPattern(ObstaclePattern pattern, float startZ)
    {
        var obstacles = new List<ObstacleData>();

        foreach (var obstacle in pattern.obstacles)
        {
            var newObstacle = new ObstacleData
            {
                type = obstacle.type,
                position = new Vector3(obstacle.position.x, obstacle.position.y, startZ + obstacle.position.z),
                destructionValue = obstacle.destructionValue,
                width = obstacle.width,
                height = obstacle.height,
                isDestructible = obstacle.isDestructible,
                obstacleColor = obstacle.obstacleColor
            };
            obstacles.Add(newObstacle);
        }

        return obstacles;
    }

    private ObstacleData CreateFinishLine(float levelLength)
    {
        return new ObstacleData
        {
            type = ObstacleType.FinishLine,
            position = new Vector3(0, 0, levelLength - 2f), // Place 2 units before the end
            destructionValue = 0f,
            width = 8f, // Make it wide so player can't miss it
            height = 3f,
            isDestructible = false,
            obstacleColor = _colorProvider.GetFinishLineColor()
        };
    }
}