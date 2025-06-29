using System;

using UnityEngine;

using VContainer.Unity;

public class GameController : IStartable, IDisposable
{
    ISoundManager soundManager;
    PlayerManager playerManager;
    ILevelBuilder levelBuilder;
    ILevelDataProvider levelDataProvider;
    private int currentLevel = 1;
    public GameController(ISoundManager soundManager, ILevelBuilder levelBuilder, ILevelDataProvider levelDataProvider)
    {
        this.soundManager = soundManager;
        this.levelBuilder = levelBuilder;
        this.levelDataProvider = levelDataProvider;
    }

    public void Dispose()
    {
        RemoveListeners();
    }

    public void Start()
    {
        AddListeners();
        playerManager = GameObject.FindFirstObjectByType<PlayerManager>();
        playerManager.SetPlayerData(levelDataProvider.GetLevelData(currentLevel).playerData);
    }

    public void GenerateCurrentLevel()
    {
        levelBuilder.GenerateLevel(currentLevel);
    }

    public void NextLevel()
    {
        currentLevel++;
        GenerateCurrentLevel();
    }

    public void RestartLevel()
    {
        GenerateCurrentLevel();
    }

    private void OnLevelGenerated(int levelNumber)
    {
        Debug.Log($"Level {levelNumber} generation completed!");
        // Handle level generation completion (UI updates, etc.)
    }

    private void OnPlayClick()
    {
        levelBuilder.GenerateLevel(currentLevel);
        PlayerController playerController = playerManager.GetPlayerController();
        playerController.StartPlayer();
    }

    private void AddListeners()
    {
        PlayGame.OnPlayClick += OnPlayClick;
        levelBuilder.OnLevelGenerated += OnLevelGenerated;
    }

    private void RemoveListeners()
    {
        PlayGame.OnPlayClick -= OnPlayClick;
        if (levelBuilder != null)
        {
            levelBuilder.OnLevelGenerated -= OnLevelGenerated;
        }
    }


}


