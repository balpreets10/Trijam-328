using System;

using DG.Tweening;

using UnityEngine;

using VContainer.Unity;

public class GameController : IStartable, IDisposable
{
    ISoundManager soundManager;
    PlayerManager playerManager;
    ILevelBuilder levelBuilder;
    ILevelDataProvider levelDataProvider;
    IPreferenceService preferenceService;
    IObstacleFactory obstacleFactory;
    private int currentLevel = 1;
    LevelState levelState = LevelState.Init;

    private GameObject InitialCameraTranform;
    private Transform cameraTransform;

    PlayerController playerController;

    public GameController(ISoundManager soundManager, ILevelBuilder levelBuilder, ILevelDataProvider levelDataProvider, IPreferenceService preferenceService, IObstacleFactory obstacleFactory)
    {
        this.soundManager = soundManager;
        this.levelBuilder = levelBuilder;
        this.levelDataProvider = levelDataProvider;
        this.preferenceService = preferenceService;
        this.obstacleFactory = obstacleFactory;
    }

    public void Dispose()
    {
        RemoveListeners();
    }

    public void Start()
    {
        InitialCameraTranform = GameObject.Find("InitialCameraPosition");
        cameraTransform = Camera.main.transform;
        AddListeners();
        currentLevel = preferenceService.CurrentLevel;
        GenerateCurrentLevel();
        playerManager = GameObject.FindFirstObjectByType<PlayerManager>();
    }

    public void GenerateCurrentLevel()
    {
        levelBuilder.GenerateLevel(currentLevel);
    }

    private void SetPlayerData()
    {
        Debug.Log($"Setting Data{levelDataProvider.GetLevelData(currentLevel).playerData.health}");
        playerManager.SetPlayerData(levelDataProvider.GetLevelData(currentLevel).playerData);
    }

    //public void NextLevel()
    //{
    //    currentLevel++;
    //}

    public void RestartLevel()
    {
        SetPlayerData();
        levelBuilder.ActivateLevel(currentLevel);
    }

    private void OnLevelGenerated(int levelNumber)
    {
        Debug.Log($"Level {levelNumber} generation completed!");
        levelState = LevelState.Generated;
        // Handle level generation completion (UI updates, etc.)
    }

    private void OnPlayClick()
    {
        levelDataProvider.GetLevelData(currentLevel).playerData.ResetData();
        SetPlayerData();
        obstacleFactory.ActivateLevel();
        playerController = playerManager.GetPlayerController();
        playerController.Activate();
        TweenCamera();
        //playerController.StartPlayer();
    }

    private void TweenCamera()
    {
        cameraTransform.DORotateQuaternion(playerController.GetMyCamera().transform.rotation, 1f).SetEase(Ease.Linear);
        cameraTransform.DOMove(playerController.GetMyCamera().transform.position, 1f).SetEase(Ease.Linear).OnComplete(
            () =>
            {
                playerController.StartPlayer();
            }
            );
    }

    private void AddListeners()
    {
        PlayGame.OnPlayClick += OnPlayClick;
        Obstacle.OnLevelCompleted += OnLevelCompleted;
        PlayerController.OnPlayerDied += OnPlayerDied;
        levelBuilder.OnLevelGenerated += OnLevelGenerated;
    }

    private void RemoveListeners()
    {
        PlayGame.OnPlayClick -= OnPlayClick;
        Obstacle.OnLevelCompleted -= OnLevelCompleted;
        PlayerController.OnPlayerDied -= OnPlayerDied;
        if (levelBuilder != null)
        {
            levelBuilder.OnLevelGenerated -= OnLevelGenerated;
        }
    }

    private void OnPlayerDied()
    {
        levelState = LevelState.Restarted;
        SetupLevel();
    }

    private void OnLevelCompleted()
    {
        levelState = LevelState.Completed;
        preferenceService.CurrentLevel = ++currentLevel;
        SetupLevel();

    }

    private void SetupLevel()
    {
        cameraTransform.localPosition = playerController.GetMyCamera().transform.position;
        playerController.Deactivate();
        if (levelState == LevelState.Completed)
            GenerateCurrentLevel();
        else if (levelState == LevelState.Restarted)
            levelBuilder.ActivateLevel(currentLevel);
        ResetCamera();
    }

    private void ResetCamera()
    {
        cameraTransform.DORotateQuaternion(InitialCameraTranform.transform.rotation, 1f).SetEase(Ease.Linear);
        cameraTransform.DOMove(InitialCameraTranform.transform.position, 1f).SetEase(Ease.Linear);
    }
}

public enum LevelState
{
    Init,
    Generated,
    Playing,
    Completed,
    Restarted
}


