using Balpreet.AudioSystem;

using UnityEngine;

using VContainer;
using VContainer.Unity;

using static UnityEngine.Rendering.STP;

public class GameScope : LifetimeScope
{
    [SerializeField] public SoundManager SoundManager;
    [SerializeField] private LevelSystemConfig config;
    [SerializeField] private ObstacleColorConfig colorConfigAsset;
    [SerializeField] private Transform pooledObjectHolder;
    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.Register(resolver => SoundManager, Lifetime.Singleton)
        .As<ISoundManager>();
        //builder.Register(resolver => SoundManager, Lifetime.Singleton)
        //.As<ISoundManager>();
        builder.RegisterEntryPoint<GameController>();
        builder.RegisterInstance(config);
        builder.RegisterInstance(pooledObjectHolder);
        builder.RegisterInstance<ObstacleColorConfig>(colorConfigAsset);


        // Register services
        builder.Register<ILevelDataProvider, LevelDataProvider>(Lifetime.Singleton);
        builder.Register<IObstacleColorProvider, ObstacleColorProvider>(Lifetime.Singleton);
        builder.Register<IObstacleFactory, ObstacleFactory>(Lifetime.Singleton);
        builder.Register<ILevelGenerator, LevelGenerator>(Lifetime.Singleton);
        builder.Register<IObstacleSpawner, ObstacleSpawner>(Lifetime.Singleton);
        builder.Register<ILevelBuilder, LevelBuilder>(Lifetime.Singleton);
    }
}
