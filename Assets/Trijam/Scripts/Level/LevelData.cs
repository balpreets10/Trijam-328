using System;
using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Properties")]
    public int levelNumber;
    public float levelLength = 100f;
    public string levelName;
    public float rowSpacing = 20f;
    // Use fixed segment size for more consistent distribution
    public float segmentLength = 12f;
    public LevelDifficulty difficulty;

    [Header("Player Configuration")]
    public PlayerData playerData;

    [Header("Obstacle Configuration")]
    public List<ObstaclePattern> obstaclePatterns;
    public AnimationCurve difficultyProgression;

    [Header("Dynamic Generation")]
    public bool useDynamicGeneration = true;
    public LevelGenerationFormula generationFormula;

    public float baseObstacleWidth = 1.5f;

    [ContextMenu("Generate Based on Level")]
    public void GenerateValuesBasedOnLevel()
    {
        if (generationFormula == null)
        {
            Debug.LogWarning("No generation formula assigned. Using default values.");
            return;
        }

        // Calculate level-based values
        float calculatedDifficulty = generationFormula.CalculateDifficulty(levelNumber);
        float calculatedDensity = generationFormula.CalculateObstacleDensity(levelNumber);

        // Set difficulty based on calculated values
        difficulty = GetDifficultyFromValue(calculatedDifficulty);

        // Adjust level length based on level number
        levelLength = Mathf.Clamp(50f + (levelNumber * 10f), 100f, 800f);

        // Adjust player stats based on level progression
        if (playerData == null) playerData = new PlayerData();

        // Base values increase with level
        playerData.baseForce = Mathf.Clamp(10f + (levelNumber * 0.5f), 10f, 30f);
        playerData.maxHealth = Mathf.Clamp(100f + (levelNumber * 5f), 100f, 200f);
        playerData.healthToPowerRatio = Mathf.Clamp(2f + (levelNumber * 0.05f), 2f, 4f);
        playerData.movementSpeed = Mathf.Clamp(5f + (levelNumber * 0.1f), 5f, 8f);

        // Reset current health to max
        playerData.health = playerData.maxHealth;

        // Adjust generation parameters
        rowSpacing = Mathf.Clamp(20f - (levelNumber * 0.2f), 12f, 25f);
        segmentLength = Mathf.Clamp(12f + (levelNumber * 0.1f), 12f, 25f);
        baseObstacleWidth = Mathf.Clamp(1.5f - (levelNumber * 0.01f), 1f, 1.5f);

        // Set level name based on number and difficulty
        levelName = $"Level {levelNumber} - {difficulty}";

        Debug.Log($"Generated values for Level {levelNumber}: " +
                  $"\nDifficulty: {difficulty} (Value: {calculatedDifficulty:F1})" +
                  $"\nObstacle Density: {calculatedDensity:F2}" +
                  $"\nPlayer Force: {playerData.baseForce:F1}" +
                  $"\nPlayer Health: {playerData.maxHealth:F0}");

        RenameAsset();
    }

    private LevelDifficulty GetDifficultyFromValue(float difficultyValue)
    {
        if (difficultyValue < 2f) return LevelDifficulty.Easy;
        if (difficultyValue < 4f) return LevelDifficulty.Medium;
        if (difficultyValue < 6f) return LevelDifficulty.Hard;
        if (difficultyValue < 8f) return LevelDifficulty.Expert;
        return LevelDifficulty.Nightmare;
    }

    private void RenameAsset()
    {
#if UNITY_EDITOR
        // Get the current asset path
        string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        if (!string.IsNullOrEmpty(assetPath))
        {
            // Generate new name based on level number and difficulty
            string newName = $"Level_{levelNumber:00}_{difficulty}";

            // Rename the asset
            UnityEditor.AssetDatabase.RenameAsset(assetPath, newName);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            Debug.Log($"Renamed asset to: {newName}");
        }
#endif
    }
}

[System.Serializable]
public enum LevelDifficulty
{
    Easy, Medium, Hard, Expert, Nightmare
}


[Serializable]
public class PlayerData
{
    public float baseForce = 10f;
    public float maxHealth = 100f;
    public float healthToPowerRatio = 2f;
    public float health;
    public float pushbackDistance = 3;
    public float movementSpeed = 5f;
    public float addedForce;

    public float boostDuration = 1f; // 1 second boost duration
    public float forceDecaySpeed = 5f; // How fast force decays per second
    [Range(0f, 1f)] public float addForceFactor = 0.01f;
    internal void ReduceHealth(float v)
    {
        health -= v;
    }

    public PlayerData()
    {
        health = maxHealth;
        addedForce = 0;
    }

    public void ResetData()
    {
        health = maxHealth;
        addedForce = 0;
    }




}
