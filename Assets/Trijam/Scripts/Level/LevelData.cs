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
    public float pushbackDistance;
    public float movementSpeed = 5f;
    public float addedForce;

    public float boostDuration = 1f; // 1 second boost duration
    public float forceDecaySpeed = 5f; // How fast force decays per second
    [Range(0f, 1f)] public float addForceFactor;
    internal void ReduceHealth(float v)
    {
        health -= v;
    }

    public PlayerData()
    {
        health = maxHealth;
        addedForce = 0;
    }


}
