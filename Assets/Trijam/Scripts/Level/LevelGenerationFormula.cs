using UnityEngine;

[System.Serializable]
public class LevelGenerationFormula
{
    [Header("Difficulty Scaling")]
    public float baseDifficulty = 1f;
    public float difficultyMultiplier = 0.3f;
    public float maxDifficulty = 10f;

    [Header("Obstacle Density")]
    public float baseObstacleDensity = 0.1f;
    public float densityGrowthRate = 0.05f;
    public float maxObstacleDensity = 0.9f; // Cap density to avoid overwhelming players

    [Header("Destruction Values")]
    public float baseDestructionValue = 5f;
    public float destructionGrowthRate = 1.5f;
    public float destructionVariance = 0.3f;

    [Header("Power Balance")]
    public float recommendedHealthUsage = 0.3f;
    public bool ensureBeatable = true;

    public float CalculateDifficulty(int level)
    {
        return Mathf.Min(baseDifficulty + (level * difficultyMultiplier), maxDifficulty);
    }

    public float CalculateObstacleDensity(int level)
    {
        float density = baseObstacleDensity + (level * densityGrowthRate);
        return Mathf.Min(density, maxObstacleDensity);
    }

    public float CalculateDestructionValue(int level, float difficulty)
    {
        float baseValue = baseDestructionValue + (level * destructionGrowthRate);
        float variance = UnityEngine.Random.Range(-destructionVariance, destructionVariance);
        return baseValue * difficulty * (1f + variance);
    }
}
